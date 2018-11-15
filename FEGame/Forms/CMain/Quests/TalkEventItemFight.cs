using System.Drawing;
using FEGame.Datas.User;
using FEGame.Forms.CMain.Quests.SceneQuests;
using FEGame.Forms.Items.Regions;

namespace FEGame.Forms.CMain.Quests
{
    internal class TalkEventItemFight : TalkEventItem
    {
        private bool isEndFight = false;

        public TalkEventItemFight(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {

        }

        public override void Init()
        {
            PictureRegion.HsActionCallback winCallback = OnWin;
            PictureRegion.HsActionCallback failCallback = OnFail;

        }

        private void OnFail()
        {
            result = evt.ChooseTarget(0);
            isEndFight = true;

            UserProfile.InfoDungeon.FightLoss ++;
        }

        private void OnWin()
        {
            result = evt.ChooseTarget(1);
            isEndFight = true;

            UserProfile.InfoDungeon.FightWin++;
            UserProfile.InfoGismo.CheckWinCount();
        }

        public override void OnFrame(int tick)
        {
            if (isEndFight)
            {
                RunningState = TalkEventState.Finish;
                isEndFight = false;
            }
        }
    }
}

