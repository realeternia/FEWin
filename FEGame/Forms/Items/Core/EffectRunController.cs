using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FEGame.DataType.Effects.Facts;

namespace FEGame.Forms.Items.Core
{
    internal class EffectRunController
    {
        private List<StaticUIEffect> coverEffectList = new List<StaticUIEffect>();

        public void Update(Control c)
        {
            if (coverEffectList.Count > 0)
            {
                for (int i = 0; i < coverEffectList.Count; i++)
                {
                    var frameEffect = coverEffectList[i];
                    if (frameEffect != null)
                    {
                        if (frameEffect.Next())
                            c.Invalidate(new Rectangle(frameEffect.Point.X, frameEffect.Point.Y, frameEffect.Size.Width, frameEffect.Size.Height));
                    }
                }
                coverEffectList.RemoveAll(eff => eff.IsFinished == RunState.Zombie);
            }
        }

        public void AddEffect(StaticUIEffect eff)
        {
            coverEffectList.Add(eff);
        }

        public void Draw(Graphics g)
        {
            for (int i = 0; i < coverEffectList.Count; i++)
            {
                coverEffectList[i].Draw(g);
            }
        }
    }
}