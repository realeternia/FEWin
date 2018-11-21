using System;
using System.Drawing;
using System.Windows.Forms;
using FEGame.Controller.Battle;
using FEGame.Core;
using FEGame.Core.Loader;
using FEGame.Forms.Items.Core;
using NarlonLib.Math;

namespace FEGame.Forms
{
    internal sealed partial class BattleForm : BasePanel
    {
        private bool showImage;
        private int baseX = 900;
        private int baseY = 250;
        private int selectTargetId;
        private Image worldMap; 
        private HSCursor myCursor;

        private BattleManager battleManager;

        public BattleForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "closebutton1.jpg");
            bitmapButtonClose.NoUseDrawNine = true;
            myCursor = new HSCursor(this);


            battleManager = new BattleManager();
            //test code
            battleManager.AddUnit(new Controller.Battle.Units.HeroSam(43020101, 15, 15));
            battleManager.AddUnit(new Controller.Battle.Units.HeroSam(43020102, 15, 13));
            battleManager.AddUnit(new Controller.Battle.Units.MonsterSam(43000005, 18, 18));
            battleManager.AddUnit(new Controller.Battle.Units.MonsterSam(43000005, 18, 21));
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            //50每格子
            worldMap = new Bitmap(BattleManager.CellSize * 30, BattleManager.CellSize * 30);
            Graphics g = Graphics.FromImage(worldMap);

            var tileImage = PicLoader.Read("Tiles", "1.jpg");
            var tileImage2 = PicLoader.Read("Tiles", "2.jpg");
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    Rectangle destRect = new Rectangle(BattleManager.CellSize * i, BattleManager.CellSize * j, BattleManager.CellSize, BattleManager.CellSize);
                    if ((i + j) % 5 == 1)
                        g.DrawImage(tileImage2, destRect, 0, 0, BattleManager.CellSize, BattleManager.CellSize, GraphicsUnit.Pixel);
                    else
                        g.DrawImage(tileImage, destRect, 0, 0, BattleManager.CellSize, BattleManager.CellSize, GraphicsUnit.Pixel);
                }
            }
            tileImage.Dispose();
            tileImage2.Dispose();

            Pen myPen = new Pen(Brushes.DarkGoldenrod, 6); //描一个金边
            g.DrawRectangle(myPen, 0+3, 0+3, 1500-6, 1500-6);
            myPen.Dispose();
            g.DrawRectangle(Pens.DarkRed, 0+5, 0+5, 1500-10, 1500-10);

            g.Dispose();

            baseX = MathTool.Clamp(baseX, 0, worldMap.Width - doubleBuffedPanel1.Width);
            baseY = MathTool.Clamp(baseY, 0, worldMap.Height - doubleBuffedPanel1.Height);
            
            showImage = true;
        }

        public override void OnFrame(int tick, float timePass)
        {
            //if ((tick % 10) == 0)
            //{
            //    doubleBuffedPanel1.Invalidate();
            //}
        }

        private void BattleForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHold)
            {
                if (MathTool.GetDistance(e.Location, dragStartPos)>3)
                {
                    baseX -= e.Location.X-dragStartPos.X;
                    baseY -= e.Location.Y - dragStartPos.Y;
                    baseX = MathTool.Clamp(baseX, 0, worldMap.Width - doubleBuffedPanel1.Width);
                    baseY = MathTool.Clamp(baseY, 0, worldMap.Height - doubleBuffedPanel1.Height);
                    dragStartPos = e.Location;
                    doubleBuffedPanel1.Invalidate();
                }
                dragStartPos = e.Location;
            }
            else
            {
                dragStartPos = e.Location;

                var newTarget = battleManager.GetRegionUnitId(baseX + e.Location.X, baseY + e.Location.Y);
                if (newTarget != selectTargetId)
                {
                    selectTargetId = newTarget;
                    doubleBuffedPanel1.Invalidate();
                }
            }
        }

        private bool mouseHold;
        private Point dragStartPos;
        private void BattleForm_MouseUp(object sender, MouseEventArgs e)
        {
            mouseHold = false;
            myCursor.ChangeCursor("default");
        }

        private void BattleForm_MouseDown(object sender, MouseEventArgs e)
        {
            mouseHold = true;
            myCursor.ChangeCursor("hand"); 
            dragStartPos = e.Location;
        }

        private void BattleForm_Click(object sender, EventArgs e)
        {
            if (selectTargetId == 0)
                return;


        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BattleForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("战斗", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

        }

        private void doubleBuffedPanel1_Paint(object sender, PaintEventArgs e)
        {
            if (!showImage)
                return;

            e.Graphics.DrawImage(worldMap, new Rectangle(0, 0, doubleBuffedPanel1.Width, doubleBuffedPanel1.Height),
                new Rectangle(baseX, baseY, doubleBuffedPanel1.Width, doubleBuffedPanel1.Height), GraphicsUnit.Pixel);

            battleManager.Draw(e.Graphics, baseX, baseY);

            if (selectTargetId > 0)
            {
                var targetUnit = battleManager.GetSam(selectTargetId);
                var image = targetUnit.GetPreview();
                int tx = targetUnit.X * BattleManager.CellSize - baseX + BattleManager.CellSize;
                if (tx > doubleBuffedPanel1.Width - image.Width)
                    tx -= image.Width + BattleManager.CellSize;
                e.Graphics.DrawImage(image, tx, targetUnit.Y * BattleManager.CellSize - baseY, image.Width, image.Height);
                image.Dispose();
            }
        }
    }
}