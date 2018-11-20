﻿using System.Drawing;
using FEGame.Core.Loader;

namespace FEGame.Forms.Items.Core
{
    internal class BorderPainter
    {
        private static Image top;
        private static Image bottom;
        private static Image left;
        private static Image right;
        private static Image topleft;
        private static Image topright;
        private static Image bottomleft;
        private static Image bottomright;

        static BorderPainter()
        {
            top = PicLoader.Read("Border", "BaseT.png");
            bottom = PicLoader.Read("Border", "BaseB.png");
            left = PicLoader.Read("Border", "BaseL.png");
            right = PicLoader.Read("Border", "BaseR.png");
            topleft = PicLoader.Read("Border", "BaseTL.png");
            topright = PicLoader.Read("Border", "BaseTR.png");
            bottomleft = PicLoader.Read("Border", "BaseBL.png");
            bottomright = PicLoader.Read("Border", "BaseBR.png");
        }

        public static void Draw(Graphics g, string head, int width, int height)
        {
            g.DrawImage(topleft, 0, 0, 50, 50);
            DrawMore(g, top, 50, 0, width - 100, 50);
            g.DrawImage(topright, width - 50, 0, 50, 50);
            DrawMore(g, right, width - 50, 50, 50, height - 100);
            g.DrawImage(bottomright, width - 50, height - 50, 50, 50);
            DrawMore(g, bottom, 50, height - 50, width - 100, 50);
            g.DrawImage(bottomleft, 0, height - 50, 50, 50);
            DrawMore(g, left, 0, 50, 50, height - 100);

            g.FillRectangle(Brushes.Black, 50, 50, width - 100, height - 100);

            if (head != "")
            {
                Image headimg = PicLoader.Read("System.Headline", string.Format("{0}.png", head));
                int wd = (width - 100)*4/5;
                g.DrawImage(headimg, (width - wd)/2, 0, wd, headimg.Height);
                headimg.Dispose();
            }
        }

        private static void DrawMore(Graphics g, Image img, int x, int y, int width, int height)
        {
            if (img.Width < width)
            {
                int leftW = width;
                while (leftW > 0)
                {
                    if (leftW > img.Width)
                    {
                        g.DrawImage(img, x + width - leftW, y, img.Width, img.Height);
                        leftW -= img.Width;
                    }
                    else
                    {
                        g.DrawImage(img, new Rectangle(x + width - leftW, y, leftW, img.Height), new Rectangle(0, 0, leftW, img.Height), GraphicsUnit.Pixel);
                        leftW = 0;
                    }
                }
            }
            else if (img.Height < height)
            {
                int leftH = height;
                while (leftH > 0)
                {
                    if (leftH > img.Height)
                    {
                        g.DrawImage(img, x, y + height - leftH, img.Width, img.Height);
                        leftH -= img.Height;
                    }
                    else
                    {
                        g.DrawImage(img, new Rectangle(x, y + height - leftH, img.Width, leftH), new Rectangle(0, 0, img.Width, leftH), GraphicsUnit.Pixel);
                        leftH = 0;
                    }
                }
            }
        }
    }
}
