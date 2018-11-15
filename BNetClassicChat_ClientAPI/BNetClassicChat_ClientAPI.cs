using BNetClassicChat_ClientAPI.Resources;
using BNetClassicChat_ClientAPI.Resources.EArgs;
using BNetClassicChat_ClientAPI.Resources.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace BNetClassicChat_ClientAPI
{
    /// <summary>
    /// C# wrapper for Blizzard's CAPI. This wrapper is threadsafe amd provides synchronous and asynchronous methods sending and recieving data.
    /// </summary>
    public class BNetClassicChat_Client : IDisposable
    {
        #region PrivateFields

        private Object mutex = new Object();
        private bool isConnected = false, isReady = false;
        private int requestID = 0;
        private WebSocket socket = new WebSocket(Constants.TargetURL, "json");
        //TODO: Maybe use a more futureproof method of parsing instead of dict to func mapping
        private Dictionary<string, Func<RequestResponseModel, Task>> msgHandlers;

        #endregion PrivateFields

        #region InternalMessageHandlers

        #region ConnectionHandshakeHandlers

        private async Task _onauthresponse_(RequestResponseModel msg)
        {
            //Step 2: Once auth accept response is received, attempt to connect to chat
            Debug.WriteLine("[RESPONSE]Authenticated! Attempting to enter chat...");

            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.ConnectRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++)
            };
            await Task.Run(() => socket.Send(JsonConvert.SerializeObject(request)));
        }

        private async Task _onchatconnectevent_(RequestResponseModel msg)
        {
            //Step 3: Recieving this response means login and connect is successful
            lock (mutex)
            {
                isReady = true;
            }
            string channelname = (string)msg.Payload["channel"];
            ChannelJoinArgs c = new ChannelJoinArgs(channelname);
            Task t = Task.Run(() => OnChannelJoin?.Invoke(this, c));

            Debug.WriteLine($"[EVENT]Entered channel: {channelname}");
            await t;
        }

        #endregion ConnectionHandshakeHandlers

        #region RequestResponses

        //As of Alphav3, API does not fill in error codes
        private async Task _onchatconnectresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Chat Connect");
            await Task.Run(() => __RequestResponseHelper__(msg));
        }

        private async Task _onchatdisconnectresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Disconnect");
            await Task.Run(() => __RequestResponseHelper__(msg));
        }

        private async Task _onchatsendmessageresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Message");
            await Task.Run(() => __RequestResponseHelper__(msg));
        }

        private async Task _onchatsendwhisperresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Whisper");
            await Task.Run(() => __RequestResponseHelper__(msg));
        }

        private async Task _onbanuserresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Ban user");
            await Task.Run(() => __RequestResponseHelper__(msg));
        }

        private async Task _onunbanuserresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Unban user");
            await Task.Run(() => __RequestResponseHelper__(msg));
        }

        private async Task _onsendemoteresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Send Emote");
            await Task.Run(() => __RequestResponseHelper__(msg));
        }

        private async Task _onkickuserresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Kick user");
            await Task.Run(() => __RequestResponseHelper__(msg));
        }

        private async Task _onsetmoderatorresponse_(RequestResponseModel msg)
        {
            Debug.WriteLine("[RESPONSE]Set Moderator");
            await Task.Run(() => __RequestResponseHelper__(msg));
        }

        #endregion RequestResponses

        #region ImportantAsyncEvents

        private async Task _onchatmessageevent_(RequestResponseModel msg)
        {
            ulong user = Convert.ToUInt64(msg.Payload["user_id"]);
            string message = (string)msg.Payload["message"];
            string type = (string)msg.Payload["type"];
            ChatMessageArgs args = new ChatMessageArgs(user, message, type);
            Task t = Task.Run(() => OnChatMessage?.Invoke(this, args));

            Debug.WriteLine($"[EVENT]Chat message [{type}] from UID {user}: {message}");
            await t;
        }

        private async Task _onuserupdateevent_(RequestResponseModel msg)
        {
            ulong user = Convert.ToUInt64(msg.Payload["user_id"]);
            string toonname = (string)msg.Payload["toon_name"];
            UserJoinArgs args;

            //API doesn't reply with a lot of the data here despite being documented in spec doc
            try
            {
                List<string> flags = (List<string>)(msg.Payload["flags"]);
                Dictionary<string, string> attributes = (Dictionary<string, string>)(msg.Payload["attributes"]);

                string flag1 = flags[0];
                string flag2 = flags[1];
                string pid = attributes["ProgramId"];
                string rate = attributes["Rate"];
                string rank = attributes["Rank"];
                string wins = attributes["Wins"];

                args = new UserJoinArgs(user, toonname, flag1, flag2, pid, rate, rank, wins);
            }
            catch (Exception)
            {
                args = new UserJoinArgs(user, toonname, null, null, null, null, null, null);
            }
            Task t = Task.Run(() => OnUserJoin?.Invoke(this, args));

            Debug.WriteLine($"[EVENT]User joined: {user}: {toonname}");
            await t;
        }

        private async Task _onuserleaveevent_(RequestResponseModel msg)
        {
            ulong user = Convert.ToUInt64(msg.Payload["user_id"]);
            UserLeaveArgs args = new UserLeaveArgs(user);
            Task t = Task.Run(() => OnUserLeave?.Invoke(this, args));

            Debug.WriteLine($"[EVENT]User left: {user}");
            await t;
        }

        private async Task _onchatdisconnectevent_(RequestResponseModel msg)
        {
            Task t = Task.Run(() => socket.Close());
            Debug.WriteLine("[EVENT]Disconnected");
            await t;
        }

        #endregion ImportantAsyncEvents

        #endregion InternalMessageHandlers

        #region PublicMethodsAndVariables

        /// <summary>
        /// Called when bot joins a channel.
        /// Since there are no channel switch commands, this event should be considered the "connect success" event.
        /// </summary>
        public event EventHandler<ChannelJoinArgs> OnChannelJoin; //Connected when this event fires

        /// <summary>
        /// Called when a chat message is received.
        /// </summary>
        public event EventHandler<ChatMessageArgs> OnChatMessage;

        /// <summary>
        /// Called when a user joins the channel.
        /// Note that the API does not keep track of users currently in the channel.
        /// </summary>
        public event EventHandler<UserJoinArgs> OnUserJoin;

        /// <summary>
        /// Called when a user leaves the channel.
        /// </summary>
        public event EventHandler<UserLeaveArgs> OnUserLeave;

        /// <summary>
        /// Called when the bot disconnects.
        /// </summary>
        public event EventHandler<DisconnectArgs> OnDisconnect;

        /// <summary>
        /// Called when the API returns an error.
        /// </summary>
        public event EventHandler<ErrorArgs> OnError; //Handling errors not required so far

        //Constructors/Destructors and getters/setters
        /// <summary>
        /// Access the API key.
        /// Doesnt matter if APIKey changes while connected because its only used during connection negotiation.
        /// </summary>
        public string APIKey { get; set; } = null;

        /// <summary>
        /// Creates a new instance of the CAPI client wrapper with an option to specify the API key.
        /// </summary>
        /// <param name="key">API key</param>
        public BNetClassicChat_Client(string key = null)
        {
            APIKey = key;
            __InitializeObjects__();
        }

        ~BNetClassicChat_Client()
        {
            Dispose();
        }

        //Functions for sending data to BNet
        /// <summary>
        /// Initiate a connection to battle.net.
        /// Calling this function begins the handshake process, and does not necessarily mean a connection is successful.
        /// The <see cref="OnChannelJoin"/> event is raised when it is successful.
        /// </summary>
        /// <exception cref="InvalidOperationException">No api key was provided</exception>
        /// <exception cref="InvalidOperationException">Trying to connect when already connected</exception>
        public void Connect()
        {
            if (APIKey.IsNullOrEmpty())
                throw new InvalidOperationException("No api key specified!");
            lock (mutex)
            {
                if (!isConnected)
                {
                    socket.Connect();
                    isConnected = true;
                }
                else
                    throw new InvalidOperationException("Already connected");
            }
        }

        /// <summary>
        /// Async version of Connect().
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            await Task.Run(() => Connect());
        }

        /// <summary>
        /// Disconnect from battle.net.
        /// Should only be called after a successful connection has been established (ie. OnChannelJoin is raised).
        /// Calling this function begins the disconnect handshake process, and does not necessarily mean the client is disconnected.
        /// The <see cref="OnDisconnect"/> event is raised when disconnected.
        /// </summary>
        /// <exception cref="InvalidOperationException">Attempting to disconnect before connecting</exception>
        public void Disconnect()
        {
            __ActiveConnectionCheck__();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.DisconnectRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++)
            };
            socket.Send(JsonConvert.SerializeObject(request));

            Debug.WriteLine("[REQUEST]Disconnect");
        }

        /// <summary>
        /// Async version of Disconnect().
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {
            await Task.Run(() => Disconnect());
        }

        /// <summary>
        /// Sends a message to the current channel.
        /// </summary>
        /// <param name="msg">Message to send</param>
        public void SendMessage(string msg)
        {
            __ActiveConnectionCheck__();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.SendMessageRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++),
                Payload = new Dictionary<string, object>()
                {
                    {"message", msg }
                }
            };
            socket.Send(JsonConvert.SerializeObject(request));

            Debug.WriteLine($"[REQUEST]Send Message: {msg}");
        }

        /// <summary>
        /// Async version of SendMessage.
        /// </summary>
        /// <param name="msg">Message to send</param>
        /// <returns></returns>
        public async Task SendMessageAsync(string msg)
        {
            await Task.Run(() => SendMessage(msg));
        }

        /// <summary>
        /// Sends a whisper message to a user.
        /// </summary>
        /// <param name="msg">Mesage to send</param>
        /// <param name="userid">User to message</param>
        public void SendWhisper(string msg, ulong userid)
        {
            __ActiveConnectionCheck__();
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
            socket.Send(JsonConvert.SerializeObject(request));

            Debug.WriteLine($"[REQUEST]Send Whisper: {userid}: {msg}");
        }

        /// <summary>
        /// Async version of SendWhisper().
        /// </summary>
        /// <param name="msg">Mesage to send</param>
        /// <param name="userid">User to message</param>
        /// <returns></returns>
        public async Task SendWhisperAsync(string msg, ulong userid)
        {
            await Task.Run(() => SendWhisper(msg, userid));
        }

        /// <summary>
        /// Bans a user from the channel.
        /// </summary>
        /// <param name="userid">User to ban</param>
        public void BanUser(ulong userid)
        {
            __ActiveConnectionCheck__();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.BanUserRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++),
                Payload = new Dictionary<string, object>()
                {
                    {"user_id", userid}
                }
            };
            socket.Send(JsonConvert.SerializeObject(request));

            Debug.WriteLine($"[REQUEST]Ban user: {userid}");
        }

        /// <summary>
        /// Async version of BanUser().
        /// </summary>
        /// <param name="userid">User to ban</param>
        /// <returns></returns>
        public async Task BanUserAsync(ulong userid)
        {
            await Task.Run(() => BanUser(userid));
        }

        /// <summary>
        /// Unbans a user from the channel.
        /// </summary>
        /// <param name="toonname">User to unban</param>
        public void UnbanUser(string toonname)
        {
            __ActiveConnectionCheck__();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.UnbanUserRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++),
                Payload = new Dictionary<string, object>()
                {
                    {"toon_name", toonname}
                }
            };
            socket.Send(JsonConvert.SerializeObject(request));

            Debug.WriteLine($"[REQUEST]Unban user: {toonname}");
        }

        /// <summary>
        /// Async version of UnbanUser().
        /// </summary>
        /// <param name="toonname">User to unban</param>
        /// <returns></returns>
        public async Task UnbanUserAsync(string toonname)
        {
            await Task.Run(() => UnbanUser(toonname));
        }

        /// <summary>
        /// Sends an emoted message to the channel.
        /// </summary>
        /// <param name="emotemsg">Message to send</param>
        /// <returns></returns>
        public void SendEmote(string emotemsg)
        {
            __ActiveConnectionCheck__();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.SendEmoteRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++),
                Payload = new Dictionary<string, object>()
                {
                    {"message", emotemsg }
                }
            };
            socket.Send(JsonConvert.SerializeObject(request));

            Debug.WriteLine($"[REQUEST]Send emote: {emotemsg}");
        }

        /// <summary>
        /// Async version of SendEmote().
        /// </summary>
        /// <param name="emotemsg">Message to send</param>
        /// <returns></returns>
        public async Task SendEmoteAsync(string emotemsg)
        {
            await Task.Run(() => SendEmote(emotemsg));
        }

        /// <summary>
        /// Kicks a user from the channel.
        /// </summary>
        /// <param name="userid">User to kick</param>
        public void KickUser(ulong userid)
        {
            __ActiveConnectionCheck__();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.KickUserRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++),
                Payload = new Dictionary<string, object>()
                {
                    {"user_id", userid}
                }
            };
            socket.Send(JsonConvert.SerializeObject(request));

            Debug.WriteLine($"[REQUEST]Kick user: {userid}");
        }

        /// <summary>
        /// Async version of KickUser().
        /// </summary>
        /// <param name="userid">User to kick</param>
        /// <returns></returns>
        public async Task KickUserAsync(ulong userid)
        {
            await Task.Run(() => KickUser(userid));
        }

        /// <summary>
        /// Sets a user to channel moderator.
        /// Note that there is currently no way to unmod a user.
        /// </summary>
        /// <param name="userid">User to mod</param>
        public void SetModerator(ulong userid)
        {
            __ActiveConnectionCheck__();
            RequestResponseModel request = new RequestResponseModel()
            {
                Command = "Botapichat.SendSetModeratorRequest",
                RequestId = Interlocked.Exchange(ref requestID, requestID++),
                Payload = new Dictionary<string, object>()
                {
                    {"user_id", userid}
                }
            };
            socket.Send(JsonConvert.SerializeObject(request));

            Debug.WriteLine($"[REQUEST]Set moderator: {userid}");
        }

        /// <summary>
        /// Async version of SetModerator().
        /// </summary>
        /// <param name="userid">User to mod</param>
        /// <returns></returns>
        public async Task SetModeratorAsync(ulong userid)
        {
            await Task.Run(() => SetModerator(userid));
        }

        //Implementing Disposeable interface
        public void Dispose()
        {
            socket.Close();
            ((IDisposable)socket).Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion PublicMethodsAndVariables

        #region PrivateHelpers

        private void __ActiveConnectionCheck__()
        {
            lock (mutex)
            {
                if (!isConnected || !isReady)
                    throw new InvalidOperationException("Websocket not connected or ready");
            }
        }

        private void __InitializeObjects__()
        {
            //Initializing commands to function mappings
            msgHandlers = new Dictionary<string, Func<RequestResponseModel, Task>>()
            {
                //Handshake and initialization related responses
                {"Botapiauth.AuthenticateResponse", _onauthresponse_},
                {"Botapichat.ConnectResponse", _onchatconnectresponse_},
                {"Botapichat.DisconnectResponse", _onchatdisconnectresponse_},

                //Async responses related to connection state
                {"Botapichat.ConnectEventRequest", _onchatconnectevent_},
                {"Botapichat.DisconnectEventRequest", _onchatdisconnectevent_},

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
            socket.OnOpen += async (sender, args) =>
            {
                //Step 1: Authenticate with server using API key
                Debug.WriteLine("[SOCKET]Connected! Attempting to authenticate...");

                RequestResponseModel request = new RequestResponseModel()
                {
                    Command = "Botapiauth.AuthenticateRequest",
                    RequestId = Interlocked.Exchange(ref requestID, requestID++),
                    Payload = new Dictionary<string, object>()
                    {
                        {"api_key", APIKey }
                    }
                };
                await Task.Run (() => socket.Send(JsonConvert.SerializeObject(request)));
                //Continued in _onauthresponse_()
            };

            socket.OnMessage += async (sender, args) =>
            {
                RequestResponseModel msg = JsonConvert.DeserializeObject<RequestResponseModel>(args.Data);
                if (msg.Command.IsNullOrEmpty() || !msgHandlers.ContainsKey(msg.Command))
                {
                    Debug.WriteLine($"[ERROR]Command {msg.Command} not recognized!");
                    Debug.WriteLine($"[ERROR]Message payload: {args.Data}");
                }
                else
                    await msgHandlers[msg.Command].Invoke(msg);
            };

            socket.OnClose += async (sender, args) =>
            {
                lock (mutex)
                {
                    isConnected = false;
                    isReady = false;
                }
                DisconnectArgs dargs = new DisconnectArgs(args.Code, args.Reason, args.WasClean);
                Task t = Task.Run(() => OnDisconnect?.Invoke(this, dargs));

                Debug.WriteLine($"[SOCKET]Disconnected with code {args.Code}. Reason: {args.Reason}");
                await t;
            };

            socket.OnError += (sender, args) =>
            {
                Debug.WriteLine($"[ERROR] {args.Message}");
                if (args.Exception != null)
                    throw args.Exception;
            };
        }

        private void __RequestResponseHelper__(RequestResponseModel msg)
        {
            ErrorArgs eargs = new ErrorArgs(msg.Status?.Code, msg.Status?.Area);
            OnError?.Invoke(this, eargs);
        }

        #endregion PrivateHelpers
    }
}
