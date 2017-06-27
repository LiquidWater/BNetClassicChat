using System;
using System.Diagnostics;
using System.Collections.Generic;
using BNetClassicChat_API.Resources;
using BNetClassicChat_API.Resources.EArgs;
using Newtonsoft.Json;
using WebSocketSharp;
using BNetClassicChat_API.Resources.Models;

namespace BNetClassicChat_API
{
    public class BNetClassicChatClient
    {
        private int requestID = 0;
        private string apiKey;
        private WebSocket socket = new WebSocket(Constants.TargetURL, "json");
        //TODO: Maybe use a more futureproof method of parsing instead of dict to func mapping
        private Dictionary<string, Action<RequestResponseModel>> msgHandlers;

        #region InternalMessageHandlers
        private void _onauthresponse_(RequestResponseModel msg)
        {
            //Step 2: Once auth accept response is received, attempt to connect to chat
            Debug.WriteLine("Authenticated! Attempting to enter chat...");

            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.ConnectRequest",
                RequestId = requestID++
            };
            socket.Send(JsonConvert.SerializeObject(request));
        }

        private void _onchatconnectresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("Server accepted enter chat request!");
        }

        private void _onchatconnect_(RequestResponseModel msg)
        {
            //Step 3: Recieving this response means login and connect is successful
            ChannelJoinArgs c = new ChannelJoinArgs((string)msg.Payload["channel"]);
            OnChannelJoin?.Invoke(this, c);
            Debug.WriteLine("Entered chat!");
        }

        private void _onchatdisconnect_(RequestResponseModel msg)
        {

        }

        private void _onchatsendmessageresponse_(RequestResponseModel msg)
        {

        }

        private void _onchatsendwhisperresponse_(RequestResponseModel msg)
        {

        }

        private void _onchatmessageevent_(RequestResponseModel msg)
        {

        }

        private void _onuserupdateevent_(RequestResponseModel msg)
        {

        }

        private void _onuserleaveevent_(RequestResponseModel msg)
        {

        }
        #endregion

        #region PublicMethodsAndVars
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

            //Initializing commands to function mappings
            msgHandlers = new Dictionary<string, Action<RequestResponseModel>>()
            {
                {"Botapiauth.AuthenticateResponse", _onauthresponse_},
                {"Botapichat.ConnectResponse", _onchatconnectresponse_},

                {"Botapichat.ConnectEventRequest", _onchatconnect_},
                {"Botapichat.DisconnectEventRequest", _onchatdisconnect_},

                {"Botapichat.SendMessageResponse", _onchatsendmessageresponse_},
                {"Botapichat.SendWhisperResponse", _onchatsendwhisperresponse_ },
                {"Botapichat.MessageEventRequest", _onchatmessageevent_ },

                {"Botapichat.UserUpdateEventRequest", _onuserupdateevent_ },
                {"Botapichat.UserLeaveEventRequest", _onuserleaveevent_ }
            };

            //Defining behaviour to comply with bnet protocol
            socket.OnOpen += (sender, args) =>
            {
                //Step 1: Authenticate with server using API key
                Debug.WriteLine("Connected! Attempting to authenticate...");

                RequestResponseModel request = new RequestResponseModel()
                {
                    Command = "Botapiauth.AuthenticateRequest",
                    RequestId = requestID++,
                    Payload = new Dictionary<string, object>()
                    {
                        {"api_key", apiKey }
                    }
                };
                socket.Send(JsonConvert.SerializeObject(request));
                //Continued in _onauthresponse_
            };

            socket.OnMessage += (sender, args) =>
            {
                Debug.WriteLine("Message recieved! " + args.Data);
                RequestResponseModel msg = JsonConvert.DeserializeObject<RequestResponseModel>(args.Data);
                try
                {
                    msgHandlers[msg.Command](msg);
                }
                catch (KeyNotFoundException e)
                {
                    Debug.WriteLine("Command " + msg.Command + " not recognized!");
                }
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
        #endregion
    }
}
