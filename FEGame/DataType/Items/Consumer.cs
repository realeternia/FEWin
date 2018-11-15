using ConfigDatas;
using FEGame.DataType.User;

namespace FEGame.DataType.Items
{
    internal static class Consumer
    {
        public static bool UseItemsById(int id, HItemUseTypes useMethod)
        {
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);
            ItemConsumerConfig consumerConfig = ConfigData.GetItemConsumerConfig(id);
            if (useMethod == HItemUseTypes.Common)
            {
                if (itemConfig.SubType == (int)HItemTypes.Item)
                    return UseItem(consumerConfig);
            }

            return false;
        }

        private static bool UseItem(ItemConsumerConfig itemConfig)
        {
            if (itemConfig.ResourceId > 0)
                UserProfile.InfoBag.AddResource((GameResourceType)(itemConfig.ResourceId - 1), (uint)itemConfig.ResourceCount);
            if (itemConfig.GainExp > 0)
                UserProfile.InfoBasic.AddExp(itemConfig.GainExp);
            if (itemConfig.GainFood > 0)
                UserProfile.InfoBasic.AddFood((uint)itemConfig.GainFood);
            if (itemConfig.GainHealth > 0)
                UserProfile.InfoBasic.AddHealth((uint)itemConfig.GainHealth);
            if (itemConfig.GainMental > 0)
                UserProfile.InfoBasic.AddMental((uint)itemConfig.GainMental);
            if (itemConfig.DungeonAttr != null && itemConfig.DungeonAttr.Length > 0)
            {
                if (UserProfile.InfoDungeon.DungeonId < 0)
                    return false;
                UserProfile.InfoDungeon.ChangeAttr(itemConfig.DungeonAttr[0], itemConfig.DungeonAttr[1]
                    , itemConfig.DungeonAttr[2], itemConfig.DungeonAttr[3], itemConfig.DungeonAttr[4]);
            }
            return true;
        }

    }
}
