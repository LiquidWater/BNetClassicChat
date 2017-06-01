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
            var lastRequestId = 0;
            var apiKey = "test";
            //BNetClassicChatClient client = new BNetClassicChatClient("blargh");
            using (var ws = new WebSocket("wss://connect-bot.classic.blizzard.com/v1/rpc/chat", "json"))
            {
                ws.OnOpen += (sender, e) =>
                {
                    Console.WriteLine("Opened!");
                    var auth = "{command: Botapiauth.AuthenticateRequest\nrequest_id: " + lastRequestId + "\npayload: {api_key" + apiKey + "\n}\n}";
                    ws.Send(auth);
                };
                ws.OnMessage += (sender, e) =>
                {
                    Console.WriteLine(e.Data);
                };
                ws.OnError += (sender, e) => { throw e.Exception; };
                ws.OnClose += (sender, e) => { Console.WriteLine("Closed!"); };
                ws.Connect();
            }
            Console.ReadLine();
        }
    }
}
