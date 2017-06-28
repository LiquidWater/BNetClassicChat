using System;
using System.IO;
using System.Collections.Generic;
using BNetClassicChat_ClientAPI;
using System.Threading;

namespace BNetClassicChat_CmdLine
{
    class BNetClassicChat_CmdLine
    {
        static void Main(string[] args)
        {
            Dictionary<ulong, string> idtoname = new Dictionary<ulong, string>();
            //Blizz API key required to connect
            string apiKey = File.ReadAllLines("Config/APIKey.txt")[0];

            //Instantiate a new instance of the chat client
            BNetClassicChat_Client client = new BNetClassicChat_Client(apiKey);
            
            //Calling connect will connect the client to battlenet
            client.Connect();

            //Subscribing to all the events
            client.OnChannelJoin += (ob, e) => {
                //When this event fires, connection is established
                Console.WriteLine("Joined channel " + e.ChannelName);

                //Sending messages to the server
                client.SendMessage("test message");
                Thread.Sleep(500);
                client.SendWhisper("test whisper", 1);
                Thread.Sleep(500);
            };

            client.OnChatMessage += (ob, e) =>
            {
                try
                {
                    Console.WriteLine("[" + e.MessageTypeAsString + "]" + idtoname[e.UserId] + ": " + e.Message);
                }
                catch (Exception)
                {
                    Console.WriteLine("[" + e.MessageTypeAsString + "]" + e.UserId + ": " + e.Message);
                }
            };

            //Current version of API is inconsistent with use of userids and toonnames, so we have to keep track ourselves
            client.OnUserJoin += (ob, e) =>
            {
                try
                {
                    idtoname.Add(e.UserId, e.ToonName);
                    Console.WriteLine("User " + e.ToonName + " joined the channel.");
                }
                catch (Exception) { }
            };

            client.OnUserLeave += (ob, e) =>
            {
                try
                {
                    Console.WriteLine("User " + idtoname[e.UserId] + " has left the channel");
                    idtoname.Remove(e.UserId);
                }
                catch (Exception) { }
            };

            Console.WriteLine("Press any key to disconnect");
            Console.ReadKey();
            client.Disconnect();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
