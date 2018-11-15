namespace FEGame.Core
{
    internal static class HSErrors
    {
        public static string GetDescript(int id)
        {
            return ConfigDatas.ConfigData.GetErrorConfig(id).Des;
        }
    }
}
