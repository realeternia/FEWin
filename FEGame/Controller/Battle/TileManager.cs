using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using FEGame.DataType.Others;

namespace FEGame.Controller.Battle
{
    public class TileManager
    {
        public static TileManager Instance { get; private set; }

        public class PathResult
        {
            public Point NowCell;
            public Point Parent;
            public int MovLeft;
        }

        public class TileInfo
        {
            public int CId; //配置表id
            public int SamuraiId;
            public byte Camp; //阵营

            public int Cost
            {
                get
                {
                    if (SamuraiId > 0) //有人的格子无法移动
                        return 99;
                    return ConfigDatas.ConfigData.GetTileConfig(CId).MoveCost;
                }
            }
        }

        public const int CellSize = 50;
        private Image cachedMap;

        public int MapPixelWidth { get; private set; }
        public int MapPixelHeight { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private TileInfo[,] tileArray;

        public TileManager()
        {
            Instance = this;
        }

        public void Enter(byte x, byte y, int id, byte camp)
        {
            if (tileArray[x, y].SamuraiId != 0)
                throw new ApplicationException("cell used");
            tileArray[x, y].SamuraiId = id;
            tileArray[x, y].Camp = camp;
        }

        public void Leave(byte x, byte y, int id)
        {
            if (tileArray[x, y].SamuraiId != id)
                throw new ApplicationException("cell check Error");
            tileArray[x, y].SamuraiId = 0;
            tileArray[x, y].Camp = 0;
        }

        public void Init()
        {
            //50每格子
            Width = Height = 30;
            tileArray = new TileInfo[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if ((i + j) % 5 == 1)
                        tileArray[i, j] = new TileInfo {CId = 3};
                    else
                        tileArray[i, j] = new TileInfo { CId = 4};
                }
            }

            MapPixelWidth = CellSize * Width;
            MapPixelHeight = CellSize * Height;
            cachedMap = new Bitmap(MapPixelWidth, MapPixelHeight);
            Graphics g = Graphics.FromImage(cachedMap);

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Rectangle destRect = new Rectangle(CellSize * i, CellSize * j, CellSize, CellSize);

                    var tileImg = TileBook.GetTileImage(tileArray[i, j].CId, CellSize, CellSize);
                    g.DrawImage(tileImg, destRect, 0, 0, CellSize, CellSize, GraphicsUnit.Pixel);
                }
            }

            Pen myPen = new Pen(Brushes.DarkGoldenrod, 6); //描一个金边
            g.DrawRectangle(myPen, 0 + 3, 0 + 3, MapPixelWidth - 6, MapPixelHeight - 6);
            myPen.Dispose();
            g.DrawRectangle(Pens.DarkRed, 0 + 5, 0 + 5, MapPixelWidth - 10, MapPixelHeight - 10);

            g.Dispose();
        }

        public List<PathResult> GetPathResults(int x, int y, int movCount, byte myCamp)
        {
            List<PathResult> openList = new List<PathResult>();
            List<PathResult> closeList = new List<PathResult>();
            openList.Add(new PathResult {NowCell = new Point(x, y), Parent = Point.Empty, MovLeft = movCount});
            while (openList.Count > 0)
            {
                var oldOpenList = openList.ToArray();
                openList.Clear();
                foreach (var openCell in oldOpenList)
                {
                    if (openCell.NowCell.X >= Width || openCell.NowCell.X < 0 || openCell.NowCell.Y >= Height || openCell.NowCell.Y < 0)
                        continue; //非法节点
                    var closeNode = closeList.Find(p => p.NowCell.X == openCell.NowCell.X && p.NowCell.Y == openCell.NowCell.Y);
                    if (closeNode != null && closeNode.MovLeft >= openCell.MovLeft)
                        continue; //已经遍历过
                    var myCost = tileArray[openCell.NowCell.X, openCell.NowCell.Y].Cost;
                    if (HasEnemyBeside(openCell.NowCell.X, openCell.NowCell.Y, myCamp))
                        myCost+=2;
                    if (openCell.Parent == Point.Empty) //初始格不算消耗
                        myCost = 0;
                    if (openCell.MovLeft < myCost) //步数不足
                        continue;

                    if(closeNode != null)
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

                    openList.Add(new PathResult
                    {
                        NowCell = new Point(openCell.NowCell.X - 1, openCell.NowCell.Y), Parent = openCell.NowCell,
                        MovLeft = openCell.MovLeft- myCost
                    });
                    openList.Add(new PathResult
                    {
                        NowCell = new Point(openCell.NowCell.X + 1, openCell.NowCell.Y),
                        Parent = openCell.NowCell,
                        MovLeft = openCell.MovLeft- myCost
                    });
                    openList.Add(new PathResult
                    {
                        NowCell = new Point(openCell.NowCell.X, openCell.NowCell.Y - 1),
                        Parent = openCell.NowCell,
                        MovLeft = openCell.MovLeft- myCost
                    });
                    openList.Add(new PathResult
                    {
                        NowCell = new Point(openCell.NowCell.X, openCell.NowCell.Y + 1),
                        Parent = openCell.NowCell,
                        MovLeft = openCell.MovLeft- myCost
                    });
                }
            }

            return closeList;
        }

        private bool HasEnemyBeside(int x, int y, byte myCamp)
        {
            return CheckCellEnemy(x - 1, y, myCamp) || CheckCellEnemy(x + 1, y, myCamp) ||
                   CheckCellEnemy(x, y - 1, myCamp) || CheckCellEnemy(x, y + 1, myCamp);
        }

        private bool CheckCellEnemy(int x, int y, byte myCamp)
        {
            if (x >= Width || x < 0)
                return false;
            if (y >= Height || y < 0)
                return false;
            var targetCamp = tileArray[x, y].Camp;
            return targetCamp > 0 && targetCamp != myCamp;
        }

        public void Draw(Graphics g, int baseX, int baseY, int panelW, int panelH)
        {
            g.DrawImage(cachedMap, new Rectangle(0, 0, panelW, panelH),
                new Rectangle(baseX, baseY, panelW, panelH), GraphicsUnit.Pixel);
        }
    }
}