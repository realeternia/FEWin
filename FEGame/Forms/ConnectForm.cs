using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using FEGame.Core.Loader;
using FEGame.Forms.Items.Core;

namespace FEGame.Forms
{
    internal partial class ConnectForm : BasePanel
    {
        public ConnectForm()
        {
            InitializeComponent();
        }

        private void ConnectForm_Load(object sender, EventArgs e)
        {
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "closebutton1.jpg");
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (textBoxTitle.Text != "" || textBoxContent.Text != "")
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.Host = "smtp.163.com";
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("hscscard", "11111111");
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage("hscscard@163.com", "narlonzjh@163.com");
                message.Subject = textBoxTitle.Text;
                message.Body = textBoxContent.Text;
                String version = FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
                message.Body += String.Format("[�汾��:{0}]", version);
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                try
                {
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"Send Email Failed." + ex);
                }
            }
            Close();
        }

        private void textBoxContent_MouseEnter(object sender, EventArgs e)
        {
            if (textBoxContent.Text == @"��ϣ���õ��ظ���������ͨ�ŵ�ַ��лл��")
            {
                textBoxContent.Text = "";
            }
        }

        private void ConnectForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("����", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("��ϵ��", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}