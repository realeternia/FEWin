using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using FEGame.Core.Loader;
using FEGame.Forms.Items.Regions;
using FEGame.Tools;
using NarlonLib.Log;
using NarlonLib.Tools;

namespace FEGame.DataType.Samurais
{
    internal static class SamuraiBook
    {
        private static Dictionary<string, int> itemNameIdDict;

        public static int GetOneId(string ename)
        {
            if (itemNameIdDict == null)
            {
                itemNameIdDict = new Dictionary<string, int>();
                foreach (var peopleConfig in ConfigData.SamuraiDict.Values)
                {
                    if (itemNameIdDict.ContainsKey(peopleConfig.Ename))
                    {
                        NLog.Warn("GetPeopleId key={0} exsited", peopleConfig.Ename);
                        continue;
                    }
                    itemNameIdDict[peopleConfig.Ename] = peopleConfig.Id;
                }
            }
            if (itemNameIdDict.ContainsKey(ename))
                return itemNameIdDict[ename];
            return 0;
        }

        public static Image GetPreview(int id)
        {
            var peopleConfig = ConfigData.GetSamuraiConfig(id);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage(PaintTool.GetTalkColor);
            tipData.AddTextNewLine(peopleConfig.Name, "White", 20);
            tipData.AddTextNewLine(string.Format("{0}级{1}", 1, ""), "White");
            tipData.AddLine();
            //int[] attrs = JobBook.GetJobLevelAttr(peopleConfig.Job, peopleConfig.Level);
            //tipData.AddTextNewLine(string.Format("战斗 {0,3:D}  守护 {1,3:D}", attrs[0], attrs[1]), "Lime");
            //tipData.AddTextNewLine(string.Format("法术 {0,3:D}  技巧 {1,3:D}", attrs[2], attrs[3]), "Lime");
            //tipData.AddTextNewLine(string.Format("速度 {0,3:D}  幸运 {1,3:D}", attrs[4], attrs[5]), "Lime");
            //tipData.AddTextNewLine(string.Format("体质 {0,3:D}  生存 {1,3:D}", attrs[6], attrs[7]), "Lime");
            //tipData.AddLine();
            //tipData.AddTextNewLine("王牌", "White", 30);
            //foreach (int cid in peopleConfig.Cards)
            //{
            //    tipData.AddImage(MonsterBook.GetMonsterImage(cid, 40, 40), 28);
            //}
            return tipData.Image;
        }

        public static Image GetImage(int id)
        {
            string fname = string.Format("Samurai/{0}.png", ConfigData.GetSamuraiConfig(id).Figue);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Samurai", string.Format("{0}.png", ConfigData.GetSamuraiConfig(id).Figue));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static void Fight(int pid, string map, int rlevel, PictureRegion.HsActionCallback winEvent, PictureRegion.HsActionCallback lossEvent, PictureRegion.HsActionCallback cancelEvent)
        {

        }
    }
}
