using System.Configuration;
using System.Runtime.Serialization;

namespace TwitchBotListener
{
    [DataContract]
    class BotTextMsg : BotMsg
    {
        /// <summary>
        /// Bot Message
        /// </summary>
        [DataMember(Name = "text")]
        public string msg { get; set; }

        /// <summary>
        /// Bot Text message constructor
        /// </summary>
        /// <param name="m"></param>
        public BotTextMsg(string m)
        {
            msg = m;
        }
    }
}
