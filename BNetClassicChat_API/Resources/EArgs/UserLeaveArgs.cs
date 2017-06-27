using System;

namespace BNetClassicChat_API.Resources.EArgs
{
    public class UserLeaveArgs : EventArgs
    {
        private UInt64 userid;

        internal UserLeaveArgs(UInt64 u)
        {
            userid = u;
        }

        public UInt64 UserId
        {
            get{ return userid; }
        }
    }
}
