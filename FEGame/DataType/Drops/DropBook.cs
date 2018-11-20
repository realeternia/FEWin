using System;
using System.Collections.Generic;
using ConfigDatas;
using FEGame.DataType.Items;
using NarlonLib.Log;
using NarlonLib.Math;

namespace FEGame.DataType.Drops
{
    public static class DropBook
    {
        private static Dictionary<string, int> itemNameIdDict;
        public static int GetDropId(string ename)
        {
            if (itemNameIdDict == null)
            {
                itemNameIdDict = new Dictionary<string, int>();
                foreach (var hItemConfig in ConfigData.DropDict.Values)
                {
                    if (itemNameIdDict.ContainsKey(hItemConfig.Ename))
                    {
                        NLog.Warn("GetDropId key={0} exsited", hItemConfig.Ename);
                        continue;
                    }
                    itemNameIdDict[hItemConfig.Ename] = hItemConfig.Id;
                }
            }
            return itemNameIdDict[ename];
        }

        public static List<int> GetDropItemList(string groupName)
        {
            return GetDropItemList(GetDropId(groupName));
        }

        public static List<int> GetDropItemList(int groupId)
        {
            List<int> items = new List<int>();
            var dropConfig = ConfigData.GetDropConfig(groupId);
            
            for (int j = 0; j < dropConfig.Count; j++)
            {
                if (dropConfig.Items.Length > 0)
                    DropItems(dropConfig.Items, dropConfig.ItemRate, items);
            }
            return items;
        }

        private static void DropItems(string[] dropItems, int[] rates, List<int> items)
        {
            int roll = MathTool.GetRandom(100);
            int sum = 0;
            for (int i = 0; i < dropItems.Length; i++)
            {
                sum += rates[i];
                if (sum > roll)
                {
                    var itemId = HItemBook.GetItemId(dropItems[i]);
                    items.Add(itemId);
                    break;
                }
            }
        }

    }
}