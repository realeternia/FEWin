using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using FEGame.DataType.Others;

namespace FEGame.Controller.Battle
{
    public class TileManager
    {
        public class PathResult
        {
            public Point NowCell;
            public Point Parent;
            public int MovLeft;
        }

        public const int CellSize = 50;
        private Image cachedMap;

        public int MapPixelWidth { get; private set; }
        public int MapPixelHeight { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private int[,] tileArray;

        public void Init()
        {
            //50每格子
            Width = Height = 30;
            tileArray = new int[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if ((i + j) % 5 == 1)
                        tileArray[i,j] = 3;
                    else
                        tileArray[i, j] = 4;
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
                    Rectangle destRect = new Rectangle(CellSize * i, CellSize * j, CellSize/2, CellSize/2);

                    Pen b = new Pen(Color.FromName(ConfigData.GetTileConfig(tileArray[i, j]).Color)); //测试用，画一个区域
                    g.DrawRectangle(b, destRect);
                    b.Dispose();

                    //var tileImg = TileBook.GetTileImage(tileArray[i, j], CellSize, CellSize);
                    //g.DrawImage(tileImg, destRect, 0, 0, CellSize, CellSize, GraphicsUnit.Pixel);
                }
            }

            Pen myPen = new Pen(Brushes.DarkGoldenrod, 6); //描一个金边
            g.DrawRectangle(myPen, 0 + 3, 0 + 3, MapPixelWidth - 6, MapPixelHeight - 6);
            myPen.Dispose();
            g.DrawRectangle(Pens.DarkRed, 0 + 5, 0 + 5, MapPixelWidth - 10, MapPixelHeight - 10);

            g.Dispose();
        }

        public List<PathResult> GetPathResults(int x, int y, int movCount)
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
                    var myCost = ConfigDatas.ConfigData.GetTileConfig(tileArray[openCell.NowCell.X, openCell.NowCell.Y]).MoveCost;
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

        public void Draw(Graphics g, int baseX, int baseY, int panelW, int panelH)
        {
            g.DrawImage(cachedMap, new Rectangle(0, 0, panelW, panelH),
                new Rectangle(baseX, baseY, panelW, panelH), GraphicsUnit.Pixel);
        }
    }
}