﻿using ConfigDatas;

namespace FEGame.Core
{
    internal static class HSTypes
    {
        public static string I2Attr(int aid)
        {
            string[] rt = { "无", "水", "风", "火", "地", "光", "暗"};
            return rt[aid];
        }

        public static string I2BuffImmune(int aid)
        {
            string[] rt = { "生命", "意志", "物理", "元素" };
            return rt[aid];
        }

        public static string I2CardTypeSub(int rid)
        {
            if (rid > 0 && rid < 100)
                return ConfigData.GetMonsterRaceConfig(rid).Name;
            switch (rid)
            {
                case 100: return "武器";
                case 101: return "卷轴";
                case 102: return "防具";
                case 103: return "饰品";
                case 104: return "神器";

                case 200: return "单体法术";
                case 201: return "群体法术";
                case 202: return "基本法术";
                case 203: return "地形变化";
            }

            return "";
        }
        public static string I2CardTypeDesSub(int rid)
        {
            if (rid > 0 && rid < 100)
                return ConfigData.GetMonsterRaceConfig(rid).Des;
            switch (rid)
            {
                case 100: return "--每次命中敌人消耗耐久";
                case 101: return "--每次命中敌人消耗耐久";
                case 102: return "--每次抵御伤害消耗耐久";
                case 103: return "--每回合消耗耐久";
                case 104: return "--影响整个战场的强力道具";

                case 200: return "单体法术";
                case 201: return "群体法术";
                case 202: return "基本法术";
                case 203: return "地形变化";
            }

            return "";
        }

        public static string I2HItemType(int id)
        {
            string[] rt = { "", "普通", "材料", "任务" };
            return rt[id];
        }
        public static string I2EquipSlotType(int id)
        {
            string[] rt = { "", "主楼", "旗帜", "武器", "城墙" , "附楼" };
            return rt[id];
        }

        public static string I2Quality(int id)
        {
            string[] rt = {"普通", "良好", "优质", "史诗", "传说", "神", "烂"};
            return rt[id];
        }

        public static string I2QualityColor(int id)
        {
            string[] rt = { "White", "Green", "DodgerBlue", "Violet", "Orange", "Gray", "Gray", "", "", "" };
            return rt[id];
        }

        public static string I2QualityColorD(int id)
        {
            string[] rt = { "DimGray", "DarkGreen", "MediumBlue", "DarkViolet", "OrangeRed", "Gray", "Gray", "", "", "" };
            return rt[id];
        }

        public static string I2EaddonColor(int id)
        {
            string[] rt = { "Gray", "White", "Green", "DodgerBlue", "Violet", "Orange", "Red" };
            return rt[id];
        }

        //道具的稀有度
        public static string I2RareColor(int value)
        {
            string[] rares = { "Gray", "Green", "Green", "DodgerBlue", "DodgerBlue", "Violet", "Orange", "Red" };
            return rares[value];
        }

        public static string I2Resource(int id)
        {
            string[] rt = { "黄金", "木材", "矿石", "水银", "红宝石", "硫磺", "水晶" };
            return rt[id];
        }

        public static string I2ResourceColor(int id)
        {
            string[] rt = { "Gold", "DarkGoldenrod", "DarkKhaki", "White", "Red", "Yellow", "DodgerBlue" };
            return rt[id];
        }

        public static string I2ResourceTip(int id)
        {
            string[] rt = {
                "世界的基本货币",
                "用来建造建筑和扩建农场",
                "用来建造建筑",
                "用来购买祝福和魔法卡牌",
                "用来贿赂和购买怪物卡牌",
                "用来刷新和重置玩法",
                "用来购买武器卡牌"
                          };
            return rt[id];
        }

        public static string I2CardLevelColor(int id)
        {
            string[] rt = { "", "White", "White", "DodgerBlue", "DodgerBlue", "Yellow", "Orange", "Red", "Red" };
            return rt[id];
        }

        public static string I2TaskStateColor(int id)
        {
            string[] rt = { "", "White", "Green", "Orange", "Red" };
            return rt[id];
        }

        public static string I2LevelInfoColor(int id)
        {
            string[] rt = { "White", "Lime", "Violet" };
            return rt[id];
        }

        public static string I2QuestDangerColor(int id)
        {
            string[] rt = { "White", "Yellow", "Orange","Red" };
            return rt[id];
        }

        public static string I2LevelInfoType(int id)
        {
            string[] rt = { "", "新功能", "获得道具" };
            return rt[id];
        }
        public static string I2AttrName(int id)
        {
            string[] rt = { "攻击", "防御", "魔力", "攻速", "命中", "回避", "暴击", "幸运", "生命"};
            return rt[id];
        }

        public static string I2HeroAttrTip(int id)
        {
            string[] rt = { 
                "攻击:提升主塔伤害", 
                "生命:提升主塔最大生命",
                "攻速:提升攻击速度",
                "射程:提升射击范围",
                "领导:提升LP回复比率",
                "力量:提升PP回复比率",
                "魔力:提升MP回复比率"
                          };
            return rt[id];
        }

        public static string I2InitialAttrTip(int aid)
        {
            string[] rt = {
                              "无属性$-10单位无属性粉末",
                              "水属性$-10单位水属性粉末",
                              "风属性$-10单位风属性粉末",
                              "火属性$-10单位火属性粉末",
                              "地属性$-10单位地属性粉末",
                              "光属性$-10单位光属性粉末",
                              "暗属性$-10单位暗属性粉末"
                          };
            return rt[aid];
        }

        public static string I2ConstellationTip(int aid)
        {
            string[] rt = {
                              "白羊座$3/22-4/20$虽然你是乐天派，但凡事要稳着点",
                              "金牛座$4/21-5/20$偶尔跨出保守，是良性的冒险",
                              "双子座$5/21-6/21$好奇、好动、好玩，你的人生好精彩",
                              "巨蟹座$6/22-7/22$减少情绪风暴发生的次数，你的人生更美好",
                              "狮子座$7/23-8/22$小心被强烈的自尊心反咬一口",
                              "处女座$8/23-9/22$注意小细节，更要抓紧大方向",
                              "天秤座$9/23-10/23$持中庸之道享受平稳的人生",
                              "天蝎座$10/24-11/22$可以孤独，但勿封闭",
                              "射手座$11/23-12/21$追求自由，接近阳光",
                              "摩羯座$12/22-1/20$踏实的作风助你达成目标",
                              "水瓶座$1/21-2/19$热爱知识，精神生活丰富",
                              "双鱼座$2/20-3/21$要加强锻炼意志力"
                          };
            return rt[aid];
        }

        public static string I2BloodTypeTip(int aid)
        {
            string[] rt = {
                              "A型$重视外界反映，是完美主义者",
                              "B型$我行我素，兴趣广泛",
                              "AB型$一心二用，自由奔放",
                              "O型$热爱生活，重视力量"
                          };
            return rt[aid];
        }

        public static string I2TemperatureName(int id)
        {
            string[] rt = { "-", "严寒", "冷", "常温", "热", "酷暑" };
            return rt[id];
        }
        public static string I2HumitityName(int id)
        {
            string[] rt = { "-", "极湿", "潮湿", "舒适", "干燥", "极干" };
            return rt[id];
        }
        public static string I2AltitudeName(int id)
        {
            string[] rt = { "-", "谷底", "低地", "平原", "山丘", "高原" };
            return rt[id];
        }
    }
}
