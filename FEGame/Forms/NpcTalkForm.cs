﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using ControlPlus.Drawing;
using FEGame.Core.Loader;
using FEGame.DataType;
using FEGame.DataType.Others;
using FEGame.DataType.Peoples;
using FEGame.DataType.Scenes;
using FEGame.DataType.User;
using FEGame.Forms.CMain;
using FEGame.Forms.CMain.Quests;
using FEGame.Forms.CMain.Quests.SceneQuests;
using FEGame.Forms.Items.Core;
using FEGame.Forms.Items.Regions;

namespace FEGame.Forms
{
    internal sealed partial class NpcTalkForm : BasePanel
    {
        private int tar = -1;
        private bool showImage;
        private SceneQuestBlock interactBlock;

        private ColorWordRegion colorWord;//问题区域
        private VirtualRegion vRegion;
        private SceneQuestConfig config;
        private List<SceneQuestBlock> answerList; //回答区
        private TalkEventItem evtItem; //事件交互区
        private ImageToolTip tooltip = SystemToolTip.Instance;

        public int EventId { get; set; }
        public int CellId { get; set; } //格子id
        private int eventLevel;

        private Dictionary<int, string> dnaChangeDict = new Dictionary<int, string>();

        public NpcTalkForm()
        {
            InitializeComponent();
            NeedBlackForm = true;
            colorWord = new ColorWordRegion(340, 38+10, Width-350, new Font("微软雅黑", 13 * 1.33f, GraphicsUnit.Pixel), Color.White);
            vRegion = new VirtualRegion(this);
            vRegion.RegionEntered += VRegion_RegionEntered;
            vRegion.RegionLeft += VRegion_RegionLeft;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            showImage = true;
            config = ConfigData.GetSceneQuestConfig(EventId);
            if (config.Level > 0)
                eventLevel = config.Level;
            else
                eventLevel = ConfigData.GetSceneConfig(UserProfile.InfoBasic.MapId).Level;

            int regionIndex = 1;
            if (config.TriggerDNAHard != null && config.TriggerDNAHard.Length > 0)
            {
                for (int i = 0; i < config.TriggerDNAHard.Length; i ++)
                {
                    var dnaId = DnaBook.GetDnaId(config.TriggerDNAHard[i]);
                    if (UserProfile.InfoBasic.HasDna(dnaId))
                    {
                        vRegion.AddRegion(new ImageRegion(dnaId, 28*regionIndex,55, 24,24, ImageRegionCellType.None, DnaBook.GetDnaImage(dnaId)));
                        dnaChangeDict[dnaId] = "事件难度 " + config.TriggerDNAHard[i].Substring(3);
                        //dnaChangeDict[dnaId] += "$经验资源 " + GetDnaStr(-int.Parse(config.TriggerDNAHard[i + 1]));
                        regionIndex++;
                    }
                }
            }
            if (config.TriggerDNARate != null && config.TriggerDNARate.Length > 0)
            {
                for (int i = 0; i < config.TriggerDNARate.Length; i ++)
                {
                    var dnaId = DnaBook.GetDnaId(config.TriggerDNARate[i]);
                    if (UserProfile.InfoBasic.HasDna(dnaId))
                    {
                        var dataStr = "出现几率 " + config.TriggerDNARate[i].Substring(3);
                        if (dnaChangeDict.ContainsKey(dnaId))
                        {
                            dnaChangeDict[dnaId] += "$" + dataStr;
                        }
                        else
                        {
                            vRegion.AddRegion(new ImageRegion(dnaId, 28 * regionIndex, 55, 24, 24, ImageRegionCellType.None, DnaBook.GetDnaImage(dnaId)));
                            dnaChangeDict[dnaId] = dataStr;
                        }
                        regionIndex++;
                    }
                }
            }
            interactBlock = SceneQuestBook.GetQuestData(this, EventId, eventLevel, config.Script);
            answerList = new List<SceneQuestBlock>();
            SetupQuestItem();
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);

            if (evtItem != null && evtItem.RunningState == TalkEventItem.TalkEventState.Running)
            {
                evtItem.OnFrame(tick);
                if (evtItem.RunningState == TalkEventItem.TalkEventState.Finish)
                {
                    interactBlock = evtItem.GetResult();
                    if (interactBlock is SceneQuestSay)
                        SetupQuestItem();
                    else if (interactBlock == null)
                    {
                        answerList.Clear();
                        var block = new SceneQuestAnswer(this, EventId, eventLevel, "结束", 999, 999);
                        AddBlockAnswer(block);
                    }
                }
                Invalidate();
            }

            if (colorWord != null)
            {
                colorWord.OnFrame(tick, this);
            }
        }

        private void NpcTalkForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (tar != -1)
            {
                if (interactBlock == null)//一般是最后一选了
                {
                    Close();
               //     Scene.Instance.OnEventEnd(CellId, config.Id, evtItem != null ? evtItem.Type : "");
               //     Scene.Instance.CheckALiveAndQuestState();
                    return;
                }
                
                if (evtItem != null && evtItem.RunningState == TalkEventItem.TalkEventState.Running)
                {
                    //事件过程中无视点击
                    return;
                }

                interactBlock = answerList[tar]; //对话换页

                if (evtItem != null)
                {
                    evtItem.Dispose();
                    evtItem = null;
                }
           
                if (interactBlock.Children.Count == 1 && interactBlock.Children[0] is SceneQuestSay)
                {
                    interactBlock = interactBlock.Children[0];
                    SetupQuestItem();
                }
                else if (interactBlock.Children.Count == 1 && interactBlock.Children[0] is SceneQuestEvent)
                {
                    var evt = interactBlock.Children[0] as SceneQuestEvent;
                    if (evt.Type == "npc")
                        tar = -1; //为了修一个显示bug
                    var region = new Rectangle(340, Height - 10 - 3*30 - 150, Width - 350, 150);
                    evtItem = TalkEventItem.CreateEventItem(CellId, EventId, eventLevel, this, region, evt);
                    evtItem.Init();
                }

                if (evtItem != null && evtItem.AutoClose())
                {
                    Close();
            //        Scene.Instance.OnEventEnd(CellId, config.Id, evtItem != null ? evtItem.Type : "");
            //        Scene.Instance.CheckALiveAndQuestState();
                }
                this.Invalidate();
            }
        }

        private void SetupQuestItem()
        {
            Graphics g = this.CreateGraphics();
            colorWord.UpdateText(interactBlock.Script, g);
            g.Dispose();
            answerList.Clear();
            foreach (var sceneQuestBlock in interactBlock.Children)
            {
                if (sceneQuestBlock.Disabled)
                    continue;
                AddBlockAnswer(sceneQuestBlock);

                if (sceneQuestBlock.Children != null)
                {
                    var childScript = sceneQuestBlock.Children[0].Script;
                    if (childScript.StartsWith("fight")) //如果是战斗
                    {
                        sceneQuestBlock.SetScript(string.Format("|icon.abl1||{0}", sceneQuestBlock.Script));
                        if (config.CanBribe)//判断战斗贿赂
                        {
                            int fightLevel = Math.Max(1, eventLevel);
                            var cost = GameResourceBook.OutCarbuncleBribe(UserProfile.InfoBasic.Level, fightLevel);
                            if (UserProfile.InfoBag.HasResource(GameResourceType.Carbuncle, cost))
                            {
                                var questBlock = SceneQuestBook.GetQuestData(this, EventId, eventLevel, "blockbribe");
                                questBlock.SetScript(string.Format("|icon.res5||{0}|lime|(消耗{1})", questBlock.Script, cost));
                                questBlock.Children[0].Children[0].Children[0] = sceneQuestBlock.Children[0].Children[1].Children[0].Children[0];//找到成功的结果
                                AddBlockAnswer(questBlock);
                            }
                        }
                    }
                }

            }

            if (interactBlock != null && interactBlock.Depth == 0)
            {//额外的任务目标
                if (!string.IsNullOrEmpty(config.EnemyName))
                {
                    var peopleId = PeopleBook.GetPeopleId(config.EnemyName);
                    if (peopleId > 0 && PeopleBook.IsPeople(peopleId)) //是否要结交新英雄
                    {
                        var peopleConfig = ConfigData.GetPeopleConfig(peopleId);
                        string reason;
                        var result = GetRivalActivateResult(peopleConfig, out reason);
                        SceneQuestBlock questBlock;
                        if (result)
                        {
                            questBlock = SceneQuestBook.GetQuestData(this, EventId, eventLevel, "blockunlock");
                            questBlock.SetScript(string.Format("|icon.hatt8||{0}", questBlock.Script));
                        }
                        else
                        {
                            questBlock = SceneQuestBook.GetQuestData(this, EventId, eventLevel, "blockunlockfail");
                            questBlock.SetScript(string.Format("|icon.hatt8||{0}|darkred|{1}", questBlock.Script, reason));
                        }
                        AddBlockAnswer(questBlock);
                    }
                }
            }
        }

        private void AddBlockAnswer(SceneQuestBlock block)
        {
            answerList.Add(block);
            int id = 0;
            foreach (var sceneQuestBlock in answerList) //所有位置重算
            {
                var yoff = id * 30 + Height - 10 - answerList.Count * 30;
                sceneQuestBlock.SetRect(new Rectangle(360 + 10, yoff, 400, 400));
                id++;
            }
        }

        private bool GetRivalActivateResult(PeopleConfig peopleConfig, out string reason)
        {
            reason = "";
            if (peopleConfig.RivalDefeat > 0)
            {
                var winCount = 0;
                if (winCount < peopleConfig.RivalDefeat)
                {
                    reason = string.Format("(击败次数{0}/{1})", winCount, peopleConfig.RivalDefeat);
                    return false;
                }
            }
            else if (peopleConfig.RivalRecordId > 0)
            {
                var myVal = UserProfile.InfoRecord.GetRecordById(peopleConfig.RivalRecordId);
                if (myVal < peopleConfig.RivalRecordValue)
                {
                    reason = string.Format("(需求{0}(已有{1})>={2})", ConfigData.GetRecordInfoConfig(peopleConfig.RivalRecordId).Cname, myVal, peopleConfig.RivalRecordValue);
                    return false;
                }
            }
            return true;
        }

        private void NpcTalkForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (evtItem == null || evtItem.RunningState != TalkEventItem.TalkEventState.Running) //evtItem运行期间无法选择
            {
                int val = -1;
                if (e.Y > Height - 10 - answerList.Count * 30 && e.Y < Height - 10 && e.X > 360 && e.X < 360 + 400)
                {
                    val = Math.Min(answerList.Count-1, (e.Y - (Height - 10) + answerList.Count * 30) / 30);
                }
                if (val != tar)
                {
                    tar = val;
                    Invalidate();
                }
            }
        }

        private void VRegion_RegionEntered(int id, int x, int y, int key)
        {
            var dnaConfig = ConfigData.GetPlayerDnaConfig(id);
            Image image = DrawTool.GetImageByString(string.Format("{0}[DNA效果]",dnaConfig.Name), dnaChangeDict[id], 120, Color.White);
            tooltip.Show(image, this, x, y);
        }

        private void VRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void NpcTalkForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            if (showImage)
            {
                Font font2 = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                e.Graphics.DrawString(string.Format("{0}(Lv{1})({2})",config.Name, eventLevel, config.Script), font2, Brushes.White, Width / 2 - 40, 8);
                font2.Dispose();

                e.Graphics.DrawImage(SceneQuestBook.GetSceneQuestImageBig(config.Id), 20, 60, 300, 300);
                Image border = PicLoader.Read("Border", "scenequestbg.png"); //边框
                e.Graphics.DrawImage(border, 20, 60, 300, 300);
                border.Dispose();

                if (config.TriggerRate > 0 && config.TriggerRate <= 30)
                {//稀有
                    Image rareImg = PicLoader.Read("System", "sqrare2.png");
                    e.Graphics.DrawImage(rareImg, 20+16, 60+16, 64, 32);
                    rareImg.Dispose();
                }
                else if (config.TriggerRate > 0 && config.TriggerRate <= 60)
                {//罕见
                    Image rareImg = PicLoader.Read("System", "sqrare1.png");
                    e.Graphics.DrawImage(rareImg, 20 + 16, 60 + 16, 64, 32);
                    rareImg.Dispose();
                }

                if (evtItem != null)
                    evtItem.Draw(e.Graphics);

                colorWord.Draw(e.Graphics);
                if (vRegion != null)
                    vRegion.Draw(e.Graphics);

                if (answerList != null && (evtItem == null || evtItem.RunningState != TalkEventItem.TalkEventState.Running))
                {
                    int id = 0;
                    var bgTarget = PicLoader.Read("System", "WordBack1.png");
                    var bgCommon = PicLoader.Read("System", "WordBack2.png");
                    foreach (var word in answerList)
                    {
                        var rect = word.Rect;
                        var bgRect = new Rectangle(rect.X, rect.Y, rect.Width - 20, 24);
                        if (id == tar)
                            e.Graphics.DrawImage(bgTarget, bgRect);
                        else
                            e.Graphics.DrawImage(bgCommon, bgRect);

                        word.Draw(e.Graphics);
                        id++;
                    }
                    bgTarget.Dispose();
                    bgCommon.Dispose();
                }
            }
        }

    }
}