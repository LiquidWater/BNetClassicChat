using System;
using System.Collections.Generic;
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
        private ClientWebSocket socket = new ClientWebSocket();
        private int requestid = 0;
        private CancellationTokenSource sourcetoken = new CancellationTokenSource();
        private CancellationToken passaroundtoken;
        private ArraySegment<byte> inputbuffer;

        public BNetClassicChatClient(string apikey = null)
        {
            APIKey = apikey ?? throw new ArgumentException();
            passaroundtoken = sourcetoken.Token;
            inputbuffer = ClientWebSocket.CreateClientBuffer(1024,1024);
            Connect().Wait();
        }

        private async Task Connect()
        {
            //Step 1: Begin C# websocket connection
            await socket.ConnectAsync(Constants.TargetURI, passaroundtoken);

            //Step 2: Authenticate with server
            RequestResponseModel authrequest = new RequestResponseModel();
            authrequest.Command = "Botapiauth.AuthenticateRequest";
            authrequest.RequestId = requestid++;
            authrequest.Payload = new Dictionary<string, string>{{ "api_key", APIKey }};

            string serializedobject = JsonConvert.SerializeObject(authrequest);
            ArraySegment<byte> outputbuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(serializedobject));

            await socket.SendAsync(outputbuffer, WebSocketMessageType.Text, false, passaroundtoken);
            await socket.ReceiveAsync(inputbuffer, passaroundtoken);

            //If the socket is suddenly closed, probably bad API key
            if (socket.State != WebSocketState.Open){
                throw new WebSocketException("Socket no longer open with state " + socket.State);
            }

            string response = Encoding.UTF8.GetString(inputbuffer.Array);
            RequestResponseModel authresponse = JsonConvert.DeserializeObject<RequestResponseModel>(response);

            if (authresponse.Status.area != 0 || authresponse.Status.code != 0)
                throw new Exception("Response error. area: " + authresponse.Status.area + " code: " + authresponse.Status.code);

            //Step 3: Connect to chat
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
