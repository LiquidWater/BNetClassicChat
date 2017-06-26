using System;

namespace BNetClassicChat_API.Resources.EArgs
{
    public class UserLeaveArgs : EventArgs
    {
        private string userid;

        internal UserLeaveArgs(string u)
        {
            userid = u;
        }

        public string UserId
        {
            get{ return userid; }
        }
    }
}
