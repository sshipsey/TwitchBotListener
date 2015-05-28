using System.Configuration;
using System.Runtime.Serialization;

namespace TwitchBotListener
{
    /// <summary>
    /// Sent Message structures
    /// </summary>
    [DataContract]
    public class BotMsg
    {
        [DataMember(Name = "bot_id")]
        public string botId = ConfigurationSettings.AppSettings["BOT_ID"];
    }
}
