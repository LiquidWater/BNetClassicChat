using System;

namespace BNetClassicChat_API.Resources.EArgs
{
    public class ChatMessageArgs : EventArgs
    {
        public enum MessageSource
        {
            MSG_NOTIMPLEMENTED = 0,
            MSG_WHISPER = 1,
            MSG_CHANNEL = 2,
            MSG_SERVERINFO = 3,
            MSG_SERVERERROR = 4,
            MSG_EMOTE = 5
        };

        private ulong userid;
        private string message;
        private MessageSource msgtype;

        internal ChatMessageArgs(ulong uid, string msg, string type)
        {
            userid = uid;
            message = msg;
            msgtype = _stringtomessagesource_(type);
        }

        private MessageSource _stringtomessagesource_(string s)
        {
            switch (s.ToLower())
            {
                case "whisper":
                    return MessageSource.MSG_WHISPER;
                case "channel":
                    return MessageSource.MSG_CHANNEL;
                case "serverinfo":
                    return MessageSource.MSG_SERVERINFO;
                case "servererror":
                    return MessageSource.MSG_SERVERERROR;
                case "emote":
                    return MessageSource.MSG_EMOTE;
                default:
                    return MessageSource.MSG_NOTIMPLEMENTED;
            }
        }

        public ulong UserId
        {
            get { return userid; }
        }

        public string Message
        {
            get { return message; }
        }

        public MessageSource MessageType
        {
            get { return msgtype; }
        }
    }
}
