using System;
using System.Diagnostics;
using BNetClassicChat_API.Resources;
using BNetClassicChat_API.Resources.EArgs;
using Newtonsoft.Json;
using WebSocketSharp;

namespace BNetClassicChat_API
{
    public class BNetClassicChatClient
    {
        private int requestID = 0;
        private string apiKey;
        private WebSocket socket = new WebSocket(Constants.TargetURL, "json");

        //Subscribers must handle events in order to recieve messages
        public event EventHandler<ChannelJoinArgs> OnChannelJoin;
        public event EventHandler<ChatMessageArgs> OnChatMessage;
        public event EventHandler<UserJoinArgs> OnUserJoin;
        public event EventHandler<UserLeaveArgs> OnUserLeave;

        public BNetClassicChatClient(string apikey)
        {
            //Basic input sanitation
            if (apikey != null)
                apiKey = apikey;
            else
                throw new ArgumentNullException();

            //Defining behaviour to comply with bnet protocol
            socket.OnOpen += (sender, args) =>
            {
                //Step 1: Authenticate with server using API key
                Debug.WriteLine("Connected!\n Attempting to authenticate...");

                var auth = "{\n" +
                    "command: Botapiauth.AuthenticateRequest,\n" +
                    "request_id: " + requestID + ",\n" +
                    "payload:\n" +
                    " {api_key: " + apiKey + "\n}" +
                    "\n}";
                socket.Send(auth);

                //Step 2: Once auth accept response is received, attempt to connect to chat
                Debug.WriteLine("Authenticated!\n Attempting to connect to chat...");

                Debug.WriteLine("Connected to chat!");
            };

            socket.OnMessage += (sender, args) =>
            {
                Debug.WriteLine("Message recieved! " + args.Data );
            };

            socket.OnClose += (sender, args) =>
            {
                Debug.WriteLine("Disconnected!");
            };

            socket.OnError += (sender, args) =>
            {
                Debug.WriteLine("Error " + args.Message);
                throw args.Exception;
            };
        }

        public void Connect()
        {
            socket.Connect();
            return;
        }

        public void Disconnect()
        {
            socket.Close(CloseStatusCode.Normal, "Goodbye");
            return;
        }

        public void SendMessage(string msg)
        {
            return;
        }

        public void SendWhisper(string msg, string userid)
        {
            return;
        }

        public void BanUser(string userid)
        {
            return;
        }

        public void UnbanUser(string userid)
        {
            return;
        }

        public void KickUser(string userid)
        {
            return;
        }
    }
}
