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


        public static async Task Listen_async(HttpListener listener)
        {
            while (true)
            {
                Console.WriteLine("Listening for new message...");

                Process_async(await listener.GetContextAsync());
            }
        }

        public static async void Process_async(HttpListenerContext message)
        {
            string botId = ConfigurationSettings.AppSettings["BOT_ID"];

            HttpListenerResponse response = message.Response;
            var json = new StreamReader(message.Request.InputStream).ReadToEnd();
            dynamic responseObject = JsonConvert.DeserializeObject(json);
            string text = responseObject.text;
            string sndMsg = "";
            bool hasAttachment = false;
            var attachments = new attachment{};
            int flag = 1;
            //Read message and check if it contains "Kappa"
            //If so, send kappa
            if (text.Contains("Kappa"))
            {
                hasAttachment = true;
                attachments = new attachment
                {
                    type = "image",
                    url = "http://i.groupme.com/25x28.png.ed8bf2c9b9084d9d8e00474bbd1e8a5f"
                };
            }
            else if (text.Contains("Kreygasm"))
            {
                hasAttachment = true;
                attachments = new attachment
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
            else
            {
                flag = 0;
            }
            //Check if a valid response was recieved and if it wasn't from the bot
            if (flag == 1 && responseObject.sender_id != "204062")
            {
                dynamic post;

                if (hasAttachment)
                {
                    var a = new attachment[1];
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

        public static bool Contains(string source, string compare)
        {
            return Regex.IsMatch(source, compare, RegexOptions.IgnoreCase);
        }

        public static string getRekt()
        {
            string[] rekt = { "☑ REKTangle", "☑ SHREKT", "☑ The Good, the Bad, and the REKT", "☑ LawREKT of Arabia", "☑ Tyrannosaurus REKT", "☑ REKT-it Ralph", "☑ The Lord of the REKT", "The Usual SusREKTs ☑", "☑ REKT to the Future", "☑ eREKTile dysfunction"};
            Random r = new Random();
            return rekt[(int)r.Next(0, 9)];
        }
        public static string getTold()
        {
            string[] told = {
                                "☐ Not Told\t\t☑ TOLDASAURUS REX",
"☐ Not Told\t\t☑ Cash4Told.com",
"☐ Not Told\t\t☑ No Country for Told Men",
"☐ Not Told\t\t☑ Knights of the Told Republic",
"☐ Not Told\t\t☑ ToldSpice",
"☐ Not Told\t\t☑ The Elder Tolds IV: Oblivious",
"☐ Not Told\t\t☑ Command & Conquer: Toldberian Sun",
"☐ Not Told\t\t☑ GuiTold Hero: World Told",
"☐ Not Told\t\t☑ Told King of Boletaria",
"☐ Not Told\t\t☑ Countold Strike",
"☐ Not Told\t\t☑ Unreal Toldament",
"☐ Not Told\t\t☑ Stone-told Steve Austin",
"☐ Not Told\t\t☑ Half Life 2: Episode Told",
"☐ Not Told\t\t☑ Roller Coaster Toldcoon",
"☐ Not Told\t\t☑ Assassin’s Creed: Tolderhood",
"☐ Not Told\t\t☑ Battletolds",
"☐ Not Told\t\t☑ S.T.A.L.K.E.R.: Shatold of Chernobyl",
"☐ Not Told\t\t☑ Toldasauraus Rex 2: Electric Toldaloo",
"☐ Not Told\t\t☑ Told of Duty 4: Modern Toldfare",
"☐ Not Told\t\t☑ Pokemon Told and Silver",
"☐ Not Told\t\t☑ The Legend of Eldorado : The Lost City of Told",
"☐ Not Told\t\t☑ Rampage: Toldal Destruction",
"☐ Not Told\t\t☑ Told Fortress Classic",
"☐ Not Told\t\t☑ Toldman: Arkham Told",
"☐ Not Told\t\t☑ The Good, The Bad, and The Told",
"☐ Not Told\t\t☑ Super Mario SunTold",
"☐ Not Told\t\t☑ Legend of Zelda: Toldacarnia of Time",
"☐ Not Told\t\t☑ Toldstone Creamery",
"☐ Not Told\t\t☑ Mario Golf: Toldstool Tour",
"☐ Not Told\t\t☑ Super Told Boy",
"☐ Not Told\t\t☑ Sir Barristan the Told",
"☐ Not Told\t\t☑ Left 4 Told",
"☐ Not Told\t\t☑ Battoldfield: Bad Company 2",
"☐ Not Told\t\t☑ Toldman Sachs",
"☐ Not Told\t\t☑ Conker’s Bad Fur Day: Live and Retolded",
"☐ Not Told\t\t☑ Lead and Told: Gangs of the Wild West",
"☐ Not Told\t\t☑ Portold 2",
"☐ Not Told\t\t☑ Avatold: The Last Airbender",
"☐ Not Told\t\t☑ Dragon Ball Z Toldkaichi Budokai",
"☐ Not Told\t\t☑ Toldcraft II: Tolds of Toldberty",
"☐ Not Told\t\t☑ Leo Toldstoy",
"☐ Not Told\t\t☑ Metal Gear Toldid 3: Snake Eater",
"☐ Not Told\t\t☑ 3D Dot Told Heroes",
"☐ Not Told\t\t☑ J.R.R Toldkien’s Lord of the Told",
"☐ Not Told\t\t☑ Told you that PS3 has no games",
"☐ Not Told\t\t☑ LitTOLD Big Planet",
"☐ Not Told\t\t☑ Rome: Toldal War",
"☐ Not Told\t\t☑ Gran Toldrismo 5",
"☐ Not Told\t\t☑ Told Calibur 4",
"☐ Not Told\t\t☑ Told Fortress 2",
"☐ Not Told\t\t☑ Castlevania: RonTold of Blood",
"☐ Not Told\t\t☑ Guilty Gear XX Accent Told",
"☐ Not Told\t\t☑ Cyndaquil, Chicorita, and Toldodile",
"☐ Not Told\t\t☑ Was foretold",
"☐ Not Told\t\t☑ Demon’s Told",
"☐ Not Told\t\t☑ http://www.youtold.com",
"☐ Not Told\t\t☑ Tolden Sun: Dark Dawn",
"☐ Not Told\t\t☑ Tic-Tac-Told",
"☐ Not Told\t\t☑ Biotold 2",
"☐ Not Told\t\t☑ Toldbound",
"☐ Not Told\t\t☑ Icetold",
"☐ Not Told\t\t☑ Told of the Rings",
"☐ Not Told\t\t☑ Hisoutentoldu",
"☐ Not Told\t\t☑ Told dish of revenge served",
"☐ Not Told\t\t☑ Apply told water to burnt area",
"☐ Not Told\t\t☑ The Tolden Rule",
"☐ Not Told\t\t☑ Dark Tolds",
"☐ Not Told\t\t☑ Told Story",
"☐ Not Told\t\t☑ Tolden Axe",
"☐ Not Told\t\t☑ Gary Toldman",
"☐ Not Told\t\t☑ Told MacDonald Had a Farm",
"☐ Not Told\t\t☑ Super Told XLVIII",
"☐ Not Told\t\t☑ Told Finger",
"☐ Not Told\t\t☑ Toldeneye",
"☐ Not Told\t\t☑ Told and Tolder",
"☐ Not Told\t\t☑ Told and Tolder Told",
"☐ Not Told\t\t☑ Lord Toldermort",
"☐ Not Told\t\t☑ Told Bond: Medicated Powder",
"☐ Not Told\t\t☑ The Tolder Scrolls",
"☐ Not Told\t\t☑ House Toldgaryan",
"☐ Not Told\t\t☑ Toldèmon O/P",
"☐ Not Told\t\t☑ Told Testament",
"☐ Not Told\t\t☑ World of Toldcraft: The Burning Told",
"☐ Not Told\t\t☑ All Told Everything",
"☐ Not Told\t\t☑ JRR Toldkien",
"☐ Not Told\t\t☑ Reddit Told",
"☐ Not Told\t\t☑ Told's spaghetti",
"☐ Not Told\t\t☑ The Toldman Show",
"☐ Not Told\t\t☑ Mementold",
"☐ Not Told\t\t☑ Toldega Nights: The Ballad of Ricky Toldy Battold Royale",
"☐ Not Told\t\t☑ I'll have the toldalini Alfredo, please",
"☐ Not Told\t\t☑ The Big Letoldski",
"☐ Not Told\t\t☑ Tolddock Saints",
"☐ Not Told\t\t☑ Legend of Total Toldage",
"☐ Not Told\t\t☑ carved into a toldem pole",
"☐ Not Told\t\t☑ DIS NIGGA GOT TOLD",
"☐ Not Told\t\t☑ Told-finger death punch",
"☐ Not Told\t\t☑ Told alexandra",
"☐ Not Told\t\t☑ Bring me the told",
"☐ Not Told\t\t☑ toldboy and the tolden army",
"☐ Not Told\t\t☑ Followed the tolden brick road",
"☐ Not Told\t\t☑ Fear and Loathing in Told Vegas",
"☐ Not Told\t\t☑ Told Mountain",
"☐ Not Told\t\t☑ Snoop told",
"☐ Not Told\t\t☑ Toldclub",
"☐ Not Told\t\t☑ The Told and the Textless",
"☐ Not Told\t\t☑ Tolden Caulfield"
        };
            Random r = new Random();
            return told[(int)r.Next(0, 106)];
        }
    }

    [DataContract]
    public class BotPictureMsg
    {
        [DataMember(Name = "bot_id")]
        public string botId { get; set; }
 
        [DataMember(Name="attachments")]
        public attachment[] attachments { get; set; }
    }

    [DataContract]
    public class BotTextMsg
    {
        [DataMember(Name = "bot_id")]
        public string botId { get; set; }

        [DataMember(Name = "text")]
        public string text { get; set; }
    }

    public class attachment
    {
        public string type { get; set; }
        public string url { get; set; }
    }
}
