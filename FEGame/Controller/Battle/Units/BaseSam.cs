using System;
using System.Drawing;
using ConfigDatas;
using FEGame.Controller.Battle.Units.Frags;
using FEGame.Core;
using FEGame.Tools;

namespace FEGame.Controller.Battle.Units
{
    public abstract class BaseSam
    {
        public BattleManager BM { get; set; }

        public int Id { get; set; } //唯一id
        public int Cid { get; private set; }
        public byte Level { get; protected set; }
        public byte Camp { get; protected set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public string Name { get; protected set; }

        public bool IsFinished { get; set; } //回合结束

        protected SamAttr baseAttr; //基础属性

        public int Atk
        {
            get { return baseAttr.Str; }
        }
        public int Def
        {
            get { return baseAttr.Def; }
        }
        public int Mov
        {
            get { return baseAttr.Mov; }
        }
        public int Range
        {
            get { return 1; }
        }
        public int LeftHp { get; set; }

        public int Job { get; set; }

        protected BaseSam(int id, byte x, byte y, byte camp)
        {
            Cid = id;
            X = x;
            Y = y;
            Camp = camp;
            baseAttr = new SamAttr();
        }

        public virtual void Init()
        {
            var samuraiConfig = ConfigData.GetSamuraiConfig(Cid);
            Name = samuraiConfig.Name;
            baseAttr.Str = samuraiConfig.Str;
            baseAttr.Def = samuraiConfig.Def;
            baseAttr.Skl = samuraiConfig.Skl;
            baseAttr.Spd = samuraiConfig.Spd;
            baseAttr.Hp = samuraiConfig.Hp;
            baseAttr.Mag = samuraiConfig.Mag;
            baseAttr.Luk = samuraiConfig.Luk;
            baseAttr.Mov = samuraiConfig.Mov;
            Level = samuraiConfig.Level;
            Job = samuraiConfig.Job;

            LeftHp = baseAttr.Hp;
        }

        public void OnRemove()
        {

        }

        public bool OnAttack(BaseSam attacker)
        {
            LeftHp -= attacker.Atk - Def;
            if (LeftHp <= 0)
            {
                BattleManager.Instance.RemoveUnit(this);
                return true;
            }

            return false;
        }

        public int GetDistanceFrom(int tx, int ty)
        {
            return Math.Abs(X - tx) + Math.Abs(Y - ty);
        }

        public virtual void Draw(Graphics g, int baseX, int baseY)
        {
            var cellSize = TileManager.CellSize;
            var cellX = X * cellSize - baseX;
            var cellY = Y * cellSize - baseY;

            var img = HSIcons.GetImage("Samurai", Cid);

            g.DrawImage(img, new Rectangle(cellX, cellY, cellSize, cellSize), 0, 0, 
                img.Width, img.Height, GraphicsUnit.Pixel, Camp == CampConfig.Indexer.Reborn ? HSImageAttributes.ToGreen : HSImageAttributes.ToRed);

            if (IsFinished)
                g.DrawImage(img, new Rectangle(cellX+2, cellY+2, cellSize-4, cellSize-4), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, HSImageAttributes.ToGray);
            else
                g.DrawImage(img, cellX+2, cellY+2, cellSize-4, cellSize-4);

            g.FillRectangle(Brushes.Black, cellX+5, cellY + cellSize - 8, cellSize-5, 8);
            g.FillRectangle(Brushes.Lime, cellX+5+1, cellY + cellSize - 8+1, (cellSize-5-2)* LeftHp / baseAttr.Hp, 8-2);

            var jobImg = HSIcons.GetImage("Job", Job);
            g.DrawImage(jobImg, cellX, cellY + cellSize - 16, 16, 16);

            if (IsFinished)
            {
                g.DrawImage(HSIcons.GetSystemImage("rgmoved"), cellX + cellSize - 25, cellY + cellSize - 25, 21, 23);
            }
        }

        public Image GetPreview()
        {
            ControlPlus.TipImage tipData = new ControlPlus.TipImage(160);
            tipData.AddTextNewLine(string.Format("{0} Lv{1}", Name, Level.ToString()), Camp == CampConfig.Indexer.Reborn ? "Lime" : "Red", 20);

            tipData.AddTextNewLine("", "White", 20);
            tipData.AddImageXY(HSIcons.GetIconsByEName("hatt1"), 0, 0, 32, 32, 5, 20, 20, 20);
            tipData.AddTextOff(Atk.ToString(), "White", 30);
            tipData.AddImageXY(HSIcons.GetIconsByEName("hatt2"), 0, 0, 32, 32, 5+80, 20, 20, 20);
            tipData.AddTextOff(Def.ToString(), "White", 30+80);

            tipData.AddTextNewLine("", "White", 20);
            tipData.AddImageXY(HSIcons.GetIconsByEName("hatt5"), 0, 0, 32, 32, 5, 40, 20, 20);
            tipData.AddTextOff(Mov.ToString(), "White", 30);
            tipData.AddImageXY(HSIcons.GetIconsByEName("hatt8"), 0, 0, 32, 32, 5 + 80, 40, 20, 20);
            tipData.AddTextOff(Range.ToString(), "White", 30 + 80);

            tipData.AddTextNewLine("", "White", 20);
            tipData.AddImageXY(HSIcons.GetIconsByEName("hatt7"), 0, 0, 32, 32, 5, 60, 20, 20);
            tipData.AddTextOff(string.Format("{0}/{1}", LeftHp, baseAttr.Hp), "White", 30);

            return tipData.Image;
        }
    }
}