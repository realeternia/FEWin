using System.Drawing;

namespace FEGame.DataType.Effects.Facts
{
    /// <summary>
    /// 静态的特效，适合外部ui面板使用
    /// </summary>
    internal class StaticUIImageEffect : StaticUIEffect
    {
        public Image imgD;

        public StaticUIImageEffect(Effect effect, Image img, Point location, Size size)
            : base(effect, location, size)
        {
            imgD = img;
        }

        public override void Draw(Graphics g)
        {
            if (frameId >= 0 && frameId < effect.Frames.Length)
            {
                int x = Point.X;
                int y = Point.Y;
                effect.Frames[frameId].Draw(g, x, y, Size.Width, Size.Height, imgD);
            }
        }
    }
}
