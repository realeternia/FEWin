using FEGame.Core;

namespace FEGame.DataType.User.Db
{
    public class DbHeroAttr
    {
        [FieldIndex(Index = 1)] public int SamuraiId; //英雄id
        [FieldIndex(Index = 2)] public byte Level; //当前等级

        [FieldIndex(Index = 11)] public byte StrP; //力量加成值
        [FieldIndex(Index = 12)] public byte DefP;
        [FieldIndex(Index = 13)] public byte SpdP;
        [FieldIndex(Index = 14)] public byte SklP;
        [FieldIndex(Index = 15)] public byte MagP;
        [FieldIndex(Index = 16)] public byte LukP;
        [FieldIndex(Index = 17)] public byte MovP;
        [FieldIndex(Index = 18)] public byte HpP;
    }
}