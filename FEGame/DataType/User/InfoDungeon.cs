﻿using System;
using System.Collections.Generic;
using ConfigDatas;
using FEGame.Core;
using FEGame.DataType.User.Db;
using FEGame.Forms.CMain;
using NarlonLib.Log;

namespace FEGame.DataType.User
{
    public class InfoDungeon : IUserInfoSub
    {
        [FieldIndex(Index = 1)] public int DungeonId; //副本id

        [FieldIndex(Index = 2)] public int Str; //力量
        [FieldIndex(Index = 3)] public int Agi; //敏捷
        [FieldIndex(Index = 4)] public int Intl; //智慧
        [FieldIndex(Index = 5)] public int Perc; //感知
        [FieldIndex(Index = 6)] public int Endu; //耐力

        [FieldIndex(Index = 7)] public List<DbGismoState> EventList; //完成的任务列表，如果0，表示任务失败
        [FieldIndex(Index = 8)] public int StoryId; //故事id
        [FieldIndex(Index = 9)] public int JobId; //故事id

        [FieldIndex(Index = 11)] public int FightWin;
        [FieldIndex(Index = 12)] public int FightLoss;
        [FieldIndex(Index = 13)] public List<IntPair> Items; //副本道具

        [FieldIndex(Index = 22)] public int StrAddon; //力量改变值
        [FieldIndex(Index = 23)] public int AgiAddon; //敏捷改变值
        [FieldIndex(Index = 24)] public int IntlAddon; //智慧改变值
        [FieldIndex(Index = 25)] public int PercAddon; //感知改变值
        [FieldIndex(Index = 26)] public int EnduAddon; //耐力改变值

        public InfoDungeon()
        {
            EventList = new List<DbGismoState>();
            Items = new List<IntPair>();
        }
        void IUserInfoSub.OnLogin()
        {
        }

        void IUserInfoSub.OnLogout()
        {
        }
        public void Enter(int dungeonId) //进入副本需要初始化
        {
            DungeonId = dungeonId;

            EventList = new List<DbGismoState>();
            Items = new List<IntPair>();
            FightWin = 0;
            FightLoss = 0;

            StrAddon = 0;
            AgiAddon = 0;
            IntlAddon = 0;
            PercAddon = 0;
            EnduAddon = 0;

            RecalculateAttr();
        }

        public void Leave()
        {
            DungeonId = 0;
            Items.Clear();
        }

        public int Step { get { return EventList.Count; } }

        public void OnEventEnd(int id, string type)
        {
            UserProfile.InfoRecord.AddRecordById(RecordInfoConfig.Indexer.TotalEvent, 1);
            if (DungeonId > 0)
            {
                EventList.Add(new DbGismoState(id, type));
                UserProfile.InfoGismo.CheckEventList();
                NLog.Debug("OnEventEnd {0} {1}", id, type);
            }
        }

        /// <summary>
        /// 获取一个任务已经完成的次数
        /// </summary>
        public int GetQuestCount(int qid)
        {
            if (DungeonId <= 0)
                return 0;

            return EventList.FindAll(v => v.BaseId == qid).Count;
        }

        public bool CheckQuestCount(int qid, string state, int countNeed, bool needContinue)
        {
            if (EventList.Count == 0)
                return false;

            if (EventList[EventList.Count - 1].BaseId != qid)
                return false;

            int count = 0;
            for (int i = EventList.Count-1; i >=0 ; i--)
            {
                var checkData = EventList[i];
                if (checkData.BaseId == qid)
                {
                    if (!string.IsNullOrEmpty(state) && state != checkData.ResultName)
                        break;

                    count++;
                }
                else
                {
                    if(needContinue)
                        break;
                }
            }

            return count >= countNeed;
        }

        public bool CheckQuestTagCount(string tag, string state, int countNeed, bool needContinue)
        {
            if (EventList.Count == 0)
                return false;

            var lastQuestConfig = ConfigData.GetDungeonGismoConfig(EventList[EventList.Count - 1].BaseId);
            if (lastQuestConfig.FinishSceneQuestTag != tag)
                return false;

            int count = 0;
            for (int i = EventList.Count - 1; i >= 0; i--)
            {
                var checkQuestConfig = ConfigData.GetDungeonGismoConfig(EventList[i].BaseId);
                if (checkQuestConfig.FinishSceneQuestTag == tag)
                {
                    if (!string.IsNullOrEmpty(state) && state != EventList[i].ResultName)
                        break;

                    count++;
                }
                else
                {
                    if (needContinue)
                        break;
                }
            }

            return count >= countNeed;
        }

