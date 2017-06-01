using System;
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
            //BNetClassicChatClient client = new BNetClassicChatClient("blargh");
            using (var ws = new WebSocket("wss://connect-bot.classic.blizzard.com/v1/rpc/chat"))
            {
                ws.OnOpen += (sender, e) => { Console.WriteLine("Opened!"); };
                ws.OnError += (sender, e) => { throw e.Exception; };
                ws.Connect();
            }
            Console.ReadLine();
        }
    }
}
