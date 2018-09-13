using System;

namespace BNetClassicChat_ClientAPI.Resources.EArgs
{
    public class DisconnectArgs : EventArgs
    {
        internal DisconnectArgs(ushort code, string reason, bool clean)
        {
            Code = code;
            Reason = reason;
            WasClean = clean;
        }

        public ushort Code { get; }

        public string Reason { get; }

        public bool WasClean { get; }
    }
}
