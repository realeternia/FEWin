using System.Drawing;
using FEGame.Datas;
using FEGame.Datas.Scenes;
using FEGame.Datas.User;
using FEGame.Forms.CMain.Quests.SceneQuests;

namespace FEGame.Forms.CMain.Quests
{
    internal class TalkEventItemAction : TalkEventItem
    {
        private int cellId;

        public TalkEventItemAction(int cellId, int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            this.cellId = cellId;
        }

        public override void Init()
        {
            base.Init();
            switch (evt.Type)
            {
                case "quest": UserProfile.InfoQuest.SetQuestState(int.Parse(evt.ParamList[0]), QuestStates.Receive); break;
                case "questp": UserProfile.InfoQuest.AddQuestProgress(int.Parse(evt.ParamList[0]), byte.Parse(evt.ParamList[1])); break;
                case "removeditem": var itemId = DungeonBook.GetDungeonItemId(config.NeedDungeonItemId);
                    UserProfile.InfoDungeon.RemoveDungeonItem(itemId, config.NeedDungeonItemCount); break;
            }

            if (evt.Children.Count > 0)
                result = evt.Children[0];//应该是一个say
        }

        public override bool AutoClose()
        {
            return result == null; //没有后续就自动关闭
        }
    }
}

