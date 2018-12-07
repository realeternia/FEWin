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
            get { return 2; }
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

        public void OnAttack(BaseSam attacker)
        {
            LeftHp -= attacker.Atk - Def;
        }

        public virtual void Draw(Graphics g, int baseX, int baseY)
        {
            var cellSize = TileManager.CellSize;
            var cellX = X * cellSize - baseX;
            var cellY = Y * cellSize - baseY;

            var img = HSIcons.GetImage("Samurai", Cid);

            g.DrawImage(img, cellX, cellY, cellSize, cellSize);

            g.FillRectangle(Brushes.Black, cellX+5, cellY + cellSize - 10, cellSize-5, 10);
            g.FillRectangle(Brushes.Lime, cellX+5+1, cellY + cellSize - 10+1, (cellSize-5-2)* LeftHp / baseAttr.Hp, 10-2);

            var jobImg = HSIcons.GetImage("Job", Job);
            g.DrawImage(jobImg, cellX, cellY + cellSize - 16, 16, 16);
        }

        public Image GetPreview()
        {
            ControlPlus.TipImage tipData = new ControlPlus.TipImage(PaintTool.GetTalkColor);
            tipData.AddTextNewLine(Name, "Lime", 20);
            tipData.AddTextNewLine(Level.ToString(), "Lime", 20);
            //   tipData.AddTextNewLine(string.Format("区域: {0}", ConfigData.GetSceneRegionConfig(sceneConfig.RegionId).Name), "White", 20);

            return tipData.Image;
        }
    }
}