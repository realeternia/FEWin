using System;
using System.Drawing;
using ConfigDatas;
using FEGame.Core.Loader;
using FEGame.Tools;

namespace FEGame.Core
{
    internal static class HSIcons
    {
        public static Image GetIconsByEName(string name)
        {
            string fname = string.Format("Icon/{0}.png", name);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Icon", string.Format("{0}.png", name));
                ImageManager.AddImage(fname, image, true);
            }
            return ImageManager.GetImage(fname);
        }

        public static Image GetImage(string tableName, int id)
        {
            string url = "";
            if (tableName == "Dna")
            {
                url = ConfigData.GetPlayerDnaConfig(id).Url;
            }
            else if (tableName == "Samurai")
            {
                url = ConfigData.GetSamuraiConfig(id).Figue;
            }
            else if (tableName == "Job")
            {
                url = ConfigData.GetJobConfig(id).Icon;
            }
            else
            {
                throw new ApplicationException(string.Format("not implement tablename({0}) in HSIcons.GetImage", tableName));
            }

            string fname = string.Format("{0}/{1}.png", tableName, url);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read(tableName, string.Format("{0}.png", url));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static Image GetSystemImage(string name)
        {
            string fname = string.Format("System/{0}.png", name);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("System", string.Format("{0}.png", name));
                ImageManager.AddImage(fname, image, true);
            }
            return ImageManager.GetImage(fname);
        }
    }
}
