using ConfigDatas;

namespace FEGame.Controller.Battle.Units
{
    public class MonsterSam : BaseSam
    {
        public MonsterSam(int id, byte x, byte y)
            : base(id, x, y)
        {
        }

        public override void Init()
        {
            var samuraiConfig = ConfigData.GetSamuraiConfig(Cid);
            baseAttr.Str = samuraiConfig.Str;
            baseAttr.Def = samuraiConfig.Def;
            baseAttr.Skl = samuraiConfig.Skl;
            baseAttr.Spd = samuraiConfig.Spd;
            baseAttr.Hp = samuraiConfig.Hp;
            baseAttr.Mag = samuraiConfig.Mag;
            baseAttr.Luk = samuraiConfig.Luk;
            baseAttr.Mov = samuraiConfig.Mov;
            Level = samuraiConfig.Level;
        }
    }
}