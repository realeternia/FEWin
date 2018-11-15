using System;
using System.Windows.Forms;
using FEGame.Core;
using FEGame.Core.Loader;
using JLM.NetSocket;
using NarlonLib.Log;

namespace FEGame
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            NLog.Start(LogTargets.File);
            LogHandlerRegister.Log = NLog.DebugDirect;
            DataLoader.Init();
            PicLoader.Init(); 
            SoundManager.Init();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}