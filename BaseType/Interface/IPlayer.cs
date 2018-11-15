using System;

namespace ConfigDatas
{
    public interface IPlayer
    {
        IPlayerAction Action { get; }
        IMonster Tower { get; }
        bool IsLeft{get;}
        int Job { get; }
        float Mp { get; }
        float Lp { get; }
        float Pp { get; }
        bool Combo { get; }
        bool IsLastSpellAttr(int monAttr);

        void AddMp(double addon);//spellʹ��
        void AddLp(double addon);
        void AddPp(double addon);
        void AddMana(IMonster mon, int type, double addon);//skillʹ��
        int CardNumber { get; }
        int Luk { get; }
    }
}