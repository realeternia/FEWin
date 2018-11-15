﻿using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using FEGame.DataType;
using FEGame.DataType.Drops;
using FEGame.DataType.Others;
using FEGame.DataType.Scenes;
using FEGame.DataType.User;
using FEGame.Forms.Items.Core;
using NarlonLib.Math;

namespace FEGame.Forms.CMain.Quests.SceneQuests
{
    internal class SceneQuestAnswer : SceneQuestBlock
    {
        private ColorWordRegion colorWord;//问题区域

        public SceneQuestAnswer(Control p, int eid, int lv, string s, int depth, int line)
            : base(p, eid, lv, s, depth, line)
        {
            colorWord = new ColorWordRegion(0, 0, 400, new Font("微软雅黑", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel), Color.White);
            CheckScript(p);
        }

        private void CheckScript(Control p)
        {
            if (Script[0] == '#')
            {
                string[] infos = Script.Split('#');
                Script = infos[infos.Length - 1];
                for (int i = 1; i < infos.Length-1; i++)
                {
                    CheckCondition(infos[i]);
                    if (Disabled)
                        return; //有一个条件不满足就结束
                }
            }

            SetScript(Script);
        }

        private void CheckCondition(string info)
        {
            string[] parms = info.Split('-');
            var config = ConfigData.GetSceneQuestConfig(eventId);

            if (parms[0] == "cantrade")
            {
                int multi = int.Parse(parms[1]);
                string type = "all";
                if (parms.Length > 2)
                    type = parms[2];
                double multiNeed = multi*MathTool.Clamp(1, 0.2, 5);
                double multiGet = multi*MathTool.Clamp(1, 0.2, 5);
                uint goldNeed = 0;
                if (config.TradeGold < 0)
                    goldNeed = GameResourceBook.OutGoldSceneQuest(level, (int)(-config.TradeGold* multiNeed), true);
                uint foodNeed = 0;
                if (config.TradeFood < 0)
                    foodNeed = Math.Min(100, GameResourceBook.OutFoodSceneQuest((int)(-config.TradeFood* multiNeed), true));
                uint healthNeed = 0;
                if (config.TradeHealth < 0)
                    healthNeed = Math.Min(100, GameResourceBook.OutHealthSceneQuest((int)(-config.TradeHealth* multiNeed), true));
                uint mentalNeed = 0;
                if (config.TradeMental < 0)
                    mentalNeed = Math.Min(100, GameResourceBook.OutMentalSceneQuest((int)(-config.TradeMental* multiNeed), true));
                Disabled = !UserProfile.Profile.InfoBag.HasResource(GameResourceType.Gold, goldNeed) ||
                           UserProfile.Profile.InfoBasic.FoodPoint < foodNeed ||
                           UserProfile.Profile.InfoBasic.HealthPoint < healthNeed ||
                           UserProfile.Profile.InfoBasic.MentalPoint < mentalNeed;

                if (string.IsNullOrEmpty(config.TradeDropItem))
                {
                    uint goldAdd = 0;
                    if (config.TradeGold > 0 && (type == "all" || type == "gold"))
                        goldAdd = GameResourceBook.InGoldSceneQuest(level, (int)(config.TradeGold* multiGet), true);
                    uint foodAdd = 0;
                    if (config.TradeFood > 0 && (type == "all" || type == "food"))
                        foodAdd = Math.Min(100, GameResourceBook.InFoodSceneQuest((int)(config.TradeFood* multiGet), true));
                    uint healthAdd = 0;
                    if (config.TradeHealth > 0 && (type == "all" || type == "health"))
                        healthAdd = Math.Min(100, GameResourceBook.InHealthSceneQuest((int)(config.TradeHealth* multiGet), true));
                    uint mentalAdd = 0;
                    if (config.TradeMental > 0 && (type == "all" || type == "mental"))
                        mentalAdd = Math.Min(100, GameResourceBook.InMentalSceneQuest((int)(config.TradeMental* multiGet), true));
                    Script = string.Format("获得{0}(消耗{1})",
                        GetTradeStr(goldAdd, foodAdd, healthAdd, mentalAdd),
                        GetTradeStr(goldNeed, foodNeed, healthNeed, mentalNeed));
                }
                else
                {
                    var dropId = DropBook.GetDropId(config.TradeDropItem);
                    Script = string.Format("获得{0}(消耗{1})",
                        ConfigData.GetDropConfig(dropId).Name,
                        GetTradeStr(goldNeed, foodNeed, healthNeed, mentalNeed));

                }
            }
            else if (parms[0] == "cantest")
            {
                int type = int.Parse(parms[1]);
                bool canConvert = type == 1; //是否允许转换成幸运检测

                var testType = type == 1 ? config.TestType1 : config.TestType2;
                int sourceVal = UserProfile.InfoDungeon.GetAttrByStr(testType);
                Disabled = UserProfile.InfoDungeon.DungeonId <= 0 || sourceVal < 0;
                if (Disabled && canConvert)
                    Disabled = false;

                if (!Disabled)
                {
                    var biasData = type == 1 ? config.TestBias1 : config.TestBias2;
                    if (UserProfile.InfoDungeon.DungeonId > 0 && UserProfile.InfoDungeon.GetAttrByStr(testType) >= 0)
                    {
                        var attrNeed = UserProfile.InfoDungeon.GetRequireAttrByStr(testType, biasData);
                        Script = string.Format("|icon.oth1||进行{0}考验|lime|(判定{1} {2:0.0}%胜率)", GetTestAttrStr(testType), attrNeed, 
                            GetWinRate(UserProfile.InfoDungeon.GetAttrByStr(testType) +0.5f, attrNeed));
                    }
                    else //因为convert了
                    {
                        Script = string.Format("|icon.oth1||进行运气考验|lime|(判定{0} {1:0.0}%胜率)", 3 + biasData, 
                            GetWinRate(3.5f, 3 + biasData));
                    }
                }
            }
            else if (parms[0] == "hasditem")
            {
                var itemId = DungeonBook.GetDungeonItemId(config.NeedDungeonItemId);
                Disabled = UserProfile.InfoDungeon.GetDungeonItemCount(itemId) < config.NeedDungeonItemCount;
            }
            else if (parms[0] == "hasdna")
            {
                if (config.DnaInfo != null && config.DnaInfo.Length > 0)
                {
                    string dnaStr = "";
                    int dnaId = 0;
                    foreach (var dnaName in config.DnaInfo)
                    {
                        var nowId = DnaBook.GetDnaId(dnaName);
                        dnaId |= (int)Math.Pow(2, nowId);
                        dnaStr += ConfigData.GetPlayerDnaConfig(nowId).Name + " ";
                    }
                    Script = string.Format("|icon.oth14||{0}|lime|(DNA限定{1})", Script, dnaStr);
                    Disabled = !UserProfile.InfoBasic.HasDna(dnaId);
                }
            }
        }

