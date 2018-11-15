/*
    BNetClassicChat_CmdLine.cs: Sample program used to demonstrate use of the API

    Copyright (C) 2018 LiquidWater
    https://github.com/Liquidwater

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using BNetClassicChat_ClientAPI;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace BNetClassicChat_CmdLine
{
    //Demo program showcasing how to use the API wrapper.
    public class BNetClassicChat_CmdLine
    {
        public static void Main(string[] args)
        {
            //Subscriber keeps track of userid to username and other info mapping
            ConcurrentDictionary<ulong, string> idtoname = new ConcurrentDictionary<ulong, string>();

            //Blizz API key required to connect
            string apiKey = File.ReadAllLines("Config/APIKey.txt")[0];

            //Instantiate a new instance of the chat client
            BNetClassicChat_Client client = new BNetClassicChat_Client(apiKey);

            //Subscribing to all the events
            client.OnChannelJoin += (ob, e) =>
            {
                //When this event fires, connection is established
                Console.WriteLine("Joined channel " + e.ChannelName);

                //Slowing down with sleep to avoid triggering blizzard's anti spam protection
                Thread.Sleep(1000);
                //Sending messages to the server
                client.SendMessage("test message");
                Thread.Sleep(1000);
                client.SendEmote("test emoted message");
                Thread.Sleep(1000);
                client.SendWhisper("test whisper", 1);
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

            client.OnUserJoin += (ob, e) =>
            {
                try
                {
                    idtoname.TryAdd(e.UserId, e.ToonName);
                    Console.WriteLine("User " + e.ToonName + " joined the channel.");
                }
                catch (Exception) { }
            };

            client.OnUserLeave += (ob, e) =>
            {
                try
                {
                    string temp = idtoname[e.UserId];
                    Console.WriteLine("User " + temp + " has left the channel");
                    idtoname.TryRemove(e.UserId, out temp);
                }
                catch (Exception) { }
            };

            //Calling connect will connect the client to battlenet
            client.Connect();

            Console.WriteLine("Press any key to disconnect");
            Console.ReadKey();
            client.Disconnect();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
