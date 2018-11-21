using System.Collections.Generic;
using FEGame.Core;

namespace FEGame.DataType.User
{
    public class InfoWorld : IUserInfoSub
    {
        [FieldIndex(Index = 10)] public List<int> SavedDungeonQuests;

        public InfoWorld()
        {
            SavedDungeonQuests = new List<int>();
        }
        void IUserInfoSub.OnLogin()
        {
        }

        void IUserInfoSub.OnLogout()
        {
        }
    }
}
