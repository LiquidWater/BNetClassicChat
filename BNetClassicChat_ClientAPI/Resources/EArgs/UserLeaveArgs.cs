using System;

namespace BNetClassicChat_ClientAPI.Resources.EArgs
{
    public class UserLeaveArgs : EventArgs
    {
        internal UserLeaveArgs(ulong u)
        {
            UserId = u;
        }

        public ulong UserId { get; }
    }
}
