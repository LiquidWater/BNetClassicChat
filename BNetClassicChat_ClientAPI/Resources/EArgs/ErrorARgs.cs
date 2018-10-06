using System;

namespace BNetClassicChat_ClientAPI.Resources.EArgs
{
    public class ErrorArgs : EventArgs
    {
        public enum ErrorCode
        {
            ERR_SUCCESS = 0,
            ERR_NOTCONNECTED = 1,
            ERR_BADREQUEST = 2,
            ERR_REQUESTTIMEOUT = 3,
            ERR_HITRATELIMIT = 8
        };

        //Defined in spec sheet, but seemingly unused?
        public enum AreaCode
        {
            AREA_1 = 8,
            AREA_2 = 6
        }

        internal ErrorArgs(ErrorCode e, AreaCode a)
        {
            ACode = a;
            ECode = e;
        }

        public AreaCode ACode { get; }

        public ErrorCode ECode { get; }

        public string ErrAsString {
            get
            {
                switch (ECode)
                {
                    case ErrorCode.ERR_SUCCESS:
                        return "Success";

                    case ErrorCode.ERR_NOTCONNECTED:
                        return "Not connected";

                    case ErrorCode.ERR_BADREQUEST:
                        return "Bad request";

                    case ErrorCode.ERR_HITRATELIMIT:
                        return "Hit rate limit";

                    case ErrorCode.ERR_REQUESTTIMEOUT:
                        return "Request timeout";

                    default:
                        return "Unknown";
                }
            }
        }
    }
}
