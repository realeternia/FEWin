﻿using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using FEGame.Core.Loader;
using FEGame.Forms.CMain;
using FEGame.Forms.Items.Core;
using FEGame.Forms.Items.Regions;
using FEGame.Forms.Items.Regions.Decorators;

namespace FEGame.Forms
{
    internal sealed partial class MinigameForm : BasePanel
    {
        private VirtualRegion vRegion;

        public MinigameForm()
        {
            InitializeComponent();

            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            vRegion = new VirtualRegion(this);
            int id = 0;
            foreach (var minigameConfig in ConfigData.MinigameDict.Values)
            {
                var region = new ButtonRegion(minigameConfig.Id, 20 + (id%8)*65, 40 + (id/8)*65, 50, 50,
                    minigameConfig.IconPath + ".PNG",
                    minigameConfig.IconPath + "On.PNG");
                region.AddDecorator(new RegionTextDecorator(0, 42, 8, minigameConfig.Name));
                vRegion.AddRegion(region);
                id++;
            }
            vRegion.RegionClicked += VRegion_RegionClicked;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
        }
        
        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void VRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.Left && id > 0)
                PanelManager.ShowGameWindow(id, 0, null);
        }

        private void MinigameForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 游戏 ", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (vRegion != null)
                vRegion.Draw(e.Graphics);
        }
    }

}