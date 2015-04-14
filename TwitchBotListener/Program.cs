using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Web;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Configuration;

namespace TwitchBotListener
{
    /// <summary>
    /// An HTTP listener for GroupMe callbacks that responds to certain messages with silly things
    /// </summary>
    class TwitchBotListener
    {
        public static void Main()
        {
            HttpListener listener = new HttpListener();
            int port = 8090;

            listener.Prefixes.Add(string.Format("http://*:{0}/", port));

            try
            {
                listener.Start();
            }
            catch (HttpListenerException ex)
            {
                if (ex.ErrorCode == 5)
                {
                    Console.Write("URL Reservation missing");
                }
                throw;
            }

            Listen_async(listener).GetAwaiter().GetResult();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// Listen asynchronously on our listener and process messages as they come in
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static async Task Listen_async(HttpListener listener)
        {
            while (true)
            {
                Console.WriteLine("Listening for new message...");

                Process_async(await listener.GetContextAsync());
            }
        }

        /// <summary>
        /// Process any received message and determine an action based on its content
        /// </summary>
        /// <param name="message"></param>
        public static async void Process_async(HttpListenerContext message)
        {
            string botId = ConfigurationSettings.AppSettings["BOT_ID"];

            HttpListenerResponse response = message.Response;
            var json = new StreamReader(message.Request.InputStream).ReadToEnd();
            dynamic responseObject = JsonConvert.DeserializeObject(json);
            string text = responseObject.text;
            string sndMsg = "";
            bool hasAttachment = false;
            var attachments = new Attachment{};
            int flag = 1;

            //Begin checking for message content, set flag based on whether or not we received an important message
            if (text.Contains("Kappa"))
            {
                hasAttachment = true;
                attachments = new Attachment
                {
                    type = "image",
                    url = "http://i.groupme.com/25x28.png.ed8bf2c9b9084d9d8e00474bbd1e8a5f"
                };
            }
            else if (text.Contains("Kreygasm"))
            {
                hasAttachment = true;
                attachments = new Attachment
                {
                    type = "image",
                    url = "http://i.groupme.com/19x27.png.e885b116ad954e698e682975b00874cd"
                };
            }
            else if (Contains(text, "is guiseppe a noob?"))
            {
                sndMsg = "yes";
            }
            else if (Contains(text, "is perry a noob?"))
            {
                sndMsg = "yes. muffin is noob. it is known.";
            }
            else if (Contains(text, "rekt"))
            {
                sndMsg = getRekt();
            }
            else if (Contains(text, "told"))
            {
                sndMsg = getTold();
            }
            else if (Contains(text, "!roll")) 
            {
                sndMsg = rollDice(responseObject.name);
            }
            else {
                flag = 0;
            }
            //Check if a valid response was recieved and if it wasn't from the bot
            if (flag == 1 && responseObject.sender_id != "204062")
            {
                dynamic post;

                if (hasAttachment)
                {
                    var a = new Attachment[1];
                    a[0] = attachments;

                    post = new BotPictureMsg
                    {
                        botId = botId,
                        attachments = a
                    };
                }
                else
                {
                    post = new BotTextMsg
                    {
                        botId = botId,
                        text = sndMsg
                    };
                }

                var serialPost = JsonConvert.SerializeObject(post);

                using (var content = new ByteArrayContent(Encoding.UTF8.GetBytes(serialPost)))

                using (var client = new HttpClient())
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    await Task.Delay(2000);
                    await client.PostAsync("https://api.groupme.com/v3/bots/post", content);
                }
            }
        }

