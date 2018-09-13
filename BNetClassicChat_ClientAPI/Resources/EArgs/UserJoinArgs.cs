using System;

namespace BNetClassicChat_ClientAPI.Resources.EArgs
{
    public class UserJoinArgs : EventArgs
    {
        public enum FlagCode
        {
            FLAG_UNKNOWN = 0,
            FLAG_ADMIN = 1,
            FLAG_MODERATOR = 2,
            FLAG_SPEAKER = 3,
            FLAG_MUTEGLOBAL = 4,
            FLAG_MUTEWHISPER = 5
        };

        public struct UserJoinArgsBundle
        {
            public ulong UserID;
            public string ToonName, ProgramID, Rate, Rank, Wins, Flag1AsString, Flag2AsString;
            public FlagCode Flag1, Flag2;
        }

        internal UserJoinArgs (ulong uid, string tn, string f1, string f2, string pid,
            string r1, string r2, string w)
        {
            UserId = uid;

            ToonName = tn;
            ProgramId = pid;
            Rate = r1;
            Rank = r2;
            Wins = w;

            Flag1AsString = f1;
            Flag2AsString = f2;
            Flag1 = _stringtoflagcode_(f1);
            Flag2 = _stringtoflagcode_(f2);
        }

        private FlagCode _stringtoflagcode_ (string s)
        {
            if (string.IsNullOrEmpty(s))
                return FlagCode.FLAG_UNKNOWN;
            switch (s.ToLower())
            {
                case "admin":
                    return FlagCode.FLAG_ADMIN;
                case "moderator":
                    return FlagCode.FLAG_MODERATOR;
                case "speaker":
                    return FlagCode.FLAG_SPEAKER;
                case "muteglobal":
                    return FlagCode.FLAG_MUTEGLOBAL;
                case "mutewhisper":
                    return FlagCode.FLAG_MUTEWHISPER;
                default:
                    return FlagCode.FLAG_UNKNOWN;
            }
        }

        public ulong UserId { get; }

        public string ToonName { get; }

        public string Flag1AsString { get; }

        public string Flag2AsString { get; }

        public FlagCode Flag1 { get; }

        public FlagCode Flag2 { get; }

        public string ProgramId { get; }

        public string Rate { get; }

        public string Rank { get; }

        public string Wins { get; }

        public UserJoinArgsBundle ArgsBundle
        {
            get
            {
                UserJoinArgsBundle bundle;
                bundle.UserID = UserId;
                bundle.ToonName = ToonName;
                bundle.Flag1AsString = Flag1AsString;
                bundle.Flag2AsString = Flag2AsString;
                bundle.Flag1 = Flag1;
                bundle.Flag2 = Flag2;
                bundle.ProgramID = ProgramId;
                bundle.Rate = Rate;
                bundle.Rank = Rank;
                bundle.Wins = Wins;
                return bundle;
            }
        }
    }
}
