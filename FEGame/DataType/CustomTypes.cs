namespace FEGame.DataType
{
    internal enum HItemTypes
    {
        Common = 1, //用于type
        Material = 2,
        Task = 3,

        Dull = 10,
        Fight = 11, //用于subtype
        Gift = 12,
        Item = 13,
      //  Ore = 14,
        DropItem = 15,
        RandomCard = 16,
        Seed = 17,
    }

    public enum HItemUseTypes
    {
        Common,
        Fight,
        Seed
    }

    internal enum HItemRandomGroups
    {
        None = 0,
        Flower = 1,
        Fish = 2,
        Ore = 3,
        Mushroom = 4,
        Wood = 5,
        Fight = 10,
        Shopping = 20,
    }

    internal enum BuffEffectTypes
    {
        NoAction = 2,
        NoSkill = 5, //一般是晕眩，冰冻等时候
        NoMove = 6,

        Tile = 101,
        Shield = 102,
        Chaos = 103,
    }

    internal enum GameResourceType
    {
        /// <summary>
        /// 黄金
        /// </summary>
        Gold = 0,
        /// <summary>
        /// 木材
        /// </summary>
        Lumber,
        /// <summary>
        /// 石头
        /// </summary>
        Stone,
        /// <summary>
        /// 水银
        /// </summary>
        Mercury,
        /// <summary>
        /// 红宝石
        /// </summary>
        Carbuncle,
        /// <summary>
        /// 硫磺
        /// </summary>
        Sulfur,
        /// <summary>
        /// 水晶
        /// </summary>
        Gem
    }

    internal enum SceneFreshReason
    {
        Load,
        Warp,
        Reset
    }

    internal enum ToolBarItemTypes
    {
        Normal, Time, Limit
    }

}