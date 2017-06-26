using System;

namespace BNetClassicChat_API.Resources.EArgs
{
    public class ChannelJoinArgs : EventArgs
    {
        private string cname;

        internal ChannelJoinArgs(string n)
        {
            cname = n;
        }

        public string ChannelName
        {
            get{ return cname; }
        }
    }
}
