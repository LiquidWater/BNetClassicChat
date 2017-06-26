using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BNetClassicChat_API;
using WebSocketSharp;

namespace BNetClassicChat_CmdLine
{
    class BNetClassicChat_CmdLine
    {
        static void Main(string[] args)
        {
             string apiKey = File.ReadAllLines("Config/APIKey.txt")[0];

            BNetClassicChatClient client = new BNetClassicChatClient(apiKey);

            client.Connect();
            client.Disconnect();

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
