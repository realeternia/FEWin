using System.Drawing;
using FEGame.Core.Loader;

namespace FEGame.Controller.Battle
{
    public class TileManager
    {
        public const int CellSize = 50;
        private Image worldMap;

        public int Width { get; set; }
        public int Height { get; set; }

        public void Init()
        {
            //50每格子
            Width = CellSize * 30;
            Height = CellSize * 30;
            worldMap = new Bitmap(CellSize * 30, CellSize * 30);
            Graphics g = Graphics.FromImage(worldMap);

            var tileImage = PicLoader.Read("Tiles", "1.jpg");
            var tileImage2 = PicLoader.Read("Tiles", "2.jpg");
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    Rectangle destRect = new Rectangle(CellSize * i, CellSize * j, CellSize, CellSize);
                    if ((i + j) % 5 == 1)
                        g.DrawImage(tileImage2, destRect, 0, 0, CellSize, CellSize, GraphicsUnit.Pixel);
                    else
                        g.DrawImage(tileImage, destRect, 0, 0, CellSize, CellSize, GraphicsUnit.Pixel);
                }
            }
            tileImage.Dispose();
            tileImage2.Dispose();

            Pen myPen = new Pen(Brushes.DarkGoldenrod, 6); //描一个金边
            g.DrawRectangle(myPen, 0 + 3, 0 + 3, Width - 6, Height - 6);
            myPen.Dispose();
            g.DrawRectangle(Pens.DarkRed, 0 + 5, 0 + 5, Width - 10, Height - 10);

            g.Dispose();
        }

        public void Draw(Graphics g, int baseX, int baseY, int panelW, int panelH)
        {
            g.DrawImage(worldMap, new Rectangle(0, 0, panelW, panelH),
                new Rectangle(baseX, baseY, panelW, panelH), GraphicsUnit.Pixel);
        }
    }
}