        /// <summary>
        /// Function for case insensitive Ctrl+F
        /// </summary>
        /// <param name="source"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static bool Contains(string source, string compare)
        {
            return Regex.IsMatch(source, compare, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Create response message for "rekt"
        /// </summary>
        /// <returns></returns>
        public static string getRekt()
        {
            string[] rekt = { "☐ Not rekt\n☑ REKTangle", "☐ Not rekt\n☑ SHREKT", "☐ Not rekt\n☑ The Good, the Bad, and the REKT", "☐ Not rekt\n☑ LawREKT of Arabia", "☐ Not rekt\n☑ Tyrannosaurus REKT", "☐ Not rekt\n☑ REKT-it Ralph", "☐ Not rekt\n☑ The Lord of the REKT", "☐ Not rekt\nThe Usual SusREKTs ☑", "☐ Not rekt\n☑ REKT to the Future", "☐ Not rekt\n☑ eREKTile dysfunction" };
            Random r = new Random();
            return rekt[(int)r.Next(0, 9)];
        }

        /// <summary>
        /// Create response message for "told"
        /// </summary>
        /// <returns></returns>
        public static string getTold()
        {
            string[] told = {
                                "☐ Not Told\n☑ TOLDASAURUS REX",
"☐ Not Told\n☑ Cash4Told.com",
"☐ Not Told\n☑ No Country for Told Men",
"☐ Not Told\n☑ Knights of the Told Republic",
"☐ Not Told\n☑ ToldSpice",
"☐ Not Told\n☑ The Elder Tolds IV: Oblivious",
"☐ Not Told\n☑ Command & Conquer: Toldberian Sun",
"☐ Not Told\n☑ GuiTold Hero: World Told",
"☐ Not Told\n☑ Told King of Boletaria",
"☐ Not Told\n☑ Countold Strike",
"☐ Not Told\n☑ Unreal Toldament",
"☐ Not Told\n☑ Stone-told Steve Austin",
"☐ Not Told\n☑ Half Life 2: Episode Told",
"☐ Not Told\n☑ Roller Coaster Toldcoon",
"☐ Not Told\n☑ Assassin’s Creed: Tolderhood",
"☐ Not Told\n☑ Battletolds",
"☐ Not Told\n☑ S.T.A.L.K.E.R.: Shatold of Chernobyl",
"☐ Not Told\n☑ Toldasauraus Rex 2: Electric Toldaloo",
"☐ Not Told\n☑ Told of Duty 4: Modern Toldfare",
"☐ Not Told\n☑ Pokemon Told and Silver",
"☐ Not Told\n☑ The Legend of Eldorado : The Lost City of Told",
"☐ Not Told\n☑ Rampage: Toldal Destruction",
"☐ Not Told\n☑ Told Fortress Classic",
"☐ Not Told\n☑ Toldman: Arkham Told",
"☐ Not Told\n☑ The Good, The Bad, and The Told",
"☐ Not Told\n☑ Super Mario SunTold",
"☐ Not Told\n☑ Legend of Zelda: Toldacarnia of Time",
"☐ Not Told\n☑ Toldstone Creamery",
"☐ Not Told\n☑ Mario Golf: Toldstool Tour",
"☐ Not Told\n☑ Super Told Boy",
"☐ Not Told\n☑ Sir Barristan the Told",
"☐ Not Told\n☑ Left 4 Told",
"☐ Not Told\n☑ Battoldfield: Bad Company 2",
"☐ Not Told\n☑ Toldman Sachs",
"☐ Not Told\n☑ Conker’s Bad Fur Day: Live and Retolded",
"☐ Not Told\n☑ Lead and Told: Gangs of the Wild West",
"☐ Not Told\n☑ Portold 2",
"☐ Not Told\n☑ Avatold: The Last Airbender",
"☐ Not Told\n☑ Dragon Ball Z Toldkaichi Budokai",
"☐ Not Told\n☑ Toldcraft II: Tolds of Toldberty",
"☐ Not Told\n☑ Leo Toldstoy",
"☐ Not Told\n☑ Metal Gear Toldid 3: Snake Eater",
"☐ Not Told\n☑ 3D Dot Told Heroes",
"☐ Not Told\n☑ J.R.R Toldkien’s Lord of the Told",
"☐ Not Told\n☑ Told you that PS3 has no games",
"☐ Not Told\n☑ LitTOLD Big Planet",
"☐ Not Told\n☑ Rome: Toldal War",
"☐ Not Told\n☑ Gran Toldrismo 5",
"☐ Not Told\n☑ Told Calibur 4",
"☐ Not Told\n☑ Told Fortress 2",
"☐ Not Told\n☑ Castlevania: RonTold of Blood",
"☐ Not Told\n☑ Guilty Gear XX Accent Told",
"☐ Not Told\n☑ Cyndaquil, Chicorita, and Toldodile",
"☐ Not Told\n☑ Was foretold",
"☐ Not Told\n☑ Demon’s Told",
"☐ Not Told\n☑ http://www.youtold.com",
"☐ Not Told\n☑ Tolden Sun: Dark Dawn",
"☐ Not Told\n☑ Tic-Tac-Told",
"☐ Not Told\n☑ Biotold 2",
"☐ Not Told\n☑ Toldbound",
"☐ Not Told\n☑ Icetold",
"☐ Not Told\n☑ Told of the Rings",
"☐ Not Told\n☑ Hisoutentoldu",
"☐ Not Told\n☑ Told dish of revenge served",
"☐ Not Told\n☑ Apply told water to burnt area",
"☐ Not Told\n☑ The Tolden Rule",
"☐ Not Told\n☑ Dark Tolds",
"☐ Not Told\n☑ Told Story",
"☐ Not Told\n☑ Tolden Axe",
"☐ Not Told\n☑ Gary Toldman",
"☐ Not Told\n☑ Told MacDonald Had a Farm",
"☐ Not Told\n☑ Super Told XLVIII",
"☐ Not Told\n☑ Told Finger",
"☐ Not Told\n☑ Toldeneye",
"☐ Not Told\n☑ Told and Tolder",
"☐ Not Told\n☑ Told and Tolder Told",
"☐ Not Told\n☑ Lord Toldermort",
"☐ Not Told\n☑ Told Bond: Medicated Powder",
"☐ Not Told\n☑ The Tolder Scrolls",
"☐ Not Told\n☑ House Toldgaryan",
"☐ Not Told\n☑ Toldèmon O/P",
"☐ Not Told\n☑ Told Testament",
"☐ Not Told\n☑ World of Toldcraft: The Burning Told",
"☐ Not Told\n☑ All Told Everything",
"☐ Not Told\n☑ JRR Toldkien",
"☐ Not Told\n☑ Reddit Told",
"☐ Not Told\n☑ Told's spaghetti",
"☐ Not Told\n☑ The Toldman Show",
"☐ Not Told\n☑ Mementold",
"☐ Not Told\n☑ Toldega Nights: The Ballad of Ricky Toldy Battold Royale",
"☐ Not Told\n☑ I'll have the toldalini Alfredo, please",
"☐ Not Told\n☑ The Big Letoldski",
"☐ Not Told\n☑ Tolddock Saints",
"☐ Not Told\n☑ Legend of Total Toldage",
"☐ Not Told\n☑ carved into a toldem pole",
"☐ Not Told\n☑ DIS NIGGA GOT TOLD",
"☐ Not Told\n☑ Told-finger death punch",
"☐ Not Told\n☑ Told alexandra",
"☐ Not Told\n☑ Bring me the told",
"☐ Not Told\n☑ toldboy and the tolden army",
"☐ Not Told\n☑ Followed the tolden brick road",
"☐ Not Told\n☑ Fear and Loathing in Told Vegas",
"☐ Not Told\n☑ Told Mountain",
"☐ Not Told\n☑ Snoop told",
"☐ Not Told\n☑ Toldclub",
"☐ Not Told\n☑ The Told and the Textless",
"☐ Not Told\n☑ Tolden Caulfield"
        };
            Random r = new Random();
            return told[(int)r.Next(0, 106)];
        }
        public string rollDice(string name)
        {
            Random r = new Random();
            return String.Format("{0} rolls a {1}!", name, r.Next(1, 100).ToString());
        }
    }

    /// <summary>
    /// Sent Message structures
    /// </summary>
    [DataContract]
    public class BotPictureMsg
    {
        [DataMember(Name = "bot_id")]
        public string botId { get; set; }
 
        [DataMember(Name="attachments")]
        public Attachment[] attachments { get; set; }
    }

    [DataContract]
    public class BotTextMsg
    {
        [DataMember(Name = "bot_id")]
        public string botId { get; set; }

        [DataMember(Name = "text")]
        public string text { get; set; }
    }

    public class Attachment
    {
        public string type { get; set; }
        public string url { get; set; }
    }
}
