using System.Collections.Generic;
using System.Drawing;
using FEGame.Controller.Battle.Units;

namespace FEGame.Controller.Battle
{
    public class BattleManager
    {
        private List<BaseSam> unitList = new List<BaseSam>();
        private int unitIdOffset = 1000;

        public BattleManager()
        {
        }

        public void AddUnit(BaseSam bu)
        {
            bu.Id = unitIdOffset++;
            bu.BM = this;
            bu.Init();
            unitList.Add(bu);
        }

        public BaseSam GetSam(int id)
        {
            return unitList.Find(u => u.Id == id);
        }

        public int GetRegionUnitId(int baseX, int baseY)
        {
            foreach (var baseUnit in unitList)
            {
                if (baseX >= baseUnit.X * TileManager.CellSize && baseX < baseUnit.X * TileManager.CellSize + TileManager.CellSize &&
                    baseY >= baseUnit.Y * TileManager.CellSize && baseY < baseUnit.Y * TileManager.CellSize + TileManager.CellSize)
                    return baseUnit.Id;
            }

            return 0;
        }

        public void Draw(Graphics g, int baseX, int baseY)
        {
            foreach (var baseUnit in unitList)
            {
                baseUnit.Draw(g, baseX, baseY);
            }
        }
    }
}