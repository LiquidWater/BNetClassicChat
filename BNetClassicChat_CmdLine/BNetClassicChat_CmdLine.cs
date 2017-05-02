using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BNetClassicChat_API;

namespace BNetClassicChat_CmdLine
{
    class BNetClassicChat_CmdLine
    {
        static void Main(string[] args)
        {
            BNetClassicChatClient client = new BNetClassicChatClient("blargh");
            client.Authenticate();
            client.Connect();
        }
    }
}
