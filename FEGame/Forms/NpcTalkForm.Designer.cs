﻿namespace FEGame.Forms
{
    sealed partial class NpcTalkForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // NpcTalkForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.DimGray;
            this.DoubleBuffered = true;
            this.Name = "NpcTalkForm";
            this.Size = new System.Drawing.Size(847, 397);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.NpcTalkForm_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NpcTalkForm_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.NpcTalkForm_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion





    }
}