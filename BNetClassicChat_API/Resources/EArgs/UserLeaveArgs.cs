using System;

namespace BNetClassicChat_API.Resources.EArgs
{
    public class UserLeaveArgs : EventArgs
    {
        private ulong userid;

        internal UserLeaveArgs(ulong u)
        {
            userid = u;
        }

        public ulong UserId
        {
            get{ return userid; }
        }
    }
}
