using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Linq;

namespace TwitchBotListener
{

    /// <summary>
    /// An HTTP listener for GroupMe callbacks that responds to certain messages with silly things
    /// </summary>
    class TwitchBotListener
    {
        //TODO:
        //Implement a dictionary of sniperinos, organized by name, that can be serialized and deserialized
        //from json easily for storage and such. sniperino class has scores for now.
        public static List<Sniperino> sniperinos = new List<Sniperino>();

        public static readonly string toldLoc = "Json\\told.json";
        public static readonly string rektLoc = "Json\\rekt.json";

        public static void Main()
        {
            HttpListener listener = new HttpListener();
            int port = Convert.ToInt32(ConfigurationSettings.AppSettings["PORT"]);

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

                ProcessAsync(await listener.GetContextAsync());
            }
        }

        /// <summary>
        /// Process any received message and determine an action based on its content
        /// </summary>
        /// <param name="message"></param>
        public static async void ProcessAsync(HttpListenerContext message)
        {
            string groupId = ConfigurationSettings.AppSettings["GROUP_ID"];
            string botSenderId = ConfigurationSettings.AppSettings["BOT_SENDER_ID"];

            HttpListenerResponse response = message.Response;
            var json = new StreamReader(message.Request.InputStream).ReadToEnd();
            dynamic responseObject;

            //Sanitize input. If non-json is sent, return
            try
            {
                responseObject = JsonConvert.DeserializeObject(json);
            }
            catch(Exception ex)
            {
                Console.WriteLine("dynamic responseObject = JsonConvert.DeserializeObject(json); threw" + ex.ToString());
                return;
            }
            string text;
            string name;
            string group_id;
            try
            {
                text = responseObject.text;
                name = responseObject.name;
                group_id = responseObject.group_id;
            }
            catch (Exception e)
            {
                return;
            }
            string sndMsg = "";
            var attachments = new Attachment{};
            int flag = 1;
            bool hasAttachments = false;

            //check for message with valid group_id
            if (group_id == groupId)
            {
                //Begin checking for message content, set flag based on whether or not we received an important message
                if (text.Contains("Kappa"))
                {
                    attachments = new Attachment
                    {
                        type = "image",
                        url = "http://i.groupme.com/25x28.png.ed8bf2c9b9084d9d8e00474bbd1e8a5f"
                    };
                    hasAttachments = true;
                }
                else if (text.Contains("Kreygasm"))
                {
                    attachments = new Attachment
                    {
                        type = "image",
                        url = "http://i.groupme.com/19x27.png.e885b116ad954e698e682975b00874cd"
                    };
                    hasAttachments = true;
                }
                //TODO:
                //PJSalt
                else if (Contains(text, "is guiseppe a noob?"))
                {
                    sndMsg = "Yes. Except during games of HOTS. Then the rest of us are noobs and he is god.";
                }
                else if (Contains(text, "is perry a noob?"))
                {
                    sndMsg = "Yes. Muffin is noob. It is known.";
                }
                else if (Contains(text, "is jason a noob?"))
                {
                    sndMsg = "Does Tom Brady sit when he pees? Same answer.";
                }
                else if (Contains(text, "skeltal"))
                {
                    sndMsg = "i'm triggered";
                }
                else if (Contains(text, "spooky"))
                {
                    sndMsg = tooSpooky();
                }
                else if (Contains(text, "is blum a noob?"))
                {
                    sndMsg = "Who is blum? Does not compute. User must play games more than once a year to have noobness evaluated.";
                }
                else if (Contains(text, "triforce"))
                {
                    sndMsg = " ▲\n▲ ▲";
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
                    sndMsg = rollDice(name);
                }
                else if (Contains(text, "molly"))
                {
                    sndMsg = "༼ つ ◕_◕ ༽つ GIVE MOLLY ༼ つ ◕_◕ ༽つ";
                }
                else if (Contains(text, "!sniperino"))
                {
                    sndMsg = playSniperino(name);
                }
                else if (Contains(text, "ameno"))
                {
                    sndMsg = "༼ つ ◕_◕ ༽つ AMENO ༼ つ ◕_◕ ༽つ༼ つ ◕_◕ ༽つ AMENO ༼ つ ◕_◕ ༽つ༼ つ ◕_◕ ༽つ AMENO ༼ つ ◕_◕ ༽つ༼ つ ◕_◕ ༽つ AMENO ༼ つ ◕_◕ ༽つ༼ つ ◕_◕ ༽つ AMENO ༼ つ ◕_◕ ༽つ༼ つ ◕_◕ ༽つ AMENO ༼ つ ◕_◕ ༽つ༼ つ ◕_◕ ༽つ AMENO ༼ つ ◕_◕ ༽つ";
                }
                else
                {
                    flag = 0;
                }
            }
            else
            {
                flag = 0;
            }

            //Check if a valid response was recieved and if it wasn't from the bot
            if (flag == 1 && responseObject.sender_id != botSenderId && responseObject.group_id == groupId)
            {
                BotMsg post;
                if (hasAttachments)
                {
                    post = new BotPictureMsg(attachments);
                }
                else
                {
                    post = new BotTextMsg(sndMsg);
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
            using (StreamReader r = new StreamReader(rektLoc))
            {
                string json = r.ReadToEnd();
                string[] rekt = JsonConvert.DeserializeObject<string[]>(json);
                Random rand = new Random();
                return rekt[(int)rand.Next(0, 84)];
            }
        }

        /// <summary>
        /// Create response message for "told"
        /// </summary>
        /// <returns></returns>
        public static string getTold()
        {
            using (StreamReader r = new StreamReader(toldLoc)) 
            {
                string json = r.ReadToEnd();
                string[] told = JsonConvert.DeserializeObject<string[]>(json);
                Random rand = new Random();
                return told[(int)rand.Next(0, 181)];
            }
        }

        public static string tooSpooky()
        {
            Random r = new Random();
            int spookNum = r.Next(2, 98);
            return string.Format("{0}spooky{1}me", spookNum, spookNum + 2);
        }

        public static string rollDice(string name)
        {
            var player = (sniperinos.SingleOrDefault(sniperino => name == sniperino.name));
            
            //This guy is playing sniperino!
            if (player != null)
            {
                Random r = new Random();
                int roll = r.Next(1, 100);
                int challenge = player.challenge;
                sniperinos.Remove(player);

                if (roll > challenge)
                {
                    return String.Format("{0} rolls a {1}, beating out the challenge of {2}. The donger thanks him ʕ༼◕  ౪  ◕✿༽ʔ", name, roll, challenge);
                }
                else if (roll == challenge)
                {
                    return String.Format("{0} rolls a {1}, tieing the challenge of {2}. The donger...is forced to watch Forsen's stream ლ(ಥ Д ಥ )ლ", name, roll, challenge);
                }
                else
                {
                    return String.Format("{0} rolls a {1}, losing the challenge of {2}! RIP in pepperonis, donger. (⊃✖  〰 ✖)⊃", name, roll, challenge);
                }
            }
            //This guy is just rolling for the lols
            else
            {
                Random r = new Random();
                return String.Format("{0} rolls a {1}!", name, r.Next(1, 100).ToString());
            }
        }

        public static string playSniperino(string name)
        {
            var player = (sniperinos.SingleOrDefault(sniperino => name == sniperino.name));

            if (player == null)
            {
                Random r = new Random();
                int challenge = r.Next(1, 100);
                sniperinos.Add(new Sniperino(name, challenge));
                return String.Format("ヽ༼ຈل͜ຈ༽_•︻ ┻̿═━一 o͡͡͡╮༼ • ʖ̯ • ༽╭o͡͡͡ {0}, roll higher than a {1} or the donger gets it!", name, challenge);
            }
            else
            {
                return "ヽ༼ຈل͜ຈ༽_•︻ ┻̿═━一 You're already playing sniperinos! I oughtta sniperino YOU!";
            }
        }
    }
}
