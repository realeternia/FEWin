using System.Drawing;
using ConfigDatas;
using FEGame.DataType.Samurais;

namespace FEGame.Controller.Battle.Units
{
    public abstract class BaseSam
    {
        public BattleManager BM { get; set; }

        public int Cid { get; private set; }
        public byte X { get; set; }
        public byte Y { get; set; }

        protected BaseSam(int id, byte x, byte y)
        {
            Cid = id;
            X = x;
            Y = y;
        }

        public virtual void Draw(Graphics g, int baseX, int baseY)
        {
            var img = SamuraiBook.GetImage(Cid);
            var cellSize = BattleManager.CellSize;
            g.DrawImage(img, X * cellSize - baseX, Y * cellSize - baseY, cellSize, cellSize);
        }
    }
}