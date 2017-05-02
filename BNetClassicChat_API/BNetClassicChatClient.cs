using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using BNetClassicChat_API.Resources;
using Newtonsoft.Json;
using System.Threading;

//Entry point for the API
namespace BNetClassicChat_API
{
    public class BNetClassicChatClient
    {
        private string APIKey;
        private ClientWebSocket socket;
        private int requestid = 0;
        private CancellationTokenSource sourcetoken = new CancellationTokenSource();
        private CancellationToken passaroundtoken;

        public BNetClassicChatClient(string apikey = null)
        {
            if (apikey == null)
            {
                throw new ArgumentException();
            }
            APIKey = apikey;
            passaroundtoken = sourcetoken.Token;
            socket =  new ClientWebSocket();
            socket.ConnectAsync(new Uri("wss://connect-bot.classic.blizzard.com/v1/ rpc/chat"), passaroundtoken);
            Console.WriteLine(socket.State);
        }

        public void Authenticate()
        {
            RequestResponseModel request = new RequestResponseModel();
            request.Command = "Botapiauth.AuthenticateRequest";
            request.RequestId = requestid++;
            request.Payload = new Dictionary<string, string>();
            request.Payload.Add("api_key", APIKey);

            string serializedObject = JsonConvert.SerializeObject(request);
            ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(serializedObject));

            socket.SendAsync(buffer, WebSocketMessageType.Text, false, passaroundtoken);
            socket.ReceiveAsync(buffer, passaroundtoken);
            Console.WriteLine(buffer);
            return;
        }

        public void Connect()
        {
            return;
        }

        public void Disconnect()
        {
            return;
        }

        public void SendMessage(string msg)
        {
            return;
        }

        public void SendWhisper()
        {
            return;
        }
    }
}
