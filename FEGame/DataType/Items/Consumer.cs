using ConfigDatas;
using FEGame.DataType.User;

namespace FEGame.DataType.Items
{
    internal static class Consumer
    {
        public static bool UseItemsById(int id, HItemUseTypes useMethod)
        {
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);
            //ItemConsumerConfig consumerConfig = ConfigData.GetItemConsumerConfig(id);
            //if (useMethod == HItemUseTypes.Common)
            //{
            //    if (itemConfig.SubType == (int)HItemTypes.Item)
            //        return UseItem(consumerConfig);
            //}

            return false;
        }

    }
}
