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

        private ulong userid;
        private string toonname, programid, rate, rank, wins, strflag1, strflag2;
        private FlagCode flag1, flag2;

        internal UserJoinArgs (ulong uid, string tn, string f1, string f2, string pid,
            string r1, string r2, string w)
        {
            userid = uid;

            toonname = tn;
            programid = pid;
            rate = r1;
            rank = r2;
            wins = w;

            strflag1 = f1;
            strflag2 = f2;
            flag1 = _stringtoflagcode_(f1);
            flag2 = _stringtoflagcode_(f2);
        }

        private FlagCode _stringtoflagcode_ (string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return FlagCode.FLAG_UNKNOWN;
            }
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

        public ulong UserId
        {
            get{ return userid; }
        }

        public string ToonName
        {
            get { return toonname; }
        }

        public string Flag1AsString
        {
            get { return strflag1; }
        }

        public string Flag2AsString
        {
            get { return strflag2; }
        }

        public FlagCode Flag1
        {
            get { return flag1; }
        }

        public FlagCode Flag2
        {
            get { return flag2; }
        }

        public string ProgramId
        {
            get { return programid; }
        }

        public string Rate
        {
            get { return rate; }
        }

        public string Rank
        {
            get { return rank; }
        }

        public string Wins
        {
            get { return wins; }
        }

        public UserJoinArgsBundle ArgsBundle
        {
            get
            {
                UserJoinArgsBundle bundle;
                bundle.UserID = userid;
                bundle.ToonName = toonname;
                bundle.Flag1AsString = strflag1;
                bundle.Flag2AsString = strflag2;
                bundle.Flag1 = flag1;
                bundle.Flag2 = flag2;
                bundle.ProgramID = programid;
                bundle.Rate = rate;
                bundle.Rank = rank;
                bundle.Wins = wins;
                return bundle;
            }
        }
    }
}
