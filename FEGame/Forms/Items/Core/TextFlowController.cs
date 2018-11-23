using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FEGame.Forms.Items.Core
{
    internal class TextFlowController
    {
        internal class FlowData
        {
            public string Text;
            public Image Icon;
            public string Color;
            public int X;
            public int Y;
            public int Time;
        }

        private List<FlowData> flows = new List<FlowData>();

        public void Update(Control c)
        {

            if (flows.Count > 0)
            {
                FlowData[] datas = flows.ToArray();
                foreach (var flowData in datas)
                {
                    flowData.Time--;
                    if (flowData.Time < 11)
                        flowData.Y -= 2 + (12 - flowData.Time) / 4 * 3;
                }

                foreach (var flowData in datas)
                {
                    if (flowData.Time < 0)
                        flows.Remove(flowData);
                }
                c.Invalidate();
            }
        }

        public void Draw(Graphics g)
        {
            if (flows.Count > 0)
            {
                Font ft = new Font("宋体", 18 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                foreach (var flowData in flows.ToArray())
                {
                    if (flowData.Time >= 0)
                    {
                        int realX = flowData.X;
                        if (flowData.Icon != null)
                        {
                            g.DrawImage(flowData.Icon, flowData.X, flowData.Y - 4, 32, 32);
                            realX += 35;
                        }
                        Color color = Color.FromName(flowData.Color);
                        SolidBrush sb = new SolidBrush(color);
                        g.DrawString(flowData.Text, ft, (color.R + color.G + color.B) > 50 ? Brushes.Black : Brushes.White, realX, flowData.Y);
                       g.DrawString(flowData.Text, ft, sb, realX - 1, flowData.Y - 1);
                        sb.Dispose();
                    }
                }
                ft.Dispose();
            }
        }

        public void AddFlow(string text, string color, Image img, int x, int y)
        {
            FlowData newFlow = new FlowData
            {
                Text = text,
                Color = color,
                Icon = img,
                X = x,
                Y = y,
                Time = 16 + text.Length / 2
            };

            foreach (var word in flows)
            {//避让
                if (Math.Abs(word.X - newFlow.X) < 50 && Math.Abs(word.Y - newFlow.Y) < 20)
                    newFlow.Y = word.Y + 20;
            }

            flows.Add(newFlow);
        }
    }
}