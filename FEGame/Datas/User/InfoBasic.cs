using System;
using FEGame.Core;
using FEGame.Datas.Others;
using FEGame.Forms.CMain;
using FEGame.Rpc;

namespace FEGame.Datas.User
{
    public class InfoBasic
    {
        [FieldIndex(Index = 3)] public int Head;
        [FieldIndex(Index = 5)] public uint Dna; //可以影响sq的选项，影响sq的出现概率（未实现）
        [FieldIndex(Index = 7)] public byte Level;
        [FieldIndex(Index = 16)] public int Exp;
        [FieldIndex(Index = 17)] public int MapId;
        [FieldIndex(Index = 19)] public int LastLoginTime;
        [FieldIndex(Index = 20)] public uint FoodPoint; //饱腹值
        [FieldIndex(Index = 23)] public int Position;
        [FieldIndex(Index = 24)] public uint HealthPoint; //健康度
        [FieldIndex(Index = 25)] public uint MentalPoint; //精神

        public InfoBasic()
        {
        }

        public void AddExp(int ex)
        {
            if (Level >= ExpTree.MaxLevel)
                return;

            Exp += ex;
            MainTipManager.AddTip(string.Format("|获得|Cyan|{0}||点经验值", ex), "White");

            if (Exp >= ExpTree.GetNextRequired(Level))
            {
                int oldLevel = Level;
                while (CheckNewLevel()) //循环升级
                    OnLevel(Level);

                SystemMenuManager.ResetIconState();
                MainForm.Instance.RefreshView();
            }

            TalePlayer.C2SSender.UpdateLevelExp(0, Level, Exp);
        }

        public void AddFood(uint val)
        {
            FoodPoint += val;
            if (FoodPoint >= 100)
                FoodPoint = 100;
        }
        public void SubFood(uint val)
        {
            if (val > FoodPoint)
                FoodPoint = 0;
            else
                FoodPoint -= val;
        }

        public void AddHealth(uint val)
        {
            HealthPoint += val;
            if (HealthPoint >= 100)
                HealthPoint = 100;
        }
        public void SubHealth(uint val)
        {
            if (val > HealthPoint)
                HealthPoint = 0;
            else
                HealthPoint -= val;
        }
        public void AddMental(uint val)
        {
            MentalPoint += val;
            if (MentalPoint >= 100)
                MentalPoint = 100;
        }
        public void SubMental(uint val)
        {
            if (val > MentalPoint)
                MentalPoint = 0;
            else
                MentalPoint -= val;
        }

        private bool CheckNewLevel()
        {
            int expNeed = ExpTree.GetNextRequired(Level);
            if (Exp >= expNeed)
            {
                Exp -= expNeed;
                Level++;
                return true;
            }
            return false;
        }

        private void OnLevel(int lv)
        {

        }

        public bool HasDna(int dna)
        {
            int dnaData = (int)Math.Pow(2, dna);
            return (Dna & dnaData) == dnaData;
        }
    }
}
