namespace BNetClassicChat_API.Resources
{
    internal static class Constants
    {
        public static string TargetURL = "wss://connect-bot.classic.blizzard.com/v1/rpc/chat";

        public enum ErrorCode
        {
            SUCCESS = 0,
            NOTCONNECTED = 1,
            BADREQUEST = 2,
            REQUESTTIMEOUT = 3,
            HITRATELIMIT = 8
        };

        //Defined in spec sheet, but seemingly unused?
        public enum AreaCode
        {
            AREA1 = 8,
            AREA2 = 6
        }
    }
}
