using ConfigDatas;

namespace FEGame.Core
{
    internal static class HSTypes
    {
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

        public static string I2TaskStateColor(int id)
        {
            string[] rt = { "", "White", "Green", "Orange", "Red" };
            return rt[id];
        }

        public static string I2QuestDangerColor(int id)
        {
            string[] rt = { "White", "Yellow", "Orange","Red" };
            return rt[id];
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
