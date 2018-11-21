using System;
using System.Collections.Generic;
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

        private HSCursor myCursor;

        private BattleManager battleManager;
        private TileManager tileManager;

        private List<TileManager.PathResult> savedPath;

        public BattleForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "closebutton1.jpg");
            bitmapButtonClose.NoUseDrawNine = true;
            myCursor = new HSCursor(this);


            battleManager = new BattleManager();
            tileManager = new TileManager();

        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            tileManager.Init();

            baseX = MathTool.Clamp(baseX, 0, tileManager.MapPixelWidth - doubleBuffedPanel1.Width);
            baseY = MathTool.Clamp(baseY, 0, tileManager.MapPixelHeight - doubleBuffedPanel1.Height);

            //test code
            battleManager.AddUnit(new Controller.Battle.Units.HeroSam(43020101, 15, 15));
            battleManager.AddUnit(new Controller.Battle.Units.HeroSam(43020102, 15, 13));
            battleManager.AddUnit(new Controller.Battle.Units.MonsterSam(43000005, 18, 18));
            battleManager.AddUnit(new Controller.Battle.Units.MonsterSam(43000005, 18, 21));

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
                    baseX = MathTool.Clamp(baseX, 0, tileManager.MapPixelWidth - doubleBuffedPanel1.Width);
                    baseY = MathTool.Clamp(baseY, 0, tileManager.MapPixelHeight - doubleBuffedPanel1.Height);
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

        private void doubleBuffedPanel1_Click(object sender, EventArgs e)
        {
            var x = (dragStartPos.X+baseX) / TileManager.CellSize;
            var y = (dragStartPos.Y+baseY) / TileManager.CellSize;
            savedPath = tileManager.GetPathResults(x, y, 6);

            doubleBuffedPanel1.Invalidate();
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

            tileManager.Draw(e.Graphics, baseX, baseY, doubleBuffedPanel1.Width, doubleBuffedPanel1.Height);
            battleManager.Draw(e.Graphics, baseX, baseY);

            if (savedPath != null && savedPath.Count > 0)
            {
                Font ft = new Font("宋体", 11, FontStyle.Bold);
                Brush selectRegion = new SolidBrush(Color.FromArgb(100, Color.Green));
                foreach (var pathResult in savedPath)
                {
                    var px = pathResult.NowCell.X * TileManager.CellSize - baseX;
                    var py = pathResult.NowCell.Y * TileManager.CellSize - baseY;
                    e.Graphics.FillRectangle(selectRegion, px, py, TileManager.CellSize, TileManager.CellSize);

#if DEBUG
                    e.Graphics.DrawString(pathResult.MovLeft.ToString(), ft, Brushes.Black, px, py);
                    if (pathResult.Parent.X == pathResult.NowCell.X + 1)
                        e.Graphics.DrawString("←", ft, Brushes.Black, px, py + 15);
                    else if (pathResult.Parent.X == pathResult.NowCell.X - 1)
                        e.Graphics.DrawString("→", ft, Brushes.Black, px, py + 15);
                    else if (pathResult.Parent.Y == pathResult.NowCell.Y + 1)
                        e.Graphics.DrawString("↑", ft, Brushes.Black, px, py + 15);
                    else if (pathResult.Parent.Y == pathResult.NowCell.Y - 1)
                        e.Graphics.DrawString("↓", ft, Brushes.Black, px, py + 15);
#endif
                }

                selectRegion.Dispose();
                ft.Dispose();
            }

            if (selectTargetId > 0)
            {
                var targetUnit = battleManager.GetSam(selectTargetId);
                var image = targetUnit.GetPreview();
                int tx = targetUnit.X * TileManager.CellSize - baseX + TileManager.CellSize;
                if (tx > doubleBuffedPanel1.Width - image.Width)
                    tx -= image.Width + TileManager.CellSize;
                e.Graphics.DrawImage(image, tx, targetUnit.Y * TileManager.CellSize - baseY, image.Width, image.Height);
                image.Dispose();
            }
        }

    }
}