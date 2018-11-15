using System.Drawing;

namespace FEGame.Core.Interface
{
    public interface IRegionDecorator
    {
        void SetState(object info);
        void Draw(Graphics g, int x, int y, int width, int height);
    }
}
