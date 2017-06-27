using System;
using System.IO;
using BNetClassicChat_API;

namespace BNetClassicChat_CmdLine
{
    class BNetClassicChat_CmdLine
    {
        static void Main(string[] args)
        {
             string apiKey = File.ReadAllLines("Config/APIKey.txt")[0];

            BNetClassicChatClient client = new BNetClassicChatClient(apiKey);

            client.Connect();
            client.OnChannelJoin += (ob, e) => {
                Console.WriteLine("Joined channel " + e.ChannelName);
            };

            //client.Disconnect();

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
