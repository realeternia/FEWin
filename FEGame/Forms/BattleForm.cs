using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using FEGame.Controller.Battle;
using FEGame.Core;
using FEGame.Core.Loader;
using FEGame.DataType.User;
using FEGame.Forms.CMain;
using FEGame.Forms.Items.Core;
using FEGame.Tools;
using NarlonLib.Math;

namespace FEGame.Forms
{
    internal sealed partial class BattleForm : BasePanel
    {
        private bool showImage;
        private int baseX = 900;
        private int baseY = 250;
        private int pointerY;
        private string selectName;
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
            if ((tick % 10) == 0)
            {
                pointerY = (pointerY == 0 ? -12 : 0);
                doubleBuffedPanel1.Invalidate();
            }
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

                string newSel = "";
                foreach (var mapIconConfig in ConfigData.SceneDict.Values)
                {
                    if (mapIconConfig.Icon == "")
                        continue;

                    //var iconSize = iconSizeDict[mapIconConfig.Id];
                    //if (truex > mapIconConfig.IconX - baseX && truey > mapIconConfig.IconY - baseY && truex < mapIconConfig.IconX - baseX + iconSize.Width && truey < mapIconConfig.IconY - baseY + iconSize.Height)
                    //    newSel = mapIconConfig.Icon;
                }
                if (newSel != selectName)
                {
                    selectName = newSel;
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
            if (selectName =="")
                return;

            foreach (var mapIconConfig in ConfigData.SceneDict.Values)
            {
                if (mapIconConfig.Icon == selectName && mapIconConfig.Id != UserProfile.InfoBasic.MapId)
                {
                    if (UserProfile.InfoDungeon.DungeonId > 0) //副本中不允许外传
                        return;

                    if (MessageBoxEx2.Show("是否花10钻石立刻移动到该地区?") == DialogResult.OK)
                    {
                        if (UserProfile.InfoBag.PayDiamond(10))
                        {
                            UserProfile.InfoBasic.Position = 0;//如果是0，后面流程会随机一个位置
                    //        Scene.Instance.ChangeMap(mapIconConfig.Id, true);
                            Close();
                        }
                        else
                        {
                            MainTipManager.AddTip(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughDimond), "Red");
                        }
                    }
                    return;
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

        private static Image GetPreview(int id)
        {
            SceneConfig sceneConfig = ConfigData.GetSceneConfig(id);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage(PaintTool.GetTalkColor);
            tipData.AddTextNewLine(sceneConfig.Name, "Lime", 20);
            tipData.AddTextNewLine(string.Format("地图等级: {0}", sceneConfig.Level), sceneConfig.Level>UserProfile.InfoBasic.Level?"Red": "White");
         //   tipData.AddTextNewLine(string.Format("区域: {0}", ConfigData.GetSceneRegionConfig(sceneConfig.RegionId).Name), "White", 20);

            return tipData.Image;
        }

        private void doubleBuffedPanel1_Paint(object sender, PaintEventArgs e)
        {
            if (!showImage)
                return;

            e.Graphics.DrawImage(worldMap, new Rectangle(0, 0, doubleBuffedPanel1.Width, doubleBuffedPanel1.Height),
                new Rectangle(baseX, baseY, doubleBuffedPanel1.Width, doubleBuffedPanel1.Height), GraphicsUnit.Pixel);

            battleManager.Draw(e.Graphics, baseX, baseY);

            foreach (var mapIconConfig in ConfigData.SceneDict.Values)
            {
                if (mapIconConfig.Icon == "")
                    continue;

                if (mapIconConfig.Icon == selectName)
                {
                    int x = mapIconConfig.IconX;
                    int y = mapIconConfig.IconY;
                    int width = 100;
                    int height = 100;

                    if (x > baseX && y > baseY && x + width < baseX + doubleBuffedPanel1.Width && y + height < baseY + doubleBuffedPanel1.Height)
                    {
                        Image image = PicLoader.Read("Map.MapIcon", string.Format("{0}.png", mapIconConfig.Icon));
                        Rectangle destRect = new Rectangle(x - baseX + 11, y - baseY + 31, width + 8, height + 8);
                        Rectangle destRect2 = new Rectangle(x - baseX + 15, y - baseY + 35, width, height);
                        e.Graphics.DrawImage(image, destRect, 0, 0, width, height, GraphicsUnit.Pixel);
                        e.Graphics.DrawImage(image, destRect2, 0, 0, width, height, GraphicsUnit.Pixel);
                        image.Dispose();

                        image = GetPreview(mapIconConfig.Id);
                        int tx = x - baseX + width;
                        if (tx > doubleBuffedPanel1.Width - image.Width)
                            tx -= image.Width + width;
                        e.Graphics.DrawImage(image, tx + 15, y - baseY + 35, image.Width, image.Height);
                        image.Dispose();
                    }
                }
                //if (mapIconConfig.Id == UserProfile.InfoBasic.MapId)
                //{
                //    Image image = PicLoader.Read("Map", "arrow.png");
                //    e.Graphics.DrawImage(image, mapIconConfig.IconX - baseX + 40, mapIconConfig.IconY - baseY - 5 + pointerY, 30, 48);
                //    image.Dispose();
                //}
            }

        }
    }
}