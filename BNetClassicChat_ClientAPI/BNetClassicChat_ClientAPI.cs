﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using BNetClassicChat_ClientAPI.Resources;
using BNetClassicChat_ClientAPI.Resources.EArgs;
using BNetClassicChat_ClientAPI.Resources.Models;
using Newtonsoft.Json;
using WebSocketSharp;

namespace BNetClassicChat_ClientAPI
{
    public class BNetClassicChat_Client
    {
        #region PrivateFields
        private Object mutex = new Object();
        private bool isConnected = false, isReady = false;
        private int requestID = 0;
        private string apiKey;
        private WebSocket socket = new WebSocket(Constants.TargetURL, "json");
        //TODO: Maybe use a more futureproof method of parsing instead of dict to func mapping
        private Dictionary<string, Action<RequestResponseModel>> msgHandlers;
        #endregion

        #region InternalMessageHandlers
        #region ConnectionHandshakeHandlers
        private void _onauthresponse_(RequestResponseModel msg)
        {
            //Step 2: Once auth accept response is received, attempt to connect to chat
            Debug.WriteLine("[RESPONSE]Authenticated! Attempting to enter chat...");

            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.ConnectRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++)
            };
            socket.SendAsync(JsonConvert.SerializeObject(request), null);
        }

        private void _onchatconnect_(RequestResponseModel msg)
        {
            //Step 3: Recieving this response means login and connect is successful
            lock (mutex)
            {
                isReady = true;
            }
            string channelname = (string)msg.Payload["channel"];
            ChannelJoinArgs c = new ChannelJoinArgs(channelname);
            OnChannelJoin?.BeginInvoke(this, c, null, null);

            Debug.WriteLine("[EVENT]Entered channel: " + channelname);
        }
        #endregion
        #region RequestResponses
        //TODO: Handle potential errors for these responses
        private void _onchatconnectresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Chat Connect");
        }

        private void _onchatdisconnect_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Disconnect");
        }

        private void _onchatsendmessageresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Message");
        }

        private void _onchatsendwhisperresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Whisper");
        }

        private void _onbanuserresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Ban user");
        }
        
        private void _onunbanuserresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Unban user");
        }

        private void _onsendemoteresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Send Emote");
        }

        private void _onkickuserresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Kick user");
        }

        private void _onsetmoderatorresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Send Set Moderator");
        }
        #endregion
        #region ImportantAsyncEvents
        private void _onchatmessageevent_(RequestResponseModel msg)
        {
            ulong user = Convert.ToUInt64(msg.Payload["user_id"]);
            string message = (string)msg.Payload["message"];
            string type = (string)msg.Payload["type"];
            ChatMessageArgs args = new ChatMessageArgs(user, message, type);
            OnChatMessage?.BeginInvoke(this, args, null, null);

            Debug.WriteLine("[EVENT]Chat message [" + type + "] UID " + user + ": " + message);
        }

        private void _onuserupdateevent_(RequestResponseModel msg)
        {
            //API responses here are inconsistent with spec document
            ulong user = Convert.ToUInt64(msg.Payload["user_id"]);
            string toonname = (string)msg.Payload["toon_name"];
            //TODO: finish flags and attributes
            /*
            string flag1 = (string)(msg.Payload["flags"]);
            string flag2 = (string)(msg.Payload["flags"]);

            */

            UserJoinArgs args = new UserJoinArgs(user, toonname, "temp", "temp", "temp", "temp", "temp", "temp");
            OnUserJoin?.BeginInvoke(this, args, null, null);

            Debug.WriteLine("[EVENT]User joined: " + user + ": " + toonname);
        }

        private void _onuserleaveevent_(RequestResponseModel msg)
        {
            ulong user = Convert.ToUInt64(msg.Payload["user_id"]);
            UserLeaveArgs args = new UserLeaveArgs(user);
            OnUserLeave?.BeginInvoke(this, args, null, null);

            Debug.WriteLine("[EVENT]User left: " + user);
        }
        #endregion
        #endregion

        #region PublicMethodsAndVariables
        //Subscribers must handle events in order to recieve messages
        public event EventHandler<ChannelJoinArgs> OnChannelJoin; //Connected when this event fires
        public event EventHandler<ChatMessageArgs> OnChatMessage;
        public event EventHandler<UserJoinArgs> OnUserJoin;
        public event EventHandler<UserLeaveArgs> OnUserLeave;

        //TODO: Notify subscribers when ready, and notify when disconnected

        public BNetClassicChat_Client(string apikey)
        {
            //Basic input sanitation
            if (apikey != null)
                apiKey = apikey;
            else
                throw new ArgumentNullException("apiKey");

            //Initializing commands to function mappings
            msgHandlers = new Dictionary<string, Action<RequestResponseModel>>()
            {
                //Handshake and initialization related responses
                {"Botapiauth.AuthenticateResponse", _onauthresponse_},
                {"Botapichat.ConnectResponse", _onchatconnectresponse_},
                {"Botapichat.DisconnectResponse", _onchatdisconnect_}, //Not sure if necessary?

                //Async responses loosely connected to handshake/init
                {"Botapichat.ConnectEventRequest", _onchatconnect_},
                {"Botapichat.DisconnectEventRequest", _onchatdisconnect_}, //Not sure if necessary?

                //General responses when server acknowledges a request
                {"Botapichat.SendMessageResponse", _onchatsendmessageresponse_},
                {"Botapichat.SendWhisperResponse", _onchatsendwhisperresponse_},
                {"Botapichat.BanUserResponse", _onbanuserresponse_},
                {"Botapichat.UnbanUserResponse", _onunbanuserresponse_},
                {"Botapichat.SendEmoteResponse", _onsendemoteresponse_},
                {"Botapichat.KickUserResponse", _onkickuserresponse_},
                {"Botapichat.SendSetModeratorResponse", _onsetmoderatorresponse_},

                //Server events that require client action
                {"Botapichat.MessageEventRequest", _onchatmessageevent_},
                {"Botapichat.UserUpdateEventRequest", _onuserupdateevent_},
                {"Botapichat.UserLeaveEventRequest", _onuserleaveevent_}
            };

            //Defining socket behaviour for listening
            socket.OnOpen += (sender, args) =>
            {
                //Step 1: Authenticate with server using API key
                Debug.WriteLine("[SOCKET]Connected! Attempting to authenticate...");

                RequestResponseModel request = new RequestResponseModel()
                {
                    Command = "Botapiauth.AuthenticateRequest",
                    RequestId = Interlocked.Exchange(ref requestID, requestID++),
                    Payload = new Dictionary<string, object>()
                    {
                        {"api_key", apiKey }
                    }
                };
                socket.SendAsync(JsonConvert.SerializeObject(request), null);
                //Continued in _onauthresponse_()
            };

            socket.OnMessage += (sender, args) =>
            {
                RequestResponseModel msg = JsonConvert.DeserializeObject<RequestResponseModel>(args.Data);
                try
                {
                   msgHandlers[msg.Command].BeginInvoke(msg, null, null);
                }
                catch (KeyNotFoundException)
                {
                    Debug.WriteLine("[ERROR]Command " + msg.Command + " not recognized!");
                    Debug.WriteLine("[ERROR]Message payload: " + args.Data);
                }
            };

            socket.OnClose += (sender, args) =>
            {
                lock (mutex)
                {
                    isConnected = false;
                    isReady = false;
                    Debug.WriteLine("[SOCKET]Disconnected with code " + args.Code + ". Reason: " + args.Reason);
                }
            };

            socket.OnError += (sender, args) =>
            {
                Debug.WriteLine("[ERROR] " + args.Message);
                throw args.Exception;
            };
        }

        //Functions for sending data to BNet
        public void Connect()
        {
            lock (mutex)
            {
                if (!isConnected)
                {
                    socket.ConnectAsync();
                    isConnected = true;
                }
                else
                    throw new InvalidOperationException("Already connected");
            }
        }

        public void ConnectAsync()
        {
            Action dummyaction = Connect;
            dummyaction.BeginInvoke(null, null);
        }

        public void Disconnect()
        {
            //TODO: Use the API disconnect call instead of simply closing the socket
            lock (mutex)
            {
                if (isConnected)
                {
                    isConnected = false;
                    isReady = false;
                    socket.CloseAsync(CloseStatusCode.Normal);
                }
                else
                    throw new InvalidOperationException("Not connected");
            }
        }

        public void DisconnectAsync()
        {
            Action dummyaction = Disconnect;
            dummyaction.BeginInvoke(null, null);
        }

        public void SendMessage(string msg)
        {
            ActiveConnectionCheck();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.SendMessageRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++),
                Payload = new Dictionary<string, object>()
                {
                    {"message", msg }
                }
            };
            socket.SendAsync(JsonConvert.SerializeObject(request), null);

            Debug.WriteLine("[REQUEST]Send Message: " + msg);
        }

        public void SendMessageAsync(string msg)
        {
            Action<string> dummyaction = SendMessage;
            dummyaction.BeginInvoke(msg, null, null);
        }

        public void SendWhisper(string msg, ulong userid)
        {
            ActiveConnectionCheck();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.SendWhisperRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++),
                Payload = new Dictionary<string, object>()
                {
                    {"message", msg },
                    {"user_id", userid }
                }
            };
            socket.SendAsync(JsonConvert.SerializeObject(request), null);

            Debug.WriteLine("[REQUEST]Send Whisper: " + userid + ": " + msg);
        }

        public void SendWhisperAsync(string msg, ulong userid)
        {
            Action<string, ulong> dummyaction = SendWhisper;
            dummyaction.BeginInvoke(msg, userid, null, null);
        }

        public void BanUser(ulong userid)
        {
            ActiveConnectionCheck();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.BanUserRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++),
                Payload = new Dictionary<string, object>()
                {
                    {"user_id", userid}
                }
            };
            socket.SendAsync(JsonConvert.SerializeObject(request), null);

            Debug.WriteLine("[REQUEST]Ban user: " + userid);
        }

        public void BanUserAsync(ulong userid)
        {
            Action<ulong> dummyaction = BanUser;
            dummyaction.BeginInvoke(userid, null, null);
        }

        public void UnbanUser(string toonname)
        {
            ActiveConnectionCheck();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.UnbanUserRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++),
                Payload = new Dictionary<string, object>()
                {
                    //For some reason the api spec takes toonname instead of userid here??
                    {"toon_name", toonname}
                }
            };
            socket.SendAsync(JsonConvert.SerializeObject(request), null);

            Debug.WriteLine("[REQUEST]Unban user: " + toonname);
        }

        public void UnbanUserAsync(string toonname)
        {
            Action<string> dummyaction = UnbanUser;
            dummyaction.BeginInvoke(toonname, null, null);
        }

        public void SendEmote(string emotemsg)
        {
            ActiveConnectionCheck();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.SendEmoteRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++),
                Payload = new Dictionary<string, object>()
                {
                    {"message", emotemsg }
                }
            };
            socket.SendAsync(JsonConvert.SerializeObject(request), null);

            Debug.WriteLine("[REQUEST]Send emote: " + emotemsg);
        }

        public void SendEmoteAsync(string emotemsg)
        {
            Action<string> dummyaction = SendEmote;
            dummyaction.BeginInvoke(emotemsg, null, null);
        }

        public void KickUser(ulong userid)
        {
            ActiveConnectionCheck();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.KickUserRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++),
                Payload = new Dictionary<string, object>()
                {
                    {"user_id", userid}
                }
            };
            socket.SendAsync(JsonConvert.SerializeObject(request), null);

            Debug.WriteLine("[REQUEST]Kick user: " + userid);
        }

        public void KickUserAsync(ulong userid)
        {
            Action<ulong> dummyaction = KickUser;
            dummyaction.BeginInvoke(userid, null, null);
        }

        public void SetModerator(ulong userid)
        {
            ActiveConnectionCheck();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.SendSetModeratorRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++),
                Payload = new Dictionary<string, object>()
                {
                    {"user_id", userid}
                }
            };
            socket.SendAsync(JsonConvert.SerializeObject(request), null);

            Debug.WriteLine("[REQUEST]Set moderator: " + userid);
        }

        public void SetModeratorAsync(ulong userid)
        {
            Action<ulong> dummyaction = SetModerator;
            dummyaction.BeginInvoke(userid, null, null);
        }
        #endregion

        #region PrivateHelpers
        private void ActiveConnectionCheck()
        {
            lock (mutex)
            {
                if (!isConnected || !isReady)
                    throw new InvalidOperationException("Websocket not connected or ready");
            }
        }
        #endregion
    }
}
