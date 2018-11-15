using ControlPlus;

namespace FEGame.Forms.CMain
{
    internal class SystemToolTip
    {
        private static ImageToolTip tooltip = new ImageToolTip();

        public static ImageToolTip Instance
        {
            get { return tooltip; }
        }
    }
}
