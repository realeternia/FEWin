using ControlPlus;
using System.Windows.Forms;

namespace FEGame.Forms
{
    sealed partial class BattleForm
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
            this.bitmapButtonClose = new ControlPlus.BitmapButton();
            this.doubleBuffedPanel1 = new ControlPlus.DoubleBuffedPanel();
            this.SuspendLayout();
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.IconImage = null;
            this.bitmapButtonClose.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonClose.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonClose.ImageNormal = null;
            this.bitmapButtonClose.Location = new System.Drawing.Point(741, 5);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 27;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.TipText = null;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // doubleBuffedPanel1
            // 
            this.doubleBuffedPanel1.Location = new System.Drawing.Point(25, 38);
            this.doubleBuffedPanel1.Name = "doubleBuffedPanel1";
            this.doubleBuffedPanel1.Size = new System.Drawing.Size(950, 650);
            this.doubleBuffedPanel1.TabIndex = 28;
            this.doubleBuffedPanel1.Click += new System.EventHandler(this.doubleBuffedPanel1_Click);
            this.doubleBuffedPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.doubleBuffedPanel1_Paint);
            this.doubleBuffedPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BattleForm_MouseDown);
            this.doubleBuffedPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BattleForm_MouseMove);
            this.doubleBuffedPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BattleForm_MouseUp);
            // 
            // BattleForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.doubleBuffedPanel1);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "BattleForm";
            this.Size = new System.Drawing.Size(1000, 700);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.BattleForm_Paint);
            this.Controls.SetChildIndex(this.bitmapButtonClose, 0);
            this.Controls.SetChildIndex(this.doubleBuffedPanel1, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private DoubleBuffedPanel doubleBuffedPanel1;
    }
}