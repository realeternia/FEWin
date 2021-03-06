﻿using System.Collections.Generic;
using System.Drawing;
using FEGame.Controller.Battle.Units;

namespace FEGame.Controller.Battle
{
    public class BattleManager
    {
        public static BattleManager Instance { get; private set; }

        private List<BaseSam> unitList = new List<BaseSam>();
        private int unitIdOffset = 1000;

        public BattleManager()
        {
            Instance = this;
        }

        public void AddUnit(BaseSam bu)
        {
            bu.Id = unitIdOffset++;
            bu.BM = this;
            bu.Init();
            unitList.Add(bu);
            TileManager.Instance.Enter(bu.X, bu.Y, bu.Id, bu.Camp);
        }

        public void RemoveUnit(BaseSam bu)
        {
            bu.OnRemove();
            TileManager.Instance.Leave(bu.X, bu.Y, bu.Id);
            unitList.Remove(bu);
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

        public bool IsAllUnitsFinsh(int camp)
        {
            return unitList.Find(u => u.Camp == camp && !u.IsFinished) == null;
        }

        public BaseSam GetOneActive(int camp)
        {
            return unitList.Find(u => u.Camp == camp && !u.IsFinished);
        }
        public List<BaseSam> GetAllUnits(int camp)
        {
            return unitList.FindAll(u => u.Camp == camp);
        }

        public void Draw(Graphics g, int baseX, int baseY, int movingId)
        {
            foreach (var baseUnit in unitList)
            {
                if (baseUnit.Id == movingId) //移动单位不绘制
                    continue;
                baseUnit.Draw(g, baseX, baseY);
            }
        }
    }
}