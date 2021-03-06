﻿using System.Collections.Generic;
using System.Drawing;

namespace FEGame.Controller.Battle
{
    public class TileAdapter
    {
        private int width;
        private int height;

        public TileAdapter()
        {
            width = TileManager.Instance.Width;
            height = TileManager.Instance.Height;
        }

        public List<TileManager.PathResult> GetPathAttack(int x, int y, int atkCount, byte myCamp)
        {
            List<TileManager.PathResult> openList = new List<TileManager.PathResult>();
            List<TileManager.PathResult> closeList = new List<TileManager.PathResult>();
            var srcPoint = new TileManager.PathResult {NowCell = new Point(x, y), Parent = new Point(-1, -1), MovLeft = atkCount};
            openList.Add(srcPoint);
            while (openList.Count > 0)
            {
                var oldOpenList = openList.ToArray();
                openList.Clear();
                foreach (var openCell in oldOpenList)
                {
                    if (openCell.NowCell.X >= width || openCell.NowCell.X < 0 || openCell.NowCell.Y >= height || openCell.NowCell.Y < 0)
                        continue; //非法节点
                    var closeNode = closeList.Find(p => p.NowCell.X == openCell.NowCell.X && p.NowCell.Y == openCell.NowCell.Y);
                    if (closeNode != null && closeNode.MovLeft >= openCell.MovLeft)
                        continue; //已经遍历过
                    var myCost = 1; //攻击射程固定消耗1点
                    if (openCell.Parent.X < 0) //初始格不算消耗
                        myCost = 0;
                    if (openCell.MovLeft < myCost) //步数不足
                        continue;

                    if (closeNode != null)
                    {
                        closeNode.MovLeft = openCell.MovLeft;
                        closeNode.Parent = openCell.Parent;
                    }
                    else
                    {
                        closeList.Add(openCell);
                    }

                    if (openCell.MovLeft <= 0)
                        continue;

                    DetectNearby(openList, openCell, myCost);
                }
            }

            closeList.Remove(srcPoint);
            closeList.RemoveAll(node => TileManager.Instance.GetTile(node.NowCell.X, node.NowCell.Y).Camp == myCamp); //把同阵营剔除

            return closeList;
        }

        public List<TileManager.PathResult> GetPathMove(int x, int y, int movCount, byte myCamp)
        {
            List<TileManager.PathResult> openList = new List<TileManager.PathResult>();
            List<TileManager.PathResult> closeList = new List<TileManager.PathResult>();
            openList.Add(new TileManager.PathResult { NowCell = new Point(x, y), Parent = new Point(-1, -1), MovLeft = movCount });
            while (openList.Count > 0)
            {
                var oldOpenList = openList.ToArray();
                openList.Clear();
                foreach (var openCell in oldOpenList)
                {
                    if (openCell.NowCell.X >= width || openCell.NowCell.X < 0 || openCell.NowCell.Y >= height || openCell.NowCell.Y < 0)
                        continue; //非法节点
                    var closeNode = closeList.Find(p => p.NowCell.X == openCell.NowCell.X && p.NowCell.Y == openCell.NowCell.Y);
                    if (closeNode != null && closeNode.MovLeft >= openCell.MovLeft)
                        continue; //已经遍历过
                    var myCost = TileManager.Instance.GetTile(openCell.NowCell.X, openCell.NowCell.Y).Cost;
                    if (HasEnemyBeside(openCell.NowCell.X, openCell.NowCell.Y, myCamp))
                        myCost += 2;
                    if (openCell.Parent.X < 0) //初始格不算消耗
                        myCost = 0;
                    if (openCell.MovLeft < myCost) //步数不足
                        continue;

                    if (closeNode != null)
                    {
                        closeNode.MovLeft = openCell.MovLeft;
                        closeNode.Parent = openCell.Parent;
                    }
                    else
                    {
                        closeList.Add(openCell);
                    }

                    if (openCell.MovLeft <= 0)
                        continue;

                    DetectNearby(openList, openCell, myCost);
                }
            }

            return closeList;
        }

        private static void DetectNearby(List<TileManager.PathResult> openList, TileManager.PathResult checkCell, int moveCost)
        {
            openList.Add(new TileManager.PathResult
            {
                NowCell = new Point(checkCell.NowCell.X - 1, checkCell.NowCell.Y),
                Parent = checkCell.NowCell,
                MovLeft = checkCell.MovLeft - moveCost
            });
            openList.Add(new TileManager.PathResult
            {
                NowCell = new Point(checkCell.NowCell.X + 1, checkCell.NowCell.Y),
                Parent = checkCell.NowCell,
                MovLeft = checkCell.MovLeft - moveCost
            });
            openList.Add(new TileManager.PathResult
            {
                NowCell = new Point(checkCell.NowCell.X, checkCell.NowCell.Y - 1),
                Parent = checkCell.NowCell,
                MovLeft = checkCell.MovLeft - moveCost
            });
            openList.Add(new TileManager.PathResult
            {
                NowCell = new Point(checkCell.NowCell.X, checkCell.NowCell.Y + 1),
                Parent = checkCell.NowCell,
                MovLeft = checkCell.MovLeft - moveCost
            });
        }

        private bool HasEnemyBeside(int x, int y, byte myCamp)
        {
            return CheckCellEnemy(x - 1, y, myCamp) || CheckCellEnemy(x + 1, y, myCamp) ||
                   CheckCellEnemy(x, y - 1, myCamp) || CheckCellEnemy(x, y + 1, myCamp);
        }

        private bool CheckCellEnemy(int x, int y, byte myCamp)
        {
            if (x >= width || x < 0)
                return false;
            if (y >= height || y < 0)
                return false;
            var targetCamp = TileManager.Instance.GetTile(x, y).Camp;
            return targetCamp > 0 && targetCamp != myCamp;
        }
    }
}