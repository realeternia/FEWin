using System;
using System.Collections.Generic;
using ConfigDatas;
using FEGame.Controller.World;
using FEGame.Core;
using FEGame.Rpc;
using NarlonLib.Tools;

namespace FEGame.DataType.User
{
    public class Profile
    {
        [FieldIndex(Index = 1)] public int Pid;
        [FieldIndex(Index = 2)] public string Name; //玩家角色名
        [FieldIndex(Index = 3)] public InfoBasic InfoBasic;
        [FieldIndex(Index = 4)] public InfoBag InfoBag;
        [FieldIndex(Index = 6)] public InfoDungeon InfoDungeon;
        [FieldIndex(Index = 7)] public InfoHero InfoHero;
        [FieldIndex(Index = 12)] public InfoRecord InfoRecord;
        [FieldIndex(Index = 13)] public InfoGismo InfoGismo;
        [FieldIndex(Index = 14)] public InfoWorld InfoWorld;

        private List<IUserInfoSub> itemSubList;

        public Profile()
        {
            itemSubList = new List<IUserInfoSub>();
            itemSubList.Add(InfoBasic = new InfoBasic());
            itemSubList.Add(InfoBag = new InfoBag());
            itemSubList.Add(InfoDungeon = new InfoDungeon());
            itemSubList.Add(InfoHero = new InfoHero());
            itemSubList.Add(InfoRecord = new InfoRecord());
            itemSubList.Add(InfoGismo = new InfoGismo());
            itemSubList.Add(InfoWorld = new InfoWorld());
        }

        public void OnCreate(string name, uint dna, int headId)
        {
            Name = name;
            InfoBasic.Dna = dna;
            InfoBasic.Head = headId;
            InfoBasic.Level = 1;
            InfoBasic.MapId = 13010001;
            InfoBasic.Position = 1001;
            InfoBasic.HealthPoint = 100;
            InfoBasic.MentalPoint = 100;
            InfoBasic.FoodPoint = 100;
            InfoBag.BagCount = GameConstants.BagInitCount;
        }

        public void OnLogin()
        {
            if (TimeManager.IsDifferDay(InfoBasic.LastLoginTime, TimeTool.DateTimeToUnixTime(DateTime.Now)))
                OnNewDay();
            InfoBasic.LastLoginTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
            foreach (var userInfoSub in itemSubList)
                userInfoSub.OnLogin();
        }

        public void OnLogout()
        {
            InfoBasic.LastLoginTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
            foreach (var userInfoSub in itemSubList)
                userInfoSub.OnLogout();
        }

        public void OnSwitchScene(bool isWarp)
        {
            if(isWarp)
                TalePlayer.Save();//每次切场景存个档
        }

        public void OnNewDay()
        {
            InfoBasic.LastLoginTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
        }
    }
}
