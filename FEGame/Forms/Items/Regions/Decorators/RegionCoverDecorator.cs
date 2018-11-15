using System.Drawing;
using FEGame.Core.Interface;

namespace FEGame.Forms.Items.Regions.Decorators
{
    internal class RegionBorderDecorator : IRegionDecorator
    {
        private Color lineColor;

        public RegionBorderDecorator( Color color)
        {
            lineColor = color;
        }

        public void SetState(object info)
        {            
        }

        public void Draw(Graphics g, int x, int y, int width, int height)
        {
            using (Pen pen = new Pen(lineColor))
            {
                g.DrawRectangle(pen, x, y, width-1, height-1);
            }
        }
    }
}
