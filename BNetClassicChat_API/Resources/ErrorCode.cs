namespace BNetClassicChat_API.Resources
{
    //Error codes that Blizzard's api returns upon failure. Success code is 0
    internal static class ErrorCode
    {
        public static int SUCCESS = 0;
        public static int NOTCONNECTED = 1;
        public static int BADREQUEST = 2;
        public static int REQUESTTIMEOUT = 5;
        public static int HITRATELIMIT = 8;
    }
}
