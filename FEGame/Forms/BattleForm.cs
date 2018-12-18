using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FEGame.Controller.Battle;
using FEGame.Controller.Battle.Units;
using FEGame.Core;
using FEGame.Core.Loader;
using FEGame.DataType.Effects;
using FEGame.DataType.Effects.Facts;
using FEGame.Forms.Items.Battle;
using FEGame.Forms.Items.Core;
using NarlonLib.Core;
using NarlonLib.Math;

namespace FEGame.Forms
{
    internal sealed partial class BattleForm : BasePanel
    {
        enum RoundStage
        {
            None, SelectMove, Move, Attack, AttackAnim
        }
        private bool showImage;
        private int baseX = 900;
        private int baseY = 250;
        private int mouseOnId; //鼠标上的单位id
        private int moveId; //移动单位id
        private int attackId; //攻击单位id

        private bool mouseHold;
        private Point dragStartPos; //像素
        private Point selectCellPos; //地图格

        private HSCursor myCursor;

        private BattleManager battleManager;
        private TileManager tileManager;

        private List<TileManager.PathResult> savedPath;
        private RoundStage stage;
        private ChessMoveAnim chessMoveAnim; //移动控件

        private TextFlowController textFlow;
        private EffectRunController effectRun;

        private ActionTimely refreshAll;
        private NLTimerManager timerManager;
        private NLCoroutineManager coroutineManager;

        public BattleForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "closebutton1.jpg");
            bitmapButtonClose.NoUseDrawNine = true;
            myCursor = new HSCursor(this);

            battleManager = new BattleManager();
            tileManager = new TileManager();
            chessMoveAnim = new ChessMoveAnim();
           
            textFlow = new TextFlowController();
            effectRun = new EffectRunController();
            refreshAll = ActionTimely.Register(doubleBuffedPanel1.Invalidate, 0.05);

            timerManager = new NLTimerManager();
            coroutineManager = new NLCoroutineManager(timerManager);
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
                refreshAll.Fire();

            effectRun.Update(doubleBuffedPanel1);

            textFlow.Update(doubleBuffedPanel1);

            refreshAll.Update();

