namespace FEGame.Forms.CMain
{
    partial class PopMenuBase
    {
        /// <summary>
        /// ����������������
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// ������������ʹ�õ���Դ��
        /// </summary>
        /// <param name="disposing">���Ӧ�ͷ��й���Դ��Ϊ true������Ϊ false��</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows ������������ɵĴ���

        /// <summary>
        /// �����֧������ķ��� - ��Ҫ
        /// ʹ�ô���༭���޸Ĵ˷��������ݡ�
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PopMenuDeck
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.DoubleBuffered = true;
            this.Name = "PopMenuDeck";
            this.Size = new System.Drawing.Size(154, 215);
            this.MouseLeave += new System.EventHandler(this.PopMenuBase_MouseLeave);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PopMenuBase_Paint);
            this.Click += new System.EventHandler(this.PopMenuBase_Click);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopMenuBase_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
