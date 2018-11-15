﻿namespace ConfigDatas
{
    public interface IMonsterAction
    {
        bool IsNight { get; }
        bool IsTileMatching { get; }
        bool IsElement(string ele);
        bool IsRace(string rac);

        void Disappear();
        void AddBuff(int buffId, int blevel, double dura);
        void AddItem(int itemId);//战斗中
        void Transform(int monId);
        void ChangeAI(string type); //修正ai模式
        void SetPrepareTime(float rate);

        void Return(int costChange);

        bool HasWeapon();
        void AddWeapon(int weaponId, int lv);
        void StealWeapon(IMonster target);
        void BreakWeapon();
        void WeaponReturn();
        void LevelUpWeapon(int lv);
        void AddRandSkill();
        void AddSkill(int id, int rate);
        int GetMonsterCountByRace(int rid);
        int GetMonsterCountByType(int type);
        void AddMissile(IMonster target, int attr, double damage, string arrow);
       
        void ClearDebuff();
        void ExtendDebuff(double count);
        bool HasBuff(int id);
        void SetToPosition(string type, int step);

        void SuddenDeath();
        void Rebel();//造反

        void Summon(string type, int id, int count);
        void SummonRandomAttr(string type, int attr, int star);
        void SummonRandomRace(string type, int race, int star);
        void MadDrug(); //交换攻击和血量
        void CureRandomAlien(double rate);
        void EatTomb(IMonster tomb);
        void Silent();
        IMonsterAuro AddAuro(ISkill skill);
        void AddPArmor(double val);
        void AddMArmor(double val);
        int GetPArmor();

        void AddAttrModify(string tp, int itemId, string attr, double val);
        void AddMaxHp(string tp, int itemId, double val);
    }
}