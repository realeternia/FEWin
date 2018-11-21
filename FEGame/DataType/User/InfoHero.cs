using System.Collections.Generic;
using FEGame.Core;
using FEGame.DataType.User.Db;

namespace FEGame.DataType.User
{
    public class InfoHero : IUserInfoSub
    {
        [FieldIndex(Index = 1)] public List<DbHeroAttr> Heros;

        public InfoHero()
        {
            Heros = new List<DbHeroAttr>();
        }

        void IUserInfoSub.OnLogin()
        {
            AddHero(43020101); //todo test
            AddHero(43020102); //todo test
        }

        void IUserInfoSub.OnLogout()
        {
        }

        public void AddHero(int id)
        {
            if (Heros.Find(h => h.SamuraiId == id) != null) //已经存在
                return;

            var samuraiConfig = ConfigDatas.ConfigData.GetSamuraiConfig(id);
            if (samuraiConfig.Id == 0)
                return;
            DbHeroAttr attr = new DbHeroAttr {SamuraiId = id, Level = samuraiConfig.Level};
            Heros.Add(attr);
        }

        public DbHeroAttr GetHero(int id)
        {
            return Heros.Find(h => h.SamuraiId == id);
        }
    }
}