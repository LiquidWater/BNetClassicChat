using System;

namespace BNetClassicChat_ClientAPI.Resources.EArgs
{
    public class ChatMessageArgs : EventArgs
    {
        public enum MessageSource
        {
            MSG_UNKNOWN = 0,
            MSG_WHISPER = 1,
            MSG_CHANNEL = 2,
            MSG_SERVERINFO = 3,
            MSG_SERVERERROR = 4,
            MSG_EMOTE = 5
        };

        public struct ChatMessageArgsBundle
        {
            public ulong UserID;
            public string Message, MessageTypeAsString;
            public MessageSource MessageType;
        }

        private ulong userid;
        private string message, strmsgtype;
        private MessageSource msgtype;

        internal ChatMessageArgs(ulong uid, string msg, string type)
        {
            userid = uid;
            message = msg;
            strmsgtype = type;
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
                    return MessageSource.MSG_UNKNOWN;
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

        public string MessageTypeAsString
        {
            get { return strmsgtype; }
        }

        public MessageSource MessageType
        {
            get { return msgtype; }
        }

        public ChatMessageArgsBundle ArgsBundle
        {
            get
            {
                ChatMessageArgsBundle bundle;
                bundle.UserID = userid;
                bundle.Message = message;
                bundle.MessageTypeAsString = strmsgtype;
                bundle.MessageType = msgtype;
                return bundle;
            }
        }
    }
}
