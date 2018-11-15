using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using ControlPlus.Drawing;
using FEGame.Core;
using FEGame.Core.Interface;
using FEGame.Datas.Items;
using FEGame.Datas.Peoples;
using FEGame.Datas.Scenes;

namespace FEGame.Forms.Items.Regions
{
    internal class PictureRegion : SubVirtualRegion
    {
        public delegate void HsActionCallback();

        protected PictureRegionCellType type;
        protected int nid;//����id�����翨��id��npc��id�ȵ�

        public PictureRegion(int id, int x, int y, int width, int height, PictureRegionCellType type, int nid)
            : base(id, x, y, width, height)
        {
            this.nid = nid;
            this.type = type;
            Scale = 1;
        }

        public override void Draw(Graphics g)
        {
            if (nid > 0)
            {
                Image img = null;
                HsActionCallback preAction = null;
                HsActionCallback action = null;
                if (type == PictureRegionCellType.Item)
                {
                    img = HItemBook.GetHItemImage(nid);
                    action = () =>
                    {
                        var itemConfig = ConfigData.GetHItemConfig(nid);
                        var pen = new Pen(Color.FromName(HSTypes.I2RareColor(itemConfig.Rare)), 2);
                        g.DrawRectangle(pen, X, Y, Width, Height);
                        pen.Dispose();
                    };
                }
                else if (type == PictureRegionCellType.Gismo)
                {
                    img = DungeonBook.GetGismoImage(nid);
                }
                else if (type == PictureRegionCellType.People)
                {
                    img = PeopleBook.GetPersonImage(nid);
                    preAction = () =>
                    {
                        var peopleConfig = ConfigData.GetPeopleConfig(nid);
                        var brush = new SolidBrush(Color.FromName(HSTypes.I2QualityColorD(peopleConfig.Quality)));
                        g.FillRectangle(brush, X + 3, Y + 3, Width - 6, Height - 6);
                        brush.Dispose();
                    };
                }
                else if (type == PictureRegionCellType.CardQual)
                {
                    img = HSIcons.GetIconsByEName("gem" + nid);
                }
                else if (type == PictureRegionCellType.Job)
                {
                    img = HSIcons.GetIconsByEName("job" + nid);
                    action = () =>
                    {
                        var jobConfig = ConfigData.GetJobConfig(nid + JobConfig.Indexer.NewBie);
                        Pen pen = new Pen(Color.FromName(jobConfig.Color));
                        g.DrawRectangle(pen, X, Y, Width, Height);
                        pen.Dispose();
                    };
                }
                else if (type == PictureRegionCellType.SceneQuest)
                {
                    img = SceneQuestBook.GetSceneQuestImageScene(nid);
                }
                else if (type == PictureRegionCellType.DungeonItem)
                {
                    img = DungeonBook.GetDungeonItemImage(nid);
                }

                if (preAction != null)
                    preAction();
                if (img != null)
                {
                    if (Scale == 1)
                    {
                        if (Enabled)
                            g.DrawImage(img, X, Y, Width, Height);
                        else
                            g.DrawImage(img, new Rectangle(X, Y, Width, Height), 0,0, img.Width,img.Height,GraphicsUnit.Pixel, HSImageAttributes.ToGray);
                    }
                    else
                    {
                        int realWidth = (int)(Width*Scale);
                        int realHeight = (int)(Height * Scale);
                        g.DrawImage(img, X + (Width - realWidth) / 2, Y + (Height - realHeight) / 2, realWidth, realHeight);
                    }
                }
                if (action != null)
                    action();
            }

            foreach (IRegionDecorator decorator in decorators)
            {
                decorator.Draw(g, X, Y, Width, Height);
            }
        }

        public override void SetKeyValue(int value)
        {
            base.SetKeyValue(value);
            nid = value;
        }

        public override int GetKeyValue()
        {
            return nid;
        }

        public void SetType(PictureRegionCellType value)
        {
            type = value;
        }

        public PictureRegionCellType GetVType()
        {
            return type;
        }

        public float Scale { get; set; } //����ͼƬ����

        public override void ShowTip(ImageToolTip tooltip, Control form, int x, int y)
        {
            var regionType = GetVType();
            if (regionType == PictureRegionCellType.Item)
            {
                Image image = HItemBook.GetPreview(nid);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == PictureRegionCellType.Gismo)
            {
                Image image = DungeonBook.GetPreview(nid);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == PictureRegionCellType.People)
            {
                Image image = PeopleBook.GetPreview(nid);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == PictureRegionCellType.DungeonItem)
            {
                var itemConfig = ConfigData.GetDungeonItemConfig(nid);
                Image image = DrawTool.GetImageByString(itemConfig.Name + "$" + itemConfig.Des, 160);
                tooltip.Show(image, form, x, y);
            }
        }
    }

    internal enum PictureRegionCellType
    {
        Item,
        Equip,
        Card,
        Gismo,
        HeroSkill,
        People,
        CardQual,
        Job,
        Bless,
        SceneQuest,
        DungeonItem,
    }
}
