﻿using System;
using NarlonLib.Math;

namespace FEGame.Datas.Others
{
    internal static class GameResourceBook
    {
        private const int GoldFactor = 4;

        /// <summary>
        /// 出售道具所得
        /// </summary>
        public static uint InGoldSellItem(int rare, int rate)
        {
            int[] rareArray = {2, 3, 5, 8, 12, 18, 25, 40};
            
            return (uint)(rareArray[rare] * GoldFactor * rate / 100) / GoldFactor;
        } 
        /// <summary>
        /// 购买道具付出
        /// </summary>
        public static uint OutGoldSellItem(int rare, int rate)
        {
            int[] rareArray = { 2, 3, 5, 8, 12, 18, 25, 40 };

            return (uint)(rareArray[rare] * GoldFactor * rate / 100);
        }
        /// <summary>
        /// 战斗获得金币
        /// </summary>
        public static uint InGoldFight(int level, bool isPeople)
        {
            uint get = Math.Max(1, (uint) (ExpTree.GetGoldFactor(level)*GoldFactor/2));
            return isPeople ? get : get/2;
        }
        /// <summary>
        /// 场景剧情获得金币
        /// </summary>
        public static uint InGoldSceneQuest(int level, int rate, bool noRandom = false)
        {
            if (rate <= 0)
                return 0;
            double[] factor = new[] {0.2, 0.5, 0.5, 1, 1, 1.5, 2.2};
            rate = (int) (rate*(noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return Math.Max(1, (uint)(ExpTree.GetGoldFactor(level)*GoldFactor*rate/100));
        }
        /// <summary>
        /// 场景剧情消耗金币
        /// </summary>
        public static uint OutGoldSceneQuest(int level, int rate, bool noRandom = false)
        {
            if (rate <= 0)
                return 0;
            double[] factor = new[] { 0.5, 1, 1, 1.5};
            rate = (int) (rate*(noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]))*3/4;
            return Math.Max(1, (uint)(ExpTree.GetGoldFactor(level) * GoldFactor * rate / 100));
        }
        /// <summary>
        /// 消耗金钱制作装备图纸,level 1-5
        /// </summary>
        public static uint OutGoldMerge(int qual)
        {
            return (uint)((float)2 * 2 * Math.Sqrt(qual))*10;//2-5-10-16
        }
        /// <summary>
        /// 场景剧情获得食物
        /// </summary>
        public static uint InFoodSceneQuest(int rate, bool noRandom = false)
        {
            double[] factor = new[] { 0.5, 1, 1, 1.5 };
            rate = (int)(rate * (noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return (uint)(10 * rate / 100);
        }
        /// <summary>
        /// 场景剧情消耗食物
        /// </summary>
        public static uint OutFoodSceneQuest(int rate, bool noRandom = false)
        {
            double[] factor = new[] { 0.5, 1, 1, 1.5 };
            rate = (int)(rate * (noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return (uint)(8 * rate / 100);
        }
        /// <summary>
        /// 场景剧情获得健康
        /// </summary>
        public static uint InHealthSceneQuest(int rate, bool noRandom = false)
        {
            double[] factor = new[] { 0.5, 0.5, 1, 1, 2 };
            rate = (int)(rate * (noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return (uint)(10 * rate / 100);
        }
        /// <summary>
        /// 场景剧情消耗健康
        /// </summary>
        public static uint OutHealthSceneQuest(int rate, bool noRandom = false)
        {
            double[] factor = new[] { 0.5, 0.5, 1, 1, 2 };
            rate = (int)(rate * (noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return (uint)(10 * rate / 100);
        }
        /// <summary>
        /// 场景剧情获得精神
        /// </summary>
        public static uint InMentalSceneQuest(int rate, bool noRandom = false)
        {
            double[] factor = new[] { 0.2, 0.2, 0.5, 0.5, 1, 1, 2, 3 };
            rate = (int)(rate * (noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return (uint)(15 * rate / 100);
        }
        /// <summary>
        /// 场景剧情消耗精神
        /// </summary>
        public static uint OutMentalSceneQuest(int rate, bool noRandom = false)
        {
            double[] factor = new[] { 0.2, 0.2, 0.5, 0.5, 1, 1, 2, 3 };
            rate = (int)(rate * (noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return (uint)(15 * rate / 100);
        }
        /// <summary>
        /// 场景剧情获得经验
        /// </summary>
        public static uint InExpSceneQuest(int level, int rate)
        {
            if (rate > 0)
                return Math.Max(1, (uint)(10 * rate / 100));
            return 0;
        }
        /// <summary>
        /// 战斗获得经验值
        /// </summary>
        public static uint InExpFight(int level, int rLevel)
        {
            return Math.Max(1, (uint)(150 / (15 + Math.Max(0, level - rLevel) * 3) + 1));
        }


        /// <summary>
        /// sq资源获得通用入口
        /// </summary>
        public static uint InResSceneQuest(int resId, int level, int rate, bool noRandom = false)
        {
            if (rate <= 0)
                return 0;
            double[] factor = new[] { 0.2, 0.5, 0.5, 1, 1, 1.5, 2.2 };
            rate = (int)(rate * (noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            var count = (uint) (ExpTree.GetResFactor(level)*rate/100);
            if (resId == (int)GameResourceType.Lumber || resId == (int)GameResourceType.Stone)
                count *= 2;
            return Math.Max(1, count);
        }
        /// <summary>
        /// ore资源获得通用入口
        /// </summary>
        public static uint InResBuildOre(int resId, int level)
        {
            uint count = 0;
            if (resId == (int)GameResourceType.Gold)
                count = (uint)ExpTree.GetGoldFactor(level);
            else
                count = (uint)ExpTree.GetResFactor(level);
            if (resId == (int)GameResourceType.Lumber || resId == (int)GameResourceType.Stone)
                count *= 2;
            var valGet = MathTool.GetRandom(count*0.75, count*1.25);
            return (uint)Math.Max(1, valGet);
        }
        /// <summary>
        /// 消耗石材制作装备,level 1-5
        /// </summary>
        public static uint OutStoneCompose(int qual)
        {
            return 60 + (uint)qual*40;
        }
        /// <summary>
        /// 消耗木材制作装备,level 1-5
        /// </summary>
        public static uint OutWoodCompose(int qual)
        {
            return 60 + (uint)qual * 40;
        }
        /// <summary>
        /// 消耗木材建设农场
        /// </summary>
        public static uint OutWoodBuildFarm(uint cost)
        {
            return cost;
        }

        /// <summary>
        /// 购买武器卡牌消耗Gem
        /// </summary>
        public static uint OutGemCardBuy(int qual)
        {
            if (qual == 0)
                return 1;
            return (uint)(qual * (qual + 1) * Math.Sqrt(qual)); //2-8-20-40
        }
        /// <summary>
        /// 购买怪物卡牌消耗Carbuncle
        /// </summary>
        public static uint OutCarbuncleCardBuy(int qual)
        {
            if (qual == 0)
                return 1;
            return (uint)(qual * (qual + 1) * Math.Sqrt(qual)); //2-8-20-40
        }
        /// <summary>
        /// 战斗中贿赂怪物消耗Carbuncle
        /// </summary>
        public static uint OutCarbuncleBribe(int myLevel, int level)
        {
            int levelDiffer = level - myLevel;
            if (levelDiffer < -5)
                return 3;
            if (levelDiffer <= 0)
                return 5;
            return (uint)(levelDiffer * (levelDiffer + 1)) + 5;
        }
        /// <summary>
        /// 消耗水银购买祝福
        /// </summary>
        public static uint OutMercuryBlessBuy(int level)
        {
            return (uint)(3*level);
        }
        /// <summary>
        /// 购买法术卡牌消耗Mercury
        /// </summary>
        public static uint OutMercuryCardBuy(int qual)
        {
            if (qual == 0)
                return 1;
            return (uint)(qual * (qual + 1) * Math.Sqrt(qual)); //2-8-20-40
        }
        /// <summary>
        /// 刷新一些属性时效果
        /// </summary>
        public static uint OutSulfurRefresh(float factor)
        {
            return (uint)(10 * factor);
        }
    }
}
