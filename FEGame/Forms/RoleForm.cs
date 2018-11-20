using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using FEGame.Core.Loader;
using FEGame.DataType.Others;
using FEGame.DataType.User;
using FEGame.Forms.Items.Core;

namespace FEGame.Forms
{
    internal sealed partial class RoleForm : BasePanel
    {
        private bool show;

        public RoleForm()
        {
            InitializeComponent();
            bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "closebutton1.jpg");
            bitmapButtonJob.ImageNormal = PicLoader.Read("Button.Panel", "jobbutton.jpg");
            bitmapButtonJob.NoUseDrawNine = true;
            bitmapButtonHistory.ImageNormal = PicLoader.Read("Button.Panel", "infobutton.jpg");
            bitmapButtonHistory.NoUseDrawNine = true;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            Location = new Point(Location.X - 303/2, Location.Y); //空出右边historyform
            show = true;

            if (UserProfile.InfoDungeon.DungeonId > 0)
            {
                bitmapButtonJob.Visible = false;
                bitmapButtonHistory.Visible = false;
            }
        }

        protected override void BasePanelMessageWork(int token)
        {
            if (token == 1)
                Invalidate();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RoleForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(UserProfile.Profile.Name, font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (!show)
                return;
            e.Graphics.FillRectangle(Brushes.LightSlateGray, 25-1, 55-1, 52, 52);
            Image head = PicLoader.Read("Player", string.Format("{0}.png", UserProfile.InfoBasic.Head));
            e.Graphics.DrawImage(head, 25, 55, 50, 50);
            head.Dispose();

            font = new Font("宋体", 11*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font font2 = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);

            Brush brush = new SolidBrush(Color.FromArgb(220, Color.Black));
            e.Graphics.FillRectangle(brush, 12, 300, 305, 105);
            brush.Dispose();

            string namestr = string.Format("Lv {0}", UserProfile.InfoBasic.Level);
            e.Graphics.DrawString(namestr, font, Brushes.White, 20, 305);

            string expstr = string.Format("{0}/{1}", UserProfile.InfoBasic.Exp, ExpTree.GetNextRequired(UserProfile.InfoBasic.Level));
            e.Graphics.DrawString(expstr, font2, Brushes.White, 130, 300);
            e.Graphics.FillRectangle(Brushes.DimGray, 80, 314, 180, 4);
            e.Graphics.FillRectangle(Brushes.DodgerBlue, 80, 314, Math.Min(UserProfile.InfoBasic.Exp * 179 / ExpTree.GetNextRequired(UserProfile.InfoBasic.Level) + 1, 180), 4);

            font.Dispose();
            font2.Dispose();
        }

        private void bitmapButtonJob_Click(object sender, EventArgs e)
        {

        }

        private void bitmapButtonHistory_Click(object sender, EventArgs e)
        {

        }
    }
}