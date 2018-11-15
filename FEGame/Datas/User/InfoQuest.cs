using System.Collections.Generic;
using ConfigDatas;
using FEGame.Core;
using FEGame.Datas.Items;
using FEGame.Datas.User.Db;
using FEGame.Forms.CMain;
using NarlonLib.Log;

namespace FEGame.Datas.User
{
    public class InfoQuest
    {
        [FieldIndex(Index = 1)] public List<DbQuestData> QuestRunning; //Ŀǰ���е�������id
        [FieldIndex(Index = 2)] public List<int> QuestFinish;

        public InfoQuest()
        {
            QuestRunning = new List<DbQuestData>();
            QuestFinish = new List<int>();
        }

        public bool IsQuestNotReceive(int qid)
        {
            var questRun = QuestRunning.Find(q => q.QuestId == qid);
            var questFin = QuestFinish.Find(q => q == qid);
            return questFin <= 0 && questRun == null;
        }

        public bool IsQuestCanReceive(int qid)
        {
            if (QuestFinish.Contains(qid))
                return false;
            if (QuestRunning.Find(q => q.QuestId == qid) != null)
                return false;
            var questConfig = ConfigData.GetQuestConfig(qid);
            if (questConfig.Former > 0 && !IsQuestFinish(questConfig.Former))
                return false;
            return true;
        }

        public bool IsQuestCanProgress(int qid)
        {
            var questData = QuestRunning.Find(q => q.QuestId == qid);
            if (questData != null)
                return questData.State == (int)QuestStates.Receive && questData.Progress < 10;
            return false;
        }

        public int GetQuestProgress(int qid)
        {
            var questData = QuestRunning.Find(q => q.QuestId == qid);
            if (questData != null && questData.State == (int)QuestStates.Receive)
                return questData.Progress;
            return 0;
        }

        public bool IsQuestCanReward(int qid)
        {
            var questData = QuestRunning.Find(q => q.QuestId == qid);
            if (questData != null)
            {
                var questConfig = ConfigData.GetQuestConfig(qid);
                if (!string.IsNullOrEmpty(questConfig.RequireItem))
                    return UserProfile.InfoBag.GetItemCount(HItemBook.GetItemId(questConfig.RequireItem)) > 0;
                if (questConfig.RequireSceneId > 0)
                    return UserProfile.InfoBasic.MapId == questConfig.RequireSceneId;
                return questData.State == (int)QuestStates.Accomplish;
            }
            return false;
        }

        public bool IsQuestFinish(int qid)
        {
            return QuestFinish.Contains(qid);
        }

        public void SetQuestState(int qid, QuestStates state)
        {
            if (qid <= 0)
            {
                NLog.Warn("SetQuestState state qid==0");
                return;
            }

            var questRun = QuestRunning.Find(q => q.QuestId == qid);
            var questFin = QuestFinish.Find(q => q == qid);
            if (state == QuestStates.Receive)
            {
                if (IsQuestCanReceive(qid))
                {
                    var questData = new DbQuestData
                    {
                        QuestId = qid,
                        State = (byte) QuestStates.Receive
                    };
                    QuestRunning.Add(questData);

                    OnReceiveQuest(qid);
                }
            }
            else if (state == QuestStates.Finish)
            {
                if (IsQuestCanReward(qid) && questFin == 0)
                {
                    QuestRunning.Remove(questRun);
                    QuestFinish.Add(qid);

                    OnFinishQuest(qid);
                }
            }
            else
            {
                if (questRun != null && questRun.State < (byte)state)
                    questRun.State = (byte) state;
            }
        }

        public void AddQuestProgress(int qid, byte progress)
        {
            var questRun = QuestRunning.Find(q => q.QuestId == qid);
            if (questRun.State == (int)QuestStates.Receive)
            {
                questRun.Progress += progress;
                var questConfig = ConfigData.GetQuestConfig(qid);
                if (questRun.Progress >= 10)
                {
                    questRun.Progress = 10;
                    questRun.State = (int) QuestStates.Accomplish;
                    MainTipManager.AddTip(string.Format("������-{0}", questConfig.Name), "White");
                }
                else
                {
                    MainTipManager.AddTip(string.Format("�������-{0} {1}%", questConfig.Name, questRun.Progress*10), "White");
                }
            }
        }

        private void OnReceiveQuest(int qid)
        {
            var questConfig = ConfigData.GetQuestConfig(qid);

            MainTipManager.AddTip(string.Format("��������-{0}", questConfig.Name), "White");
            SoundManager.Play("System", "QuestActivateWhat1.mp3");
        }

        private void OnFinishQuest(int qid)
        {
            var questConfig = ConfigData.GetQuestConfig(qid);
            if (!string.IsNullOrEmpty(questConfig.RequireItem))
                UserProfile.InfoBag.DeleteItem(HItemBook.GetItemId(questConfig.RequireItem), 1);

            MainTipManager.AddTip(string.Format("�������-{0}", questConfig.Name), "White");
            UserProfile.InfoRecord.AddRecordById(RecordInfoConfig.Indexer.QuestFinish, 1);
            SoundManager.Play("System", "QuestCompleted.mp3");
        }

        public void OnSceneQuestSuccess(string questName, bool partial)
        {
            if (partial)
                return;

            foreach (var runQuest in QuestRunning)
            {
                var config = ConfigData.GetQuestConfig(runQuest.QuestId);
                if (IsQuestCanProgress(runQuest.QuestId) && config.SuccessSceneQuest == questName)
                    AddQuestProgress(runQuest.QuestId, (byte) config.ProgressAdd);
            }
        }

        public void OnSwitchScene(bool isWarp)
        {
            if (isWarp)
                ResetQuest();
        }

        public void OnLogout()
        {
          //  ResetQuest();
        }

        public void ResetQuest()
        {
            var resetList = new List<int>();
            foreach (var dbQuestData in QuestRunning)
            {
                var questConfig = ConfigData.GetQuestConfig(dbQuestData.QuestId);
                if (questConfig.ResetOnLeave)
                    resetList.Add(questConfig.Id);
            }
            foreach (var questId in resetList)
                QuestRunning.RemoveAll(quest => questId == quest.QuestId);
        }
    }
}