        public void ChangeAttr(int strC, int agiC, int intlC, int percC, int enduC)
        {
            if (strC != 0)
                StrAddon += strC;
            if (agiC != 0)
                AgiAddon += agiC;
            if (intlC != 0)
                IntlAddon += intlC;
            if (percC != 0)
                PercAddon += percC;
            if (enduC != 0)
                EnduAddon += enduC;

            RecalculateAttr();
        }

        public void RecalculateAttr()
        {
            if (DungeonId <= 0)
                return;

            var dungeonConfig = ConfigData.GetDungeonConfig(DungeonId);
            Str = dungeonConfig.Str;
            Agi = dungeonConfig.Agi;
            Intl = dungeonConfig.Intl;
            Perc = dungeonConfig.Perc;
            Endu = dungeonConfig.Endu;

            Str += StrAddon; //加成属性，一般来自sq
            Agi += AgiAddon;
            Intl += IntlAddon;
            Perc += PercAddon;
            Endu += EnduAddon;

            if (dungeonConfig.Str >= 0)
                Str = Math.Max(1, Str);
            if (dungeonConfig.Agi >= 0)
                Agi = Math.Max(1, Agi);
            if (dungeonConfig.Intl >= 0)
                Intl = Math.Max(1, Intl);
            if (dungeonConfig.Perc >= 0)
                Perc = Math.Max(1, Perc);
            if (dungeonConfig.Endu >= 0)
                Endu = Math.Max(1, Endu);
        }

        public int GetAttrByStr(string type)
        {
            switch (type)
            {
                case "str": return Str;
                case "agi": return Agi;
                case "intl": return Intl;
                case "perc": return Perc;
                case "endu": return Endu;
            }
            return -1;
        }

        public int GetRequireAttrByStr(string type, int biasData)
        {
            var dungeonConfig = ConfigData.GetDungeonConfig(DungeonId);
            switch (type)
            {
                case "str": return dungeonConfig.Str + biasData;
                case "agi": return dungeonConfig.Agi + biasData;
                case "intl": return dungeonConfig.Intl + biasData;
                case "perc": return dungeonConfig.Perc + biasData;
                case "endu": return dungeonConfig.Endu + biasData;
            }
            return 1;
        }

        public int GetDungeonItemCount(int itemId)
        {
            if (DungeonId <= 0)
                return 0;

            foreach (var pickItem in Items)
            {
                if (pickItem.Type == itemId)
                    return pickItem.Value;
            }
            return 0;
        }

        public void AddDungeonItem(int itemId, int count)
        {
            if (DungeonId <= 0)
                return;

            DungeonItemConfig itemConfig = ConfigData.GetDungeonItemConfig(itemId);
            foreach (var pickItem in Items)
            {
                if (pickItem.Type == itemId)
                {
                    pickItem.Value += count;
                    UserProfile.InfoGismo.CheckDungeonItem();
                    MainTipManager.AddTip(string.Format("|获得副本道具-|Lime|{0}||x{1}(总计{2})", itemConfig.Name, count, pickItem.Value), "White");
                    return;
                }
            }
            Items.Add(new IntPair() {Type = itemId, Value = count});
            UserProfile.InfoGismo.CheckDungeonItem();
            MainTipManager.AddTip(string.Format("|获得副本道具-|Lime|{0}||x{1}(总计{2})", itemConfig.Name, count, count), "White");
        }

        public void RemoveDungeonItem(int itemId, int count)
        {
            if (DungeonId <= 0)
                return;

            DungeonItemConfig itemConfig = ConfigData.GetDungeonItemConfig(itemId);
            foreach (var pickItem in Items)
            {
                if (pickItem.Type == itemId)
                {
                    pickItem.Value = Math.Max(0, pickItem.Value - count);
                    MainTipManager.AddTip(string.Format("|扣除副本道具-|Lime|{0}||x{1}(剩余{2})", itemConfig.Name, count, pickItem.Value), "White");
                    return;
                }
            }
        }
    }
}
