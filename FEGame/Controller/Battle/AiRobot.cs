using System.Collections.Generic;
using FEGame.Controller.Battle.Units;

namespace FEGame.Controller.Battle
{
    public class AiRobot
    {
        private BattleManager battleManager;

        public AiRobot(BattleManager manager)
        {
            battleManager = manager;
        }

        public void OnTick()
        {
            var checkOne = battleManager.GetOneActive(ConfigDatas.CampConfig.Indexer.Wild);
            if (checkOne == null)
                return;

            CheckUnitAction(checkOne);
        }

        public void CheckUnitAction(BaseSam unit)
        {
            var adapter = new TileAdapter();
            var savedPath = adapter.GetPathMove(unit.X, unit.Y, unit.Mov, unit.Camp);
            Dictionary<int, int> tileDangerDict = new Dictionary<int, int>(); //x+1000*y

            int mostDangerId = 0;
            int mostDangerMark = 0;
            List<int> mostDangerPathList = new List<int>();

            foreach (var pathResult in savedPath)
            {
                var tileId = pathResult.NowCell.X + pathResult.NowCell.Y * 1000;
                var enemyUnits = battleManager.GetAllUnits(ConfigDatas.CampConfig.Indexer.Reborn);
                foreach (var enemy in enemyUnits)
                {
                    var dis = enemy.GetDistanceFrom(unit.X, unit.Y);
                    if (enemy.GetDistanceFrom(unit.X, unit.Y) <= unit.Range) //射程内
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
                    tileDangerDict[tileId] += 10000 / (enemy.Level * enemy.LeftHp + dis*dis);
                }
            }

            if (mostDangerId > 0) //有目标
            {
                mostDangerPathList.Sort((a, b) => tileDangerDict[b] - tileDangerDict[a]);
            }
            else
            {
                List<int> tileList = new List<int>(tileDangerDict.Keys); //所有格子排序
                tileList.Sort((a, b) => tileDangerDict[b] - tileDangerDict[a]);
            }
            
        }
    }
}