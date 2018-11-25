﻿using System;
using System.Collections.Generic;
using ConfigDatas;
using FEGame.Core;
using FEGame.DataType.Items;
using FEGame.DataType.Others;
using FEGame.Forms.CMain;
using NarlonLib.Core;
using NarlonLib.Tools;

namespace FEGame.DataType.User
{
    public class InfoBag : IUserInfoSub
    {
        [FieldIndex(Index = 1)] public GameResource Resource;
        [FieldIndex(Index = 2)] public int Diamond;
        [FieldIndex(Index = 3)] public IntPair[] Items;
        [FieldIndex(Index = 4)] public int BagCount;
        [FieldIndex(Index = 5)] public int[] CdGroupStartTime; //开始时间
        [FieldIndex(Index = 6)] public int[] CdGroupTime; //到期时间

        public InfoBag()
        {
            Resource = new GameResource();
            Items = new IntPair[GameConstants.BagInitCount];
            for (int i = 0; i < GameConstants.BagInitCount; i++)
                Items[i] = new IntPair();
            CdGroupStartTime = new int[GameConstants.ItemCdGroupCount];
            CdGroupTime = new int[GameConstants.ItemCdGroupCount];
        }
        void IUserInfoSub.OnLogin()
        {
        }

        void IUserInfoSub.OnLogout()
        {
        }

        public bool CheckResource(uint[] resourceInfo)
        {
            if (Resource.Gold >= resourceInfo[0] &&
                Resource.Lumber >= resourceInfo[1] &&
                Resource.Stone >= resourceInfo[2] &&
                Resource.Mercury >= resourceInfo[3] &&
                Resource.Carbuncle >= resourceInfo[4] &&
                Resource.Sulfur >= resourceInfo[5] &&
                Resource.Gem >= resourceInfo[6])
                return true;
            return false;
        }

        internal bool HasResource(GameResourceType type, uint value)
        {
            return Resource.Has(type, (int)value);
        }

        public void AddDiamond(int value)
        {
            Diamond += value;
            MainTipManager.AddTip(string.Format("|获得|Cyan|{0}||钻石", value), "White");
        }

        public void AddResource(uint[] res)
        {
            for (int i = 0; i < 7; i++)
            {
                if (res[i] > 0)
                    AddResource((GameResourceType)i, res[i]);
            }
        }

        internal void AddResource(GameResourceType type, uint value)
        {
            Resource.Add(type, value);

            MainTipManager.AddTip(string.Format("|获得|{0}|{1}||x{2}", HSTypes.I2ResourceColor((int)type), HSTypes.I2Resource((int)type), value), "White");
        }

        public bool PayDiamond(int value)
        {
            if (Diamond < value)
                return false;
            Diamond -= value;
            MainTipManager.AddTip(string.Format("|失去了|Cyan|{0}||钻石,账户剩余|Cyan|{1}||钻石", value, Diamond), "White");
            return true;
        }

        public void SubResource(uint[] res)
        {
            for (int i = 0; i < 7; i++)
            {
                if (res[i] > 0)
                    SubResource((GameResourceType) i, res[i]);
            }
        }

        internal void SubResource(GameResourceType type, uint value)
        {
            Resource.Sub(type, value);

            MainTipManager.AddTip(string.Format("|扣除|{0}|{1}||x{2}", HSTypes.I2ResourceColor((int)type), HSTypes.I2Resource((int)type), value), "White");
        }
        
        public void AddItem(int id, int num)
        {
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);
            if (itemConfig.Id == 0)
                return;
        
            int max = itemConfig.MaxPile;
            if (max <= 0)
                return;

            int count = num;
            for (int i = 0; i < BagCount; i++)
            {
                var pickItem = Items[i];
                if (pickItem.Type == id && pickItem.Value < max)
                {
                    if (pickItem.Value + count <= max)
                    {
                        pickItem.Value += count;
                        count = 0;
                        break;
                    }
                    else
                    {
                        count -= max - pickItem.Value;
                        pickItem.Value = max;
                    }
                }
            }
            if (count > 0)
            {
                for (int i = 0; i < BagCount; i++)
                {
                    var pickItem = Items[i];
                    if (pickItem.Type == 0)
                    {
                        pickItem.Type = id;
                        if (count <= max)
                        {
                            pickItem.Value = count;
                            count = 0;
                            break;
                        }
                        else
                        {
                            pickItem.Value = max;
                            count -= max;
                        }
                    }
                }
            }
         
            if (num > count)
            { 
                MainTipManager.AddTip(string.Format("|获得物品-|{0}|{1}||x{2}", HSTypes.I2RareColor(itemConfig.Rare), itemConfig.Name, num - count), "White");
                UserProfile.InfoRecord.AddRecordById(RecordInfoConfig.Indexer.ItemGet, 1);
            }
        }

        public void UseItemByPos(int pos, HItemUseTypes type)
        {
            var pickItem = Items[pos];
            if (pickItem.Value <= 0)
                return;
            var rate = UserProfile.InfoBag.GetCdTimeRate(pickItem.Type);
            if (rate > 0)
                return; //cd中

            if (Consumer.UseItemsById(pickItem.Type, type))
            {
                //var consumerConfig = ConfigData.GetItemConsumerConfig(pickItem.Type);
                //if (consumerConfig.CdGroup > 0)
                //{
                //    CdGroupStartTime[consumerConfig.CdGroup - 1] = TimeTool.GetNowUnixTime();
                //    CdGroupTime[consumerConfig.CdGroup - 1] = TimeTool.GetNowUnixTime() + consumerConfig.CdTime;
                //}

                DeleteItemByPos(pos, 1);

                MainForm.Instance.RefreshView();
            }
            //todo 需要提示下错误类型
        }

        public void ClearItemAllByPos(int pos)
        {
            DeleteItemByPos(pos, 0);
        }

        public void SellItemAllByPos(int pos)
        {
            var pickItem = Items[pos];
            if (pickItem.Type > 0 && pickItem.Value > 0)
            {
                var config = ConfigData.GetHItemConfig(pickItem.Type);
                uint sellPrice = GameResourceBook.InGoldSellItem(config.Rare, config.ValueFactor);
                uint money = (uint)(sellPrice * pickItem.Value);
                AddResource(GameResourceType.Gold, money);
            }
            DeleteItemByPos(pos, 0);
        }
        
        public int GetItemCount(int id)
        {
            int count = 0;
            for (int i = 0; i < BagCount; i++)
            {
                var pickItem = Items[i];
                if (pickItem.Type == id)
                    count += pickItem.Value;
            }
            return count;
        }
        public int GetBlankCount()
        {
            int count = 0;
            for (int i = 0; i < BagCount; i++)
            {
                var pickItem = Items[i];
                if (pickItem.Type == 0)
                    count ++;
            }
            return count;
        }

        private void DeleteItemByPos(int pos, int num)
        {//num=0,移除所有
            IntPair cell = Items[pos];
            var itemConfig = ConfigData.GetHItemConfig(cell.Type);
            if (num != 0 && cell.Value > num)
            {
                cell.Value -= num;
                MainTipManager.AddTip(string.Format("|扣除物品-|{0}|{1}||x{2}", HSTypes.I2RareColor(itemConfig.Rare), itemConfig.Name, num), "White");
            }
            else
            {
                MainTipManager.AddTip(string.Format("|扣除物品-|{0}|{1}||x{2}", HSTypes.I2RareColor(itemConfig.Rare), itemConfig.Name, cell.Value), "White");
                cell.Value = 0;
                cell.Type = 0;
            }
        }


        public void DeleteItem(int id, int num)
        {
            int count = num;
            for (int i = 0; i < BagCount; i++)
            {
                var pickItem = Items[i];
                if (pickItem.Type == id)
                {
                    if (pickItem.Value > count)
                    {
                        pickItem.Value -= count;
                        break;
                    }
                    count -= pickItem.Value;
                    pickItem.Type = 0;
                    pickItem.Value = 0;
                }
            }

            var itemConfig = ConfigData.GetHItemConfig(id);
            MainTipManager.AddTip(string.Format("|扣除物品-|{0}|{1}||x{2}", HSTypes.I2RareColor(itemConfig.Rare), itemConfig.Name, num), "White");
        }

        public void SortItem()
        {
            Array.Sort(Items, new CompareByMid());
            for (int i = 0; i < Items.Length; i++)
            {
                var pickItem = Items[i];
                if (pickItem.Type == 0 || pickItem.Value == 0)
                    break;
                int max = ConfigData.GetHItemConfig(pickItem.Type).MaxPile;
                if (pickItem.Value < max && i+1 < Items.Length && pickItem.Type == Items[i + 1].Type)
                {
                    if (pickItem.Value + Items[i + 1].Value <= max)
                    {
                        pickItem.Value = pickItem.Value + Items[i + 1].Value;
                        Items[i + 1].Type = 0;
                        Items[i + 1].Value = 0;
                    }
                    else
                    {
                        Items[i + 1].Value = pickItem.Value + Items[i + 1].Value - max;
                        pickItem.Value = max;
                    }
                }
            }
        }

        public List<IntPair> GetItemCountByType(int type)
        {
            AutoDictionary<int, int> counter = new AutoDictionary<int, int>();
            for (int i = 0; i < BagCount; i++)
            {
                HItemConfig itemConfig = ConfigData.GetHItemConfig(Items[i].Type);
                if (itemConfig != null && itemConfig.Type == type)
                    counter[itemConfig.Id] += Items[i].Value;
            }
            List<IntPair> datas = new List<IntPair>();
            foreach (int itemId in counter.Keys())
            {
                IntPair pairData = new IntPair
                {
                    Type = itemId,
                    Value = counter[itemId]
                };
                datas.Add(pairData);
            }
            return datas;
        }

        public List<IntPair> GetItemCountByAttribute(string attrName)
        {
            AutoDictionary<int, int> counter = new AutoDictionary<int, int>();
            for (int i = 0; i < BagCount; i++)
            {
                HItemConfig itemConfig = ConfigData.GetHItemConfig(Items[i].Type);
                if (itemConfig != null && itemConfig.Attributes != null && Array.IndexOf(itemConfig.Attributes, attrName) >= 0)
                    counter[itemConfig.Id] += Items[i].Value;
            }
            List<IntPair> datas = new List<IntPair>();
            foreach (int itemId in counter.Keys())
            {
                IntPair pairData = new IntPair
                {
                    Type = itemId,
                    Value = counter[itemId]
                };
                datas.Add(pairData);
            }
            return datas;
        }

        public void ResizeBag(int newSize)
        {
            IntPair[] item = new IntPair[BagCount];
            Array.Copy(Items, item, BagCount);
            Items = new IntPair[newSize];
            Array.Copy(item, Items, BagCount);
            for (int i = BagCount; i < newSize; i++)
                Items[i] = new IntPair();
            BagCount = newSize;
        }

        public float GetCdTimeRate(int itemId)
        {
            if (itemId == 0)
                return 0;
            var canUse = ConfigData.GetHItemConfig(itemId).IsUsable;
            if (canUse)
            {
                //var consumerConfig = ConfigData.GetItemConsumerConfig(itemId);
                //var group = consumerConfig.CdGroup;
                //if (group > 0 && CdGroupStartTime[group-1] > 0)
                //{
                //    var nowTime = TimeTool.GetNowUnixTime();
                //    if (nowTime >= CdGroupTime[group - 1])
                //        return 0;
                //    else
                //        return (float)(nowTime - CdGroupStartTime[group - 1]) / (CdGroupTime[group - 1] - CdGroupStartTime[group - 1]);
                //}
            }
            return 0;
        }

    }
}
