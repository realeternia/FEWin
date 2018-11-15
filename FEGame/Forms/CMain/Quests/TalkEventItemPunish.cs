using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using FEGame.DataType;
using FEGame.DataType.Others;
using FEGame.DataType.User;
using FEGame.Forms.CMain.Quests.SceneQuests;
using FEGame.Forms.Items.Regions;

namespace FEGame.Forms.CMain.Quests
{
    internal class TalkEventItemPunish : TalkEventItem
    {
        private delegate void PunishAction(ref int index);
        private Control parent;
        private VirtualRegion vRegion; 
        private ImageToolTip tooltip = SystemToolTip.Instance;

        public TalkEventItemPunish(int evtId, int level, Control c, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            parent = c;
            vRegion = new VirtualRegion(parent);
            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;
        }

        public override void Init()
        {
            int index = 1;
            DoPunish(ref index, "gold", GetMulti(), PunishGold);
            DoPunish(ref index, "food", GetMulti(), PunishFood);
            DoPunish(ref index, "health", GetMulti(), PunishHealth);
            DoPunish(ref index, "mental", GetMulti(), PunishMental);

            if (evt.Children.Count > 0)
                result = evt.Children[0];//应该是一个say

            inited = true;
        }

        private void DoPunish(ref int index, string type, int times, PunishAction action)
        {
            if (IsBonusAvail(type))
            {
                for (int i = 0; i < times; i++)
                    action(ref index);    
            }
        }

        private bool IsBonusAvail(string tp)
        {
            foreach (var item in evt.ParamList)
            {
                if (item == tp)
                    return true;
                if (item != "x2" && item != "x3")
                    return false;
            }
            return true;
        }

        private int GetMulti()
        {
            foreach (var item in evt.ParamList)
            {
                if (item == "x2")
                    return 2;
                if (item == "x3")
                    return 3;
            }
            return 1;
        }

        #region 各种惩罚
        
        private void PunishMental(ref int index)
        {
            var mentalLoss = (uint) (GameResourceBook.OutMentalSceneQuest(config.PunishMental)*(10 + hardness)/10);
            if (mentalLoss > 0)
            {
                UserProfile.Profile.InfoBasic.SubMental(mentalLoss);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25), 60, ImageRegionCellType.Mental, (int) -mentalLoss);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        private void PunishHealth(ref int index)
        {
            var healthLoss = (uint)(GameResourceBook.OutHealthSceneQuest(config.PunishHealth) * (10 + hardness) / 10);
            if (healthLoss > 0)
            {
                UserProfile.Profile.InfoBasic.SubHealth(healthLoss);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25), 60, ImageRegionCellType.Health, (int) -healthLoss);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        private void PunishFood(ref int index)
        {
            var foodLoss = (uint)(GameResourceBook.OutFoodSceneQuest(config.PunishFood) * (10 + hardness) / 10);
            if (foodLoss > 0)
            {
                UserProfile.Profile.InfoBasic.SubFood(foodLoss);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Food, (int) -foodLoss);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        private void PunishGold(ref int index)
        {
            var goldLoss = (uint)(GameResourceBook.OutGoldSceneQuest(level, config.PunishGold) * (10 + hardness) / 10);
            if (goldLoss > 0)
            {
                UserProfile.Profile.InfoBag.SubResource(GameResourceType.Gold, goldLoss);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Gold, (int) -goldLoss);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        #endregion

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            var region = vRegion.GetRegion(id);
            if (region != null)
                region.ShowTip(tooltip, parent, x, y);
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent);
        }

        public override void Draw(Graphics g)
        {
           // g.DrawRectangle(Pens.White, pos);

            Font font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString("惩罚", font, Brushes.White, pos.X + 3, pos.Y + 3);
            font.Dispose();

            g.DrawLine(Pens.Orange, pos.X + 3, pos.Y + 3 + 20, pos.X + 3+400, pos.Y + 3 + 20);

            vRegion.Draw(g);
        }

        public override void Dispose()
        {
            vRegion.Dispose();
        }
    }
}

