using System;

/*
    DisconnectArgs.cs: EventArgs for the OnDisconnect event

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
namespace BNetClassicChat_ClientAPI.Resources.EArgs
{
    public class DisconnectArgs : EventArgs
    {
        internal DisconnectArgs(ushort code, string reason, bool clean)
        {
            Code = code;
            Reason = reason;
            WasClean = clean;
        }

        public ushort Code { get; }

        public string Reason { get; }

        public bool WasClean { get; }
    }
}