        private float GetWinRate(float myData, float needData)
        {
            if (myData*2 < needData)
                return 0;
            if (myData*2 == needData)
                return (float) Math.Pow((float) 1/3, myData)*100;
            return myData*115/(myData + needData);
        }

        private string GetTradeStr(uint goldNeed, uint foodNeed, uint healthNeed, uint mentalNeed)
        {
            string addStr = "";
            if (goldNeed > 0) addStr += goldNeed + "点金币 ";
            if (foodNeed > 0) addStr += foodNeed + "点食物 ";
            if (healthNeed > 0) addStr += healthNeed + "点生命 ";
            if (mentalNeed > 0) addStr += mentalNeed + "点精神 ";

            return addStr;
        }

        private string GetTestAttrStr(string type)
        {
            switch (type)
            {
                case "str": return "力量";
                case "agi": return "敏捷";
                case "intl": return "智慧";
                case "perc": return "感知";
                case "endu": return "耐力";
            }
            return "未知";
        }

        public override void SetRect(Rectangle r)
        {
            base.SetRect(r);

            colorWord.UpdateRect(r);
            SetScript(savedStr); //刷新一次
        }

        private string savedStr;
        public override void SetScript(string s)
        {
            base.SetScript(s);

            savedStr = Script;
            Graphics g = parent.CreateGraphics();
            colorWord.UpdateText(Script, g);
            g.Dispose();
        }

        public override void Draw(Graphics g)
        {
            colorWord.Draw(g);
        }
    }
}