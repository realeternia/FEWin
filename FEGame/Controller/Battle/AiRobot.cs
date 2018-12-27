using System.Collections.Generic;
using FEGame.Controller.Battle.Units;
using FEGame.Forms;

namespace FEGame.Controller.Battle
{
    internal class AiRobot
    {
        private BattleManager battleManager;
        private BattleForm.AttackAction actAttack;
        private BattleForm.MoveAction actMove;
        private BattleForm.StopAction actStop;

        private int actionUnitId;
        private int mostDangerId;

        public AiRobot(BattleManager manager, BattleForm.MoveAction act2, BattleForm.AttackAction act1, BattleForm.StopAction act3)
        {
            battleManager = manager;
            actAttack = act1;
            actMove = act2;
            actStop = act3;
        }

        public void OnTick()
        {
            if (actionUnitId > 0)
            {
                var actUnit = battleManager.GetSam(actionUnitId);
                if (actUnit.IsFinished)
                    actionUnitId = 0;
                else
                    return;
            }

            var checkOne = battleManager.GetOneActive(ConfigDatas.CampConfig.Indexer.Wild);
            if (checkOne == null)
                return;

            mostDangerId = 0;
            actionUnitId = checkOne.Id;

            CheckUnitAction(checkOne);
        }

        public void CheckUnitAction(BaseSam unit)
        {
            var adapter = new TileAdapter();
            var savedPath = adapter.GetPathMove(unit.X, unit.Y, unit.Mov, unit.Camp);
            Dictionary<int, int> tileDangerDict = new Dictionary<int, int>(); //x+1000*y

            mostDangerId = 0;
            int mostDangerMark = 0;
            List<int> mostDangerPathList = new List<int>();

            foreach (var pathResult in savedPath)
            {
                var tileId = pathResult.NowCell.X + pathResult.NowCell.Y * 1000;
                var enemyUnits = battleManager.GetAllUnits(ConfigDatas.CampConfig.Indexer.Reborn);
                foreach (var enemy in enemyUnits)
                {
                    var dis = enemy.GetDistanceFrom(pathResult.NowCell.X, pathResult.NowCell.Y);
                    if (dis <= unit.Range) //射程内
                    {
                        var nowDanger = 10000 / (enemy.Level * enemy.LeftHp);
                        if (enemy.Id == mostDangerId)
                        {
                            mostDangerPathList.Add(tileId);
                        }
                        else if (enemy.Id != mostDangerId && nowDanger > mostDangerMark)
                        {
                            mostDangerMark = nowDanger;
                            mostDangerId = enemy.Id;
                            mostDangerPathList.Clear();
                            mostDangerPathList.Add(tileId);
                        }
                    }

                    if (!tileDangerDict.ContainsKey(tileId))
                        tileDangerDict[tileId] = 0;
                    tileDangerDict[tileId] += 10000 / (enemy.Level * enemy.LeftHp + dis*dis*3);
                }
            }

            if (mostDangerId > 0) //有目标
            {
                mostDangerPathList.Sort((a, b) => tileDangerDict[b] - tileDangerDict[a]);

                var x = mostDangerPathList[0] % 1000;
                var y = mostDangerPathList[0] / 1000;
                actMove(unit.Id, x, y, savedPath, OnUnitMovedAndAttack);
            }
            else
            {
                List<int> tileList = new List<int>(tileDangerDict.Keys); //所有格子排序
                tileList.Sort((a, b) => tileDangerDict[b] - tileDangerDict[a]);

                var x = tileList[0] % 1000;
                var y = tileList[0] / 1000;
                actMove(unit.Id, x, y, savedPath, OnUnitMoved);
            }
          
        }

        public void OnUnitMoved(int x, int y)
        {
            actStop(actionUnitId);
            actionUnitId = 0;
        }

        public void OnUnitMovedAndAttack(int x, int y)
        {
            actAttack(actionUnitId, mostDangerId);
            actionUnitId = 0;
        }
    }
}