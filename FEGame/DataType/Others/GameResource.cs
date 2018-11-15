using System;
using FEGame.Core;

namespace FEGame.DataType.Others
{
    public class GameResource
    {
        [FieldIndex(Index = 1)]
        public uint Gold;
        [FieldIndex(Index = 2)]
        public uint Lumber; //����������ũ��
        [FieldIndex(Index = 3)]
        public uint Stone; //��������
        [FieldIndex(Index = 4)]
        public uint Mercury; //����ף������������
        [FieldIndex(Index = 5)]
        public uint Carbuncle;//������￨����¸����
        [FieldIndex(Index = 6)]
        public uint Sulfur; //ˢ�� ����(��ʵ��)
        [FieldIndex(Index = 7)]
        public uint Gem; //����������

        public GameResource()
            : this(0, 0, 0, 0, 0, 0, 0)
        {
        }

        public GameResource(uint gold, uint lumber, uint stone, uint mercury, uint carbuncle, uint sulfur, uint gem)
        {
            Gold = gold;
            Lumber = lumber;
            Stone = stone;
            Mercury = mercury;
            Carbuncle = carbuncle;
            Sulfur = sulfur;
            Gem = gem;
        }

        internal void Add(GameResourceType type, uint value)
        {
            switch ((int)type)
            {
                case 0: Gold += value; break;
                case 1: Lumber += value; break;
                case 2: Stone += value; break;
                case 3: Mercury += value; break;
                case 4: Carbuncle += value; break;
                case 5: Sulfur += value; break;
                case 6: Gem += value; break;
            }
        }

        internal void Sub(GameResourceType type, uint value)
        {
            switch ((int)type)
            {
                case 0: Gold = (uint)Math.Max(0, (long)Gold - value); break;
                case 1: Lumber = (uint)Math.Max(0, (long)Lumber - value); break;
                case 2: Stone = (uint)Math.Max(0, (long)Stone - value); break;
                case 3: Mercury = (uint)Math.Max(0, (long)Mercury - value); break;
                case 4: Carbuncle = (uint)Math.Max(0, (long)Carbuncle - value); break;
                case 5: Sulfur = (uint)Math.Max(0, (long)Sulfur - value); break;
                case 6: Gem = (uint)Math.Max(0, (long)Gem - value); break;
            }
        }

        internal uint Get(GameResourceType type)
        {
            switch ((int)type)
            {
                case 0: return Gold;
                case 1: return Lumber;
                case 2: return Stone;
                case 3: return Mercury;
                case 4: return Carbuncle;
                case 5: return Sulfur;
                case 6: return Gem;
            }
            return 0;
        }

        internal bool Has(GameResourceType type, int value)
        {
            switch ((int)type)
            {
                case 0: return Gold >= value;
                case 1: return Lumber >= value;
                case 2: return Stone >= value;
                case 3: return Mercury >= value;
                case 4: return Carbuncle >= value;
                case 5: return Sulfur >= value;
                case 6: return Gem >= value;
            }
            return false;
        }

        public static GameResource Parse(string str)
        {
            GameResource res = new GameResource();
            string[] datas = str.Split('|');
            foreach (string data in datas)
            {
                if (data == "")
                    break;

                string[] infos = data.Split(';');

                uint value = uint.Parse(infos[1]);
                switch (int.Parse(infos[0]))
                {
                    case 1: res.Gold += value; break;
                    case 2: res.Lumber += value; break;
                    case 3: res.Stone += value; break;
                    case 4: res.Mercury += value; break;
                    case 5: res.Carbuncle += value; break;
                    case 6: res.Sulfur += value; break;
                    case 7: res.Gem += value; break;
                }
            }
            return res;
        }

        public uint[] ToArray()
        {
            uint[] rt = new uint[7];
            rt[0] = Gold;
            rt[1] = Lumber;
            rt[2] = Stone;
            rt[3] = Mercury;
            rt[4] = Carbuncle;
            rt[5] = Sulfur;
            rt[6] = Gem;
            return rt;
        }
    }

}
