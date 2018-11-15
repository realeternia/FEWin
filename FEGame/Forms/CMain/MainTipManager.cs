using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus.Drawing;
using NarlonLib.Tools;

namespace FEGame.Forms.CMain
{
    internal static class MainTipManager
    {
        private struct MainTipData
        {
            public string Word;
            public string Color;
            public long CreateTime;

            public override string ToString()
            {
                return string.Format("{0}:{1}", Color, Word);
            }
        }

        private static List<MainTipData> tipList = new List<MainTipData>();
        private static int offY;

        public static void Init(int formHeight)
        {
            offY = formHeight - 125;
        }

        public static void Refresh()
        {
            tipList.Clear();
        }

        public static void DrawAll(Graphics g)
        {
            if(tipList.Count <= 0)
                return;

            Color alphaBlack = Color.FromArgb(100, Color.Black);
            Brush alphaBBrush = new SolidBrush(alphaBlack);
            g.FillRectangle(alphaBBrush, 5, offY, 200, 90);
            alphaBBrush.Dispose();

            Font font = new Font("����", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);

            MainTipData[] datas = tipList.ToArray();
            for (int i = 0; i < datas.Length; i++)
            {
                if (datas[i].Word == "") continue;

                int xoff = 0;
                if (datas[i].Word.IndexOf('|') >= 0)
                {
                    string[] text = datas[i].Word.Split('|');
                    for (int j = 0; j < text.Length; j += 2)
                    {
                        Color color = Color.FromName(datas[i].Color);
                        if (text[j] != "")
                        {
                            color = DrawTool.GetColorFromHtml(text[j]);
                        }
                        Brush brush = new SolidBrush(color);
                        g.DrawString(text[j + 1], font, brush, 11 + xoff, offY+5 + i * 18, StringFormat.GenericTypographic);
                        brush.Dispose();
                        var len = TextRenderer.MeasureText(g, text[j + 1], font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
                        xoff += len;
                    }
                }
                else
                {
                    Brush brush = new SolidBrush(Color.FromName(datas[i].Color));
                    g.DrawString(datas[i].Word, font, brush, 11, offY+5 + i * 18, StringFormat.GenericTypographic);
                    brush.Dispose();
                }
            }
            font.Dispose();
        }

        public static void AddTip(string newtip, string color)
        {
            MainTipData sp = new MainTipData
            {
                Color = color,
                Word = newtip,
                CreateTime = TimeTool.GetNowMiliSecond()
            };
            lock (tipList)
            {
                tipList.Add(sp);
                if (tipList.Count > 5)
                {
                    tipList.RemoveAt(0);
                }
            }
            MainForm.Instance.RefreshView();
        }

        public static bool OnFrame()
        {
            long nowTick = TimeTool.GetNowMiliSecond();
            lock (tipList)
            {
                foreach (var pair in tipList)
                {
                    if (pair.CreateTime < nowTick - 10 * 1000)
                    {
                        tipList.Remove(pair);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
