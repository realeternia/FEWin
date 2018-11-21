using System;
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
        [FieldIndex(Index = 12)] public InfoRecord InfoRecord;
        [FieldIndex(Index = 13)] public InfoGismo InfoGismo;
        [FieldIndex(Index = 14)] public InfoWorld InfoWorld;

        public Profile()
        {
            InfoBasic = new InfoBasic();
            InfoBag = new InfoBag();
            InfoDungeon = new InfoDungeon();
            InfoRecord = new InfoRecord();
            InfoGismo = new InfoGismo();
            InfoWorld = new InfoWorld();
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
        }

        public void OnLogout()
        {
            InfoBasic.LastLoginTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
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

        public void OnDie()
        {
            InfoBasic.HealthPoint = 50;
            InfoBasic.MentalPoint = 50;
            InfoBag.SubResource(GameResourceType.Gold, (uint)Math.Ceiling((double)InfoBag.Resource.Gold/5));

            InfoRecord.AddRecordById(RecordInfoConfig.Indexer.TotalDie, 1);
        }
    }
}
