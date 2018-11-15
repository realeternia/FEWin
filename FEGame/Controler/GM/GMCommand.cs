using System;
using FEGame.Datas;
using FEGame.Datas.Scenes;
using FEGame.Datas.User;
using FEGame.Forms;
using FEGame.Forms.CMain;

namespace FEGame.Controler.GM
{
    internal class GMCommand
    {
        public static bool IsCommand(string c)
        {
            switch (c)
            {
                case "exp": break;
                case "cad": break;
                case "atp": break;
                case "mov": break;
                case "eqp": break;
                case "eqps": break;
                case "itm": break;
                case "emys": break;
                case "gold": break;
                case "res": break;
                case "dmd": break;
                case "acv": break;
                case "view": break;
                case "fbat": break;
                case "cbat": break;
                case "scr": break;
                case "sceq": break;
                case "cure":  break;
                case "bls":  break;
                case "qst": break;
                default: return false;
            }
            return true;
        }

        public static void ParseCommand(string cmd)
        {
            string[] data = cmd.Split(' ');
            if (data.Length == 0) return;
            try
            {
                switch (data[0])
                {
                    case "exp": if (data.Length == 2) UserProfile.InfoBasic.AddExp(int.Parse(data[1])); break;
                    case "itm": if (data.Length == 3) UserProfile.InfoBag.AddItem(int.Parse(data[1]), int.Parse(data[2])); break;
                    case "gold": if (data.Length == 2)
                        {
                            UserProfile.InfoBag.AddResource(GameResourceType.Gold, uint.Parse(data[1]));
                        } break;
                    case "res": if (data.Length == 2)
                    {
                        var v = uint.Parse(data[1]);
                            UserProfile.InfoBag.AddResource(new uint[] { 0, v, v, v, v, v, v });
                        } break;
                    case "dmd": if (data.Length == 2) UserProfile.InfoBag.AddDiamond(int.Parse(data[1])); break;
                    case "acv": if (data.Length == 2) UserProfile.Profile.InfoGismo.AddGismo(int.Parse(data[1])); break;
                    case "sceq":
                        NpcTalkForm sw = new NpcTalkForm();
                        sw.EventId = SceneQuestBook.GetSceneQuestByName(data[1]);
                        PanelManager.DealPanel(sw); break;
                    case "cure": UserProfile.InfoBasic.MentalPoint=100; UserProfile.InfoBasic.HealthPoint=100;
                        UserProfile.InfoBasic.FoodPoint = 100;break;
                    case "qst":if (data.Length == 2)
                        UserProfile.InfoQuest.SetQuestState(int.Parse(data[1]), QuestStates.Receive); break;
                }
            }
            catch (FormatException) { }
            catch (IndexOutOfRangeException) { }
        }
    }
}
