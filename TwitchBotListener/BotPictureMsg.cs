using System.Configuration;
using System.Runtime.Serialization;

namespace TwitchBotListener
{
    [DataContract]
    class BotPictureMsg : BotMsg
    {
        /// <summary>
        /// Bot message attachments
        /// </summary>
        [DataMember(Name = "attachments")]
        public Attachment[] attachments { get; set; }

        /// <summary>
        /// Bot picture message constructor
        /// </summary>
        /// <param name="a"></param>
        public BotPictureMsg(Attachment a)
        {
            attachments = new Attachment[] { a };
        }
    }
}
