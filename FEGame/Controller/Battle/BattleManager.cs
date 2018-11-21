using System.Collections.Generic;
using FEGame.Controller.Battle.Units;

namespace FEGame.Controller.Battle
{
    public class BattleManager
    {
        private List<BaseUnit> unitList = new List<BaseUnit>();

        public BattleManager()
        {
        }

        public void AddUnit(BaseUnit bu)
        {
            unitList.Add(bu);
        }
    }
}