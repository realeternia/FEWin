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
            public int UnitId; //单位唯一id
            public byte Camp; //阵营

            public int Cost
            {
                get
                {
                    if (UnitId > 0) //有人的格子无法移动
                        return 99;
                    return ConfigDatas.ConfigData.GetTileConfig(CId).MoveCost;
                }
            }
        }

        public const int CellSize = 60;
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
            if (tileArray[x, y].UnitId != 0)
                throw new ApplicationException("cell used");
            tileArray[x, y].UnitId = id;
            tileArray[x, y].Camp = camp;
        }

        public void Leave(byte x, byte y, int id)
        {
            if (tileArray[x, y].UnitId != id)
                throw new ApplicationException("cell check Error");
            tileArray[x, y].UnitId = 0;
            tileArray[x, y].Camp = 0;
        }

        public void Move(byte sx, byte sy, byte tx, byte ty, int id, byte camp)
        {
            Leave(sx, sy, id);
            Enter(tx, ty, id, camp);
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

        public TileInfo GetTile(int x, int y)
        {
            return tileArray[x, y];
        }


        public void Draw(Graphics g, int baseX, int baseY, int panelW, int panelH)
        {
            g.DrawImage(cachedMap, new Rectangle(0, 0, panelW, panelH),
                new Rectangle(baseX, baseY, panelW, panelH), GraphicsUnit.Pixel);
        }
    }
}