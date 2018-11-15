/*
    ChatMessageArgs.cs: EventArgs for the OnChatMessage event

    Copyright (C) 2018 LiquidWater
    https://github.com/Liquidwater

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
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

        internal ChatMessageArgs(ulong uid, string msg, string type)
        {
            UserId = uid;
            Message = msg;
            MessageTypeAsString = type;
        }

        private MessageSource _stringtomessagesource_(string s)
        {
            if (string.IsNullOrEmpty(s))
                return MessageSource.MSG_UNKNOWN;
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

        public ulong UserId { get; }

        public string Message { get; }

        public string MessageTypeAsString { get; }

        public MessageSource MessageType
        {
            get { return _stringtomessagesource_(MessageTypeAsString); }
        }

        public ChatMessageArgsBundle ArgsBundle
        {
            get
            {
                ChatMessageArgsBundle bundle;
                bundle.UserID = UserId;
                bundle.Message = Message;
                bundle.MessageTypeAsString = MessageTypeAsString;
                bundle.MessageType = MessageType;
                return bundle;
            }
        }
    }
}
