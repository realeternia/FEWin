using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FEGame.Core;
using FEGame.DataType.Effects.Facts;
using FEGame.Forms.CMain;
using FEGame.Forms.Items.Core;
using NarlonLib.Tools;

namespace FEGame.Forms
{
    internal class BasePanel : UserControl
    {
        delegate void BasePanelMessageCallback(int token);
        public void BasePanelMessageSafe(int token)
        {
            if (InvokeRequired)
            {
                BasePanelMessageCallback d = BasePanelMessageSafe;
                Invoke(d, new object[] { token });
            }
            else
            {
                BasePanelMessageWork(token);
            }
        }

        protected virtual void BasePanelMessageWork(int token)
        {
        }

        private Panel panel1;
        private long lastMouseMoveTime;

        protected int formWidth;
        protected int formHeight;

        public bool IsChangeBgm { get; private set; }

        public bool NeedBlackForm { get; set; }
        public BasePanel ParentPanel; //黑化用

        private TextFlowController textFlow;
        private EffectRunController effectRunBase; //动态特效
        protected bool canClose = true;

        public BasePanel()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 0;
            this.panel1.Visible = false;
            // 
            // BasePanel
            // 
            this.Controls.Add(this.panel1);
            this.Name = "BasePanel";
            this.ResumeLayout(false);

            effectRunBase = new EffectRunController();
            textFlow = new TextFlowController();
        }

        public virtual void Init(int width, int height)
        {
            formWidth = width;
            formHeight = height;
            Location = new Point((formWidth - Width) / 2, (formHeight - Height) / 2);

            if (Controls.ContainsKey("bitmapButtonClose"))
                Controls["bitmapButtonClose"].Location = new Point(Width - 35, 2);
            if (Controls.ContainsKey("bitmapButtonHelp"))
                Controls["bitmapButtonHelp"].Location = new Point(Width - 62, 2);

            Paint += new PaintEventHandler(BasePanel_Paint);
        }

        public virtual void OnFrame(int tick, float timePass)
        {
            effectRunBase.Update(this);
            textFlow.Update(this);
        }

        public virtual void OnRemove()
        {
            
        }

        public void Close()
        {
            if (canClose)
                PanelManager.RemovePanel(this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (lastMouseMoveTime + 50 > TimeTool.GetNowMiliSecond())
                return;
            lastMouseMoveTime = TimeTool.GetNowMiliSecond();

            base.OnMouseMove(e);
        }

        public virtual void OnHsKeyUp(KeyEventArgs e)
        {
        }

        public virtual void OnHsKeyDown(KeyEventArgs e)
        {
        }

        public virtual void RefreshInfo()
        {
        }


        public void AddFlowCenter(string text, string color)
        {
            textFlow.AddFlow(text, color, null, (Width - GetStringWidth(text))/2, Height/2 - 10);
        }

        public void AddFlowCenter(string text, string color, Image img)
        {
            textFlow.AddFlow(text, color, img, (Width - GetStringWidth(text)) / 2, Height / 2 - 10);
        }

        private int GetStringWidth(string s)
        {
            Graphics g = CreateGraphics();
            using (Font ft = new Font("宋体", 18 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                return (int)g.MeasureString(s, ft).Width;
            }
        }

        private void BasePanel_Paint(object sender, PaintEventArgs e)
        {
            effectRunBase.Draw(e.Graphics);
            textFlow.Draw(e.Graphics);
        }

        public void SetBgm(string bgmPath)
        {
            SoundManager.PlayBGM(bgmPath);
            IsChangeBgm = true;
        }

        public void SetBlacken(bool val)
        {
            if (val)
            {
                panel1.Width = Width;
                panel1.Height = Height;
                panel1.BackColor = Color.FromArgb(180, Color.Black);
                panel1.Visible = true;
            }
            else
            {
                panel1.Visible = false;
            }
        }

        private void AddEffect(StaticUIEffect eff)
        {
            effectRunBase.AddEffect(eff);
        }
    }

}
