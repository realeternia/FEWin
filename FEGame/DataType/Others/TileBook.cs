using System;
using System.Drawing;
using ConfigDatas;
using FEGame.Core.Loader;
using FEGame.Tools;

namespace FEGame.DataType.Others
{
    internal static class TileBook
    {
        public static Image GetTileImage(int id, int width, int height)
        {
            string fname = string.Format("Tiles/{0}{1}x{2}", id, width, height);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Tiles", string.Format("{0}.jpg", ConfigData.GetTileConfig(id).Icon));
                if (image.Width != width || image.Height != height)
                    image = image.GetThumbnailImage(width, height, null, new IntPtr(0));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }
    }

}
