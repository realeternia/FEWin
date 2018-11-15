﻿namespace FEGame.Datas.User
{
    internal static class UserProfile
    {
        public static string ProfileName { get; set; }  //账号名
        public static Profile Profile { get; set; }

        public static InfoBasic InfoBasic
        {
            get { return Profile.InfoBasic; }
        }

        public static InfoBag InfoBag
        {
            get { return Profile.InfoBag; }
        }
        public static InfoDungeon InfoDungeon
        {
            get { return Profile.InfoDungeon; }
        }

        public static InfoGismo InfoGismo
        {
            get { return Profile.InfoGismo; }
        }

        public static InfoRecord InfoRecord
        {
            get { return Profile.InfoRecord; }
        }

        public static InfoQuest InfoQuest
        {
            get { return Profile.InfoQuest; }
        }

        public static InfoWorld InfoWorld
        {
            get { return Profile.InfoWorld; }
        }
    }
}
