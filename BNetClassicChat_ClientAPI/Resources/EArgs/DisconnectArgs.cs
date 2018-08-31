using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNetClassicChat_ClientAPI.Resources.EArgs
{
    public class DisconnectArgs : EventArgs
    {
        private ushort drop_code;
        private string drop_reason;
        private bool drop_clean;

        internal DisconnectArgs(ushort code, string reason, bool clean)
        {
            drop_code = code;
            drop_reason = reason;
            drop_clean = clean;
        }

        public ushort Code
        {
            get { return drop_code; }
        }

        public string Reason
        {
            get { return drop_reason; }
        }

        public bool WasClean
        {
            get { return drop_clean; }
        }
    }
}
