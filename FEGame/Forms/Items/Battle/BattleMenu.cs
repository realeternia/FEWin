using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FEGame.Core;

namespace FEGame.Forms.Items.Battle
{
    public class BattleMenu
    {
        public delegate void MenuClickEvent(string evt);

        private class MenuItemData
        {
            public int Id;

            public string Command;
            public string Text;
            public string Color;
            public string Icon;

            public Point Position;
            public Size Size;

            public MenuItemData(int id, string t, string tx, string cr, string icon)
            {
                Id = id;
                Command = t;
                Text = tx;
                Color = cr;
                Icon = icon;
            }
        }

        private int baseX;
        private int baseY;
        private int selectIndex = -1;
        private List<MenuItemData> datas = new List<MenuItemData>();
        private const int CellHeight = 18;
        private int CellWidth = 18;
        private int columnCount = 1;

        public event MenuClickEvent OnClick;

        public void Add(string cmd, string text)
        {
            datas.Add(new MenuItemData(datas.Count + 1, cmd, text, "white", ""));
        }

        public void Add(string cmd, string text, string color)
        {
            datas.Add(new MenuItemData(datas.Count + 1, cmd, text, color, ""));
        }

        public void Clear()
        {
            datas.Clear();
        }

        public bool OnMove(int mx, int my)
        {
            int nowIndex = -1;
            for (int i = 0; i < datas.Count; i++)
            {
                var menuItemData = datas[i];
                Rectangle rect = new Rectangle(menuItemData.Position.X + baseX, menuItemData.Position.Y + baseY,
                    menuItemData.Size.Width, menuItemData.Size.Height);
                if (rect.Contains(mx, my))
                    nowIndex = i;
            }

            if (nowIndex != selectIndex)
            {
                selectIndex = nowIndex;
                return true;
            }

            return false;
        }

        public void Show(int x, int y)
        {
            baseX = x;
            baseY = y;

            float maxCellWidth = 0;
            Font fontsong = new Font("宋体", 10 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Bitmap tempImg = new Bitmap(3, 3);
            Graphics g = Graphics.FromImage(tempImg);
            foreach (var menuItemData in datas)
            {//计算最大的宽度
                var imgOff = 0;
                if (menuItemData.Icon != "")
                    imgOff += 16;
                var size = TextRenderer.MeasureText(g, menuItemData.Text, fontsong, new Size(0, 0), TextFormatFlags.NoPadding);
                maxCellWidth = Math.Max(maxCellWidth, size.Width + imgOff + 11);
            }
            fontsong.Dispose();
            g.Dispose();
            tempImg.Dispose();

            CellWidth = (int)maxCellWidth;
            int index = 0;
            foreach (var menuItemData in datas)
            {
                menuItemData.Position = new Point((index % columnCount) * CellWidth, (index / columnCount) * CellHeight);
                menuItemData.Size = new Size(CellWidth, CellHeight);
                index++;
            }

         //   Width = columnCount * CellWidth;
       //     Height = ((index + columnCount - 1) / columnCount) * CellHeight;
        }

        public void Click()
        {
            if (selectIndex < 0)
                return;

            if (OnClick != null)
                OnClick(datas[selectIndex].Command);
        }

        public void Draw(Graphics g)
        {
            Font fontsong = new Font("宋体", 10 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            int index = 0;
            foreach (var menuItemData in datas)
            {
                g.FillRectangle(selectIndex == menuItemData.Id-1 ? Brushes.DeepSkyBlue : Brushes.Black,
                        menuItemData.Position.X + baseX, menuItemData.Position.Y + baseY, menuItemData.Size.Width, menuItemData.Size.Height);

                var imgOff = 0;
                if (menuItemData.Icon != "")
                {
                    var iconImg = HSIcons.GetIconsByEName(menuItemData.Icon);
                    g.DrawImage(iconImg, menuItemData.Position.X + 3 + baseX, menuItemData.Position.Y + 2 + baseY, 14, 14);
                    imgOff += 16;
                }

                using (Brush b = new SolidBrush(Color.FromName(menuItemData.Color)))
                    g.DrawString(menuItemData.Text, fontsong, b, imgOff + 3 + menuItemData.Position.X + baseX, 2 + menuItemData.Position.Y + baseY);

                index++;
            }
            fontsong.Dispose();
        }
    }
}