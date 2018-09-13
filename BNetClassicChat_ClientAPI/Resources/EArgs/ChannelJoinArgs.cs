using System;

namespace BNetClassicChat_ClientAPI.Resources.EArgs
{
    public class ChannelJoinArgs : EventArgs
    {
        internal ChannelJoinArgs(string n)
        {
            ChannelName = n;
        }

        public string ChannelName { get; }
    }
}
