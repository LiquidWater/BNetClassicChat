/*
    ErrorArgs.cs: EventArgs for the OnError event

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

using System;

namespace BNetClassicChat_ClientAPI.Resources.EArgs
{
    public class ErrorArgs : EventArgs
    {
        public enum ErrorCode
        {
            ERR_UNKNOWN = -1,
            ERR_SUCCESS = 0,
            ERR_NOTCONNECTED = 1,
            ERR_BADREQUEST = 2,
            ERR_REQUESTTIMEOUT = 5,
            ERR_HITRATELIMIT = 8
        };

        //Defined in spec sheet, but seemingly unused?
        public enum AreaCode
        {
            AREA_UNKNOWN = -1,
            AREA_1 = 8,
            AREA_2 = 6
        }

        internal ErrorArgs(int? error, int? area)
        {
            if (area.HasValue && Enum.IsDefined(typeof(AreaCode), area))
                ACode = (AreaCode)area;
            else
                ACode = AreaCode.AREA_UNKNOWN;

            if (error.HasValue && Enum.IsDefined(typeof(ErrorCode), error))
                ECode = (ErrorCode)error;
            else
                ECode = ErrorCode.ERR_UNKNOWN;
        }

        public AreaCode ACode { get; }

        public ErrorCode ECode { get; }

        public string ErrAsString
        {
            get
            {
                switch (ECode)
                {
                    case ErrorCode.ERR_UNKNOWN:
                        return "Unknown error code";

                    case ErrorCode.ERR_SUCCESS:
                        return "Success";

                    case ErrorCode.ERR_NOTCONNECTED:
                        return "Not connected";

                    case ErrorCode.ERR_BADREQUEST:
                        return "Bad request";

                    case ErrorCode.ERR_REQUESTTIMEOUT:
                        return "Request timeout";

                    case ErrorCode.ERR_HITRATELIMIT:
                        return "Hit rate limit";

                    default:
                        return "Unknown";
                }
            }
        }
    }
}
