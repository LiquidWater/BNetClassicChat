using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BNetClassicChat_API.Resources;
using Newtonsoft.Json;
using System.Threading;
using WebSocketSharp;

//Entry point for the API
namespace BNetClassicChat_API
{
    public class BNetClassicChatClient
    {
        private int requestID = 0;
        private string APIKey;
        private WebSocket socket = new WebSocket(Constants.TargetURL, "json");

        //Event handlers for signaling subscribers for incoming messages
        public event EventHandler OnChatMessage;

        public BNetClassicChatClient(string apikey)
        {
            //Basic input sanitation
            if (apikey != null)
                APIKey = apikey;
            else
                throw new ArgumentNullException();

            //Defining behaviour to comply with bnet protocol
            socket.OnOpen += (sender, e) =>
            {
                Console.WriteLine("Connected!");
                var auth = "{\n" +
                    "command: Botapiauth.AuthenticateRequest,\n" +
                    "request_id: " + requestID + ",\n" +
                    "payload:\n" +
                    " {api_key: " + APIKey + "\n}" +
                    "\n}";
                socket.Send(auth);
            };

            socket.OnMessage += (sender, e) =>
            {
                Console.WriteLine("Message recieved! " + e.Data );
 
            };

            socket.OnClose += (sender, e) =>
            {
                Console.WriteLine("Disconnected!");
            };

            socket.OnError += (sender, e) =>
            {
                Console.WriteLine("Error " + e.Message);
                throw e.Exception;
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

        public void SendWhisper()
        {
            return;
        }
    }
}
