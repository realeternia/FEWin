using System.Collections.Generic;
using System.Drawing;
using FEGame.Controller.Battle.Units;

namespace FEGame.Controller.Battle
{
    public class BattleManager
    {
        public const int CellSize = 50;
        private List<BaseSam> unitList = new List<BaseSam>();

        public BattleManager()
        {
        }

        public void AddUnit(BaseSam bu)
        {
            unitList.Add(bu);
            bu.BM = this;
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