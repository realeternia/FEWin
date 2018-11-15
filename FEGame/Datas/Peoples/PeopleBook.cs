using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using FEGame.Core.Loader;
using FEGame.Forms.Items.Regions;
using FEGame.Tools;
using NarlonLib.Log;
using NarlonLib.Tools;

namespace FEGame.Datas.Peoples
{
    internal static class PeopleBook
    {
        private static Dictionary<string, int> itemNameIdDict;

        public static int GetPeopleId(string ename)
        {
            if (itemNameIdDict == null)
            {
                itemNameIdDict = new Dictionary<string, int>();
                foreach (var peopleConfig in ConfigData.PeopleDict.Values)
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
            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage(PaintTool.GetTalkColor);
            tipData.AddTextNewLine(peopleConfig.Name, "White", 20);
            tipData.AddTextNewLine(string.Format("{0}级{1}", peopleConfig.Level, ""), "White");
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

        public static bool IsPeople(int id)
       {
           PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
           return peopleConfig.Type > 0 && peopleConfig.Type < 100; 
        }

        public static bool IsMonster(int id)
        {
            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
            return peopleConfig.Type == 0;
        }

        public static Image GetPersonImage(int id)
        {
            string fname = string.Format("People/{0}.PNG", ConfigData.GetPeopleConfig(id).Figue);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("People", string.Format("{0}.PNG", ConfigData.GetPeopleConfig(id).Figue));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static int[] GetRandNPeople(int count, int minLevel, int maxLevel)
        {
            List<int> pids = new List<int>();
            foreach (PeopleConfig peopleConfig in ConfigData.PeopleDict.Values)
            {
                if (IsPeople(peopleConfig.Id) && peopleConfig.Level >= minLevel && peopleConfig.Level <= maxLevel)
                    pids.Add(peopleConfig.Id);
            }

            ArraysUtils.RandomShuffle(pids);
            return pids.GetRange(0, count).ToArray();
        }

        public static void Fight(int pid, string map, int rlevel, PictureRegion.HsActionCallback winEvent, PictureRegion.HsActionCallback lossEvent, PictureRegion.HsActionCallback cancelEvent)
        {

        }
    }
}