            timerManager.DoTimer();
        }

        private void BattleForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHold)
            {
                if (MathTool.GetDistance(e.Location, dragStartPos) > 3)
                {
                    baseX -= e.Location.X - dragStartPos.X;
                    baseY -= e.Location.Y - dragStartPos.Y;
                    baseX = MathTool.Clamp(baseX, 0, tileManager.MapPixelWidth - doubleBuffedPanel1.Width);
                    baseY = MathTool.Clamp(baseY, 0, tileManager.MapPixelHeight - doubleBuffedPanel1.Height);
                    dragStartPos = e.Location;
                    refreshAll.Fire();
                }
            }
            else
            {
                dragStartPos = e.Location;

                var newTarget = battleManager.GetRegionUnitId(baseX + e.Location.X, baseY + e.Location.Y);
                if (newTarget != mouseOnId)
                {
                    mouseOnId = newTarget;
                    refreshAll.Fire();
                }

                var newCellPos = new Point((dragStartPos.X + baseX) / TileManager.CellSize, (dragStartPos.Y + baseY) / TileManager.CellSize);
                if (newCellPos.X != selectCellPos.X || newCellPos.Y != selectCellPos.Y)
                {
                    selectCellPos = newCellPos;
                    refreshAll.Fire();
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

        private void doubleBuffedPanel1_MouseClick(object sender, MouseEventArgs e)
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
                    var adapter = new TileAdapter(tileManager.Width, tileManager.Height);
                    savedPath = adapter.GetPathMove(x, y, tileUnit.Mov, (byte)ConfigDatas.CampConfig.Indexer.Reborn);
                    stage = RoundStage.SelectMove;
                    refreshAll.Fire();
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
                    if (tileUnit.X == x && tileUnit.Y == y)//原地走
                    {
                        attackId = moveId;
                        moveId = 0;

                        AfterMove(tileUnit, x, y);
                        stage = RoundStage.Attack;
                    }
                    else
                    {
                        roadPath.Push(new Point(tileUnit.X, tileUnit.Y));
                        chessMoveAnim.Set(tileUnit.Cid, roadPath.ToArray());
                        chessMoveAnim.FinishAction = delegate
                        {
                            tileManager.Move(tileUnit.X, tileUnit.Y, (byte)x, (byte)y, moveId, tileUnit.Camp);

                            tileUnit.X = (byte)x;
                            tileUnit.Y = (byte)y;

                            AfterMove(tileUnit, x, y);
                            stage = RoundStage.Attack;
                        };
                    }
  
                }
            }
            else if (stage == RoundStage.Attack)
            {
                if (e.Button == MouseButtons.Left)//only left key
                {
                    var selectTarget = savedPath.Find(cell => cell.NowCell.X == x && cell.NowCell.Y == y);
                    if (selectTarget != null)
                    {
                        var tileConfig = tileManager.GetTile(selectTarget.NowCell.X, selectTarget.NowCell.Y);
                        if (tileConfig.Camp > 0)
                        {
                            var atkUnit = battleManager.GetSam(attackId);
                            var targetUnit = battleManager.GetSam(tileConfig.UnitId);

                            var unitPos = new Point(targetUnit.X * TileManager.CellSize - baseX, targetUnit.Y * TileManager.CellSize - baseY);
                            var unitSize = new Size(TileManager.CellSize, TileManager.CellSize);
                            
                            var effect = new StaticUIEffect(EffectBook.GetEffect("hit1"), unitPos, unitSize);
                            effectRun.AddEffect(effect);

                            coroutineManager.StartCoroutine(DelayAttack(atkUnit, targetUnit));

                            attackId = 0;
                            savedPath = null;
                            stage = RoundStage.AttackAnim;

                            refreshAll.Fire();
                        }
                    }
                }
                else//右键取消攻击
                {
                    attackId = 0;
                    savedPath = null;
                    stage = RoundStage.None;

                    refreshAll.Fire();
                }

            }
        }

        private IEnumerator DelayAttack(BaseSam atkUnit, BaseSam targetUnit)
        {
            yield return new NLWaitForSeconds(0.2f);
            var unitPos = new Point(targetUnit.X * TileManager.CellSize - baseX, targetUnit.Y * TileManager.CellSize - baseY);
            var unitSize = new Size(TileManager.CellSize, TileManager.CellSize);
            if (targetUnit.OnAttack(atkUnit))
            {
                var effectDie = new StaticUIImageEffect(EffectBook.GetEffect("shrink"), HSIcons.GetImage("Samurai", targetUnit.Cid), unitPos, unitSize);
                effectRun.AddEffect(effectDie);
            }
            stage = RoundStage.None;
        }

        private void AfterMove(BaseSam tileUnit, int x, int y)
        {
            attackId = moveId;
            moveId = 0;

            var adapter = new TileAdapter(tileManager.Width, tileManager.Height);
            savedPath = adapter.GetPathAttack(x, y, tileUnit.Range, (byte) ConfigDatas.CampConfig.Indexer.Reborn);
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
            battleManager.Draw(e.Graphics, baseX, baseY, stage == RoundStage.SelectMove ? 0 : moveId);//ws处理SelectMove阶段显示人物

            DrawMoveRegion(e);

            chessMoveAnim.Draw(e.Graphics, baseX, baseY);

            if (selectCellPos.X >= 0 && selectCellPos.Y >= 0)
            {
                var px = selectCellPos.X * TileManager.CellSize - baseX;
                var py = selectCellPos.Y * TileManager.CellSize - baseY;

                if (stage == RoundStage.SelectMove)
                {
                    var selectImg = HSIcons.GetImage("Samurai", battleManager.GetSam(moveId).Cid);

                    e.Graphics.DrawImage(selectImg, new Rectangle(px, py, TileManager.CellSize, TileManager.CellSize), 0, 0,
                        selectImg.Width, selectImg.Height, GraphicsUnit.Pixel, HSImageAttributes.ToAlphaHalf);

                    if (savedPath.Find(node => node.NowCell.X == selectCellPos.X && node.NowCell.Y == selectCellPos.Y) == null)
                    {
                        e.Graphics.DrawImage(HSIcons.GetSystemImage("rgout"), px+5, py+5, TileManager.CellSize-10, TileManager.CellSize-10);
                    }
                }
                else
                {
                    var selectImg = HSIcons.GetSystemImage("actmark");
                    e.Graphics.DrawImage(selectImg, px, py, TileManager.CellSize, TileManager.CellSize);
                }
            }

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

            effectRun.Draw(e.Graphics);
            textFlow.Draw(e.Graphics);
        }

        private void DrawMoveRegion(PaintEventArgs e)
        {
            if (savedPath != null && savedPath.Count > 0)
            {
                Font ft = new Font("宋体", 11, FontStyle.Bold);
                TileManager.PathResult mouseTarget = null;
                foreach (var pathResult in savedPath)
                {
                    var px = pathResult.NowCell.X * TileManager.CellSize - baseX;
                    var py = pathResult.NowCell.Y * TileManager.CellSize - baseY;
                    e.Graphics.DrawImage(HSIcons.GetSystemImage(stage == RoundStage.SelectMove ? "rgmove" : "rgattack"), px, py, TileManager.CellSize, TileManager.CellSize);
                    if (stage == RoundStage.Attack)
                    {
                        var tileConfig = tileManager.GetTile(pathResult.NowCell.X, pathResult.NowCell.Y);
                        if (tileConfig.Camp > 0 && tileConfig.Camp != (byte) ConfigDatas.CampConfig.Indexer.Reborn)
                            e.Graphics.DrawImage(HSIcons.GetSystemImage("rgtarget"), px, py, TileManager.CellSize, TileManager.CellSize);
                    }

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
                ft.Dispose();

                if (stage == RoundStage.SelectMove && mouseTarget != null) //绘制移动路径
                {
                    while (mouseTarget.Parent.X >= 0)
                    {
                        e.Graphics.DrawLine(Pens.White,
                            mouseTarget.NowCell.X * TileManager.CellSize - baseX + TileManager.CellSize / 2,
                            mouseTarget.NowCell.Y * TileManager.CellSize - baseY + TileManager.CellSize / 2
                            , mouseTarget.Parent.X * TileManager.CellSize - baseX + TileManager.CellSize / 2,
                            mouseTarget.Parent.Y * TileManager.CellSize - baseY + TileManager.CellSize / 2);

                        mouseTarget = savedPath.Find(cell =>
                            cell.NowCell.X == mouseTarget.Parent.X && cell.NowCell.Y == mouseTarget.Parent.Y);
                    }
                }
            }
        }
    }
}