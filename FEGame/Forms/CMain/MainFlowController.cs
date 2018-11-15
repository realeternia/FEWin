﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FEGame.Core;

namespace FEGame.Forms.CMain
{
    /// <summary>
    /// 主scene的飘字系统
    /// </summary>
    internal class MainFlowController
    {
        private Font font;
        class FlowData
        {
            public string Text;
            public Color Color;
            public int X;
            public int Y;
            public int Height;
            public int Width;
            public int Time;
            public string IconName;
        }

        private Control parent;
        private List<FlowData> flowList;

        public MainFlowController(Control form)
        {
            parent = form;
            font = new Font("宋体", 13, FontStyle.Bold);
            flowList = new List<FlowData>();
        }

        public void CheckTick()
        {
            foreach (var flowData in flowList)
            {
                flowData.Time --;
                flowData.Y -= 3;
                parent.Invalidate(new Rectangle(flowData.X, flowData.Y, flowData.Width, flowData.Height+3));
            }
            flowList.RemoveAll(f => f.Time <= 0);
        }

        public void Add(string txt, string icon, Color clr, Point pos)
        {
            FlowData flowData = new FlowData();
            flowData.Text = txt;
            flowData.IconName = icon;
            flowData.Color = clr;
            flowData.X = pos.X;
            flowData.Y = pos.Y;
            flowData.Width = GetStringWidth(txt);
            if (!string.IsNullOrEmpty(icon))
                flowData.Width += 22;
            flowData.Height = 22;
            flowData.Time = 16 + txt.Length / 2;
            if (flowList.Count > 0)
            {
                var lastItem = flowList[flowList.Count - 1];
                if (Math.Abs(lastItem.X - pos.X) < 20 && Math.Abs(pos.Y - lastItem.Y) < 20)
                    flowData.Y = lastItem.Y + 20;
            }
            flowList.Add(flowData);
        }

        private static int GetStringWidth(string s)
        {
            double wid = 0;
            foreach (char c in s)
            {
                if (c >= '0' && c <= '9')
                    wid += 14.20594;
                else
                    wid += 19.98763;
            }
            return (int)wid;
        }

        public void DrawAll(System.Drawing.Graphics g)
        {
            foreach (var flowData in flowList)
            {
                var xoff = 0;
                if (!string.IsNullOrEmpty(flowData.IconName))
                {
                    var icon = HSIcons.GetIconsByEName(flowData.IconName);
                    g.DrawImage(icon, flowData.X, flowData.Y, 20, 20);
                    xoff += 22;
                }

                var brush = new SolidBrush(flowData.Color);
                g.DrawString(flowData.Text, font, Brushes.Black, flowData.X + xoff + 1, flowData.Y + 1);
                g.DrawString(flowData.Text, font, brush, flowData.X+ xoff, flowData.Y);
                brush.Dispose();
            }
        }
    }
}
