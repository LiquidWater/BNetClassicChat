/*
    UserJoinArgs.cs: EventArgs for the OnUserJoin event

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

        internal UserJoinArgs(ulong uid, string tn, string f1, string f2, string pid,
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
        }

        private FlagCode _stringtoflagcode_(string s)
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

        public FlagCode Flag1
        {
            get { return _stringtoflagcode_(Flag1AsString); }
        }

        public FlagCode Flag2
        {
            get { return _stringtoflagcode_(Flag2AsString); }
        }

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
