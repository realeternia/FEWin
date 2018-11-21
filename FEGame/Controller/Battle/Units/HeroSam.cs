using ConfigDatas;
using FEGame.DataType.User;

namespace FEGame.Controller.Battle.Units
{
    public class HeroSam : BaseSam
    {
        public HeroSam(int id, byte x, byte y)
            : base(id, x, y)
        {
        }

        public override void Init()
        {
            base.Init();

            var heroData = UserProfile.InfoHero.GetHero(Cid);
            if (heroData != null)
            {//修正所有属性
                baseAttr.Str += heroData.StrP;
                baseAttr.Def += heroData.DefP;
                baseAttr.Skl += heroData.SklP;
                baseAttr.Spd += heroData.SpdP;
                baseAttr.Hp += heroData.HpP;
                baseAttr.Mag += heroData.MagP;
                baseAttr.Luk += heroData.LukP;
                baseAttr.Mov += heroData.MovP;
                Level = heroData.Level;
            }
        }
    }
}