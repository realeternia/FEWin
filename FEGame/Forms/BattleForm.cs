using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FEGame.Controller.Battle;
using FEGame.Core;
using FEGame.Core.Loader;
using FEGame.Forms.Items.Battle;
using FEGame.Forms.Items.Core;
using NarlonLib.Math;

namespace FEGame.Forms
{
    internal sealed partial class BattleForm : BasePanel
    {
        enum RoundStage
        {
            None, SelectMove, Move
        }
        private bool showImage;
        private int baseX = 900;
        private int baseY = 250;
        private int mouseOnId; //鼠标上的单位id
        private int moveId; //移动单位id

        private bool mouseHold;
        private Point dragStartPos; //像素
        private Point selectCellPos; //地图格

        private HSCursor myCursor;

        private BattleManager battleManager;
        private TileManager tileManager;

        private List<TileManager.PathResult> savedPath;
        private RoundStage stage;
        private ChessMoveAnim chessMoveAnim; //移动控件

        public BattleForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "closebutton1.jpg");
            bitmapButtonClose.NoUseDrawNine = true;
            myCursor = new HSCursor(this);

            battleManager = new BattleManager();
            tileManager = new TileManager();
            chessMoveAnim = new ChessMoveAnim();
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            tileManager.Init();

            baseX = MathTool.Clamp(baseX, 0, tileManager.MapPixelWidth - doubleBuffedPanel1.Width);
            baseY = MathTool.Clamp(baseY, 0, tileManager.MapPixelHeight - doubleBuffedPanel1.Height);

            //test code
            battleManager.AddUnit(new Controller.Battle.Units.HeroSam(43020101, 15, 15, 1));
            battleManager.AddUnit(new Controller.Battle.Units.HeroSam(43020102, 15, 13, 1));
            battleManager.AddUnit(new Controller.Battle.Units.MonsterSam(43000005, 18, 18, 2));
            battleManager.AddUnit(new Controller.Battle.Units.MonsterSam(43000005, 18, 21, 2));

            showImage = true;
        }

        public override void OnFrame(int tick, float timePass)
        {
            if (chessMoveAnim.Update())
                doubleBuffedPanel1.Invalidate();
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
                if (newTarget != mouseOnId)
                {
                    mouseOnId = newTarget;
                    doubleBuffedPanel1.Invalidate();
                }

                var newCellPos = new Point((dragStartPos.X + baseX) / TileManager.CellSize, (dragStartPos.Y + baseY) / TileManager.CellSize);
                if (newCellPos.X != selectCellPos.X || newCellPos.Y != selectCellPos.Y)
                {
                    selectCellPos = newCellPos;
                    doubleBuffedPanel1.Invalidate();
                }
            }
        }

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
            var x = (dragStartPos.X + baseX) / TileManager.CellSize;
            var y = (dragStartPos.Y + baseY) / TileManager.CellSize;
            if (stage == RoundStage.None)
            {
                if (mouseOnId <= 0)
                    return;

                moveId = mouseOnId;
                var tileUnit = battleManager.GetSam(mouseOnId);
                if (tileUnit.Camp == ConfigDatas.CampConfig.Indexer.Reborn)
                {
                    savedPath = tileManager.GetPathResults(x, y, tileUnit.Mov, (byte)ConfigDatas.CampConfig.Indexer.Reborn);
                    stage = RoundStage.SelectMove;
                    doubleBuffedPanel1.Invalidate();
                }
            }
            else if (stage == RoundStage.SelectMove)
            {
                var selectTarget = savedPath.Find(cell => cell.NowCell.X == x && cell.NowCell.Y == y);
                if (selectTarget != null)
                {
                    stage = RoundStage.Move;
                    Stack<Point> roadPath = new Stack<Point>();
                    do
                    {
                        roadPath.Push(selectTarget.NowCell);
                        selectTarget = savedPath.Find(cell => cell.NowCell.X == selectTarget.Parent.X && cell.NowCell.Y == selectTarget.Parent.Y);
                    } while (selectTarget != null && selectTarget.Parent.X >= 0);

                    savedPath = null;
                    var tileUnit = battleManager.GetSam(moveId);
                    roadPath.Push(new Point(tileUnit.X, tileUnit.Y));
                    chessMoveAnim.Set(tileUnit.Cid, roadPath.ToArray());
                    chessMoveAnim.FinishAction = delegate
                    {
                        tileManager.Leave(tileUnit.X, tileUnit.Y, moveId);
                        tileUnit.X = (byte)x;
                        tileUnit.Y = (byte)y;
                        tileManager.Enter(tileUnit.X, tileUnit.Y, moveId, tileUnit.Camp);
                        stage = RoundStage.None;
                        moveId = 0;
                    };
                }
            }
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
            battleManager.Draw(e.Graphics, baseX, baseY, moveId);

            if (savedPath != null && savedPath.Count > 0)
            {
                Font ft = new Font("宋体", 11, FontStyle.Bold);
                Brush selectRegion = new SolidBrush(Color.FromArgb(100, Color.Green));

                TileManager.PathResult mouseTarget = null;
                foreach (var pathResult in savedPath)
                {
                    var px = pathResult.NowCell.X * TileManager.CellSize - baseX;
                    var py = pathResult.NowCell.Y * TileManager.CellSize - baseY;
                    e.Graphics.FillRectangle(selectRegion, px, py, TileManager.CellSize, TileManager.CellSize);

                    if (pathResult.NowCell.X == selectCellPos.X && pathResult.NowCell.Y == selectCellPos.Y)
                    {
                        mouseTarget = pathResult;
                    }

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

                if (mouseTarget != null) //选中了一个移动格子
                {
                    while (mouseTarget.Parent.X >= 0)
                    {
                        e.Graphics.DrawLine(Pens.White, mouseTarget.NowCell.X * TileManager.CellSize - baseX+ TileManager.CellSize/2, mouseTarget.NowCell.Y * TileManager.CellSize - baseY + TileManager.CellSize / 2
                            , mouseTarget.Parent.X * TileManager.CellSize - baseX + TileManager.CellSize / 2, mouseTarget.Parent.Y * TileManager.CellSize - baseY + TileManager.CellSize / 2);

                        mouseTarget = savedPath.Find(cell => cell.NowCell.X == mouseTarget.Parent.X && cell.NowCell.Y == mouseTarget.Parent.Y);
                    }
                }
            }

            chessMoveAnim.Draw(e.Graphics, baseX, baseY);

            if (mouseOnId > 0)
            {
                var targetUnit = battleManager.GetSam(mouseOnId);
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