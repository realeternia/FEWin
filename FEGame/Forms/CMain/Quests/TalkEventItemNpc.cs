using System.Drawing;
using FEGame.Forms.CMain.Quests.SceneQuests;

namespace FEGame.Forms.CMain.Quests
{
    internal class TalkEventItemNpc : TalkEventItem
    {
        private bool autoClose;
        public TalkEventItemNpc(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            autoClose = true;
        }

        public override void Init()
        {
     
            inited = true;
        }

        public override bool AutoClose()
        {
            return autoClose;
        }
    }
}

