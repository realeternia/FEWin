using System.Drawing;
using ConfigDatas;
using FEGame.Controller.Battle.Units.Frags;
using FEGame.DataType.Samurais;

namespace FEGame.Controller.Battle.Units
{
    public abstract class BaseSam
    {
        public BattleManager BM { get; set; }

        public int Cid { get; private set; }
        public byte Level { get; protected set; }
        public byte X { get; set; }
        public byte Y { get; set; }

        protected SamAttr baseAttr; //基础属性

        protected BaseSam(int id, byte x, byte y)
        {
            Cid = id;
            X = x;
            Y = y;
            baseAttr = new SamAttr();
        }

        public virtual void Init()
        {

        }

        public virtual void Draw(Graphics g, int baseX, int baseY)
        {
            var img = SamuraiBook.GetImage(Cid);
            var cellSize = BattleManager.CellSize;
            g.DrawImage(img, X * cellSize - baseX, Y * cellSize - baseY, cellSize, cellSize);
        }
    }
}