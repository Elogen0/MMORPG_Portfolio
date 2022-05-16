using Google.Protobuf.Protocol;
using InflearnServer.DB;
using InflearnServer.Game.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace InflearnServer.Game
{
    public class Quest
    {
        private Dictionary<int, QuestRecord> _onGoingQuests = new Dictionary<int, QuestRecord>();
        private Dictionary<int, QuestRecord> _rewardedQuests = new Dictionary<int, QuestRecord>();

        public Dictionary<int, QuestRecord> OnGoingQuests => _onGoingQuests;

        public Dictionary<int, QuestRecord> RewardedQuests => _rewardedQuests;

        private Player player;

        public Quest(Player player)
        {
            this.player = player;
        }

        public QuestRecord Add(QuestDb questDb)
        {
            if (HasQuest(questDb.TemplateId))
                return null;
            
            var record = QuestRecord.MakeRecord(questDb);
            if (record.Status == QuestStatus.QuestRewarded)
                _rewardedQuests.Add(record.Id, record);
            else if (record.Status == QuestStatus.QuestAccepted || record.Status == QuestStatus.QuestCompleted)
                _onGoingQuests.Add(record.Id, record);
            return record;
        }

        public void AcceptQuest(int questId)
        {
            if (HasQuest(questId))
                return;
            DBTransaction.AcceptQuest(player, questId, player.Room);
        }

        public void QuitQuest(int questId)
        {
            QuestRecord record = GetRecord(questId);
            if (record == null)
                return;
            if (record.Status == QuestStatus.QuestRewarded || record.Status == QuestStatus.QuestNone)
                return;

            _onGoingQuests.Remove(questId);
            record.Status = QuestStatus.QuestNone;
            DbQuestStatusChange(record);
            SendQuestStatus(record);
        }

        public bool HasQuest(int questId)
        {
            return (_onGoingQuests.ContainsKey(questId) || _rewardedQuests.ContainsKey(questId));
        }

        public QuestRecord GetRecord(int questId)
        {
            if (_onGoingQuests.ContainsKey(questId))
            {
                return _onGoingQuests[questId];
            }
            if (_rewardedQuests.ContainsKey(questId))
            {
                return _rewardedQuests[questId];
            }
            return null;
        }

        public void ProcessQuest(QuestType type, int targetId, int count)
        {
            foreach (var pair in _onGoingQuests)
            {
                QuestRecord record = pair.Value;
                if (record.Status == QuestStatus.QuestRewarded || record.Status == QuestStatus.QuestNone)
                    continue;
                if (!record.HasObjective(type, targetId))
                    continue;
                int progress = record.ProcessObjective(count);
                Console.WriteLine($"ProcessQuest({record.Id}) : { count }");

                QuestDb questDb = new QuestDb();
                questDb.QuestDbId = record.DbId;
                questDb.Progress = progress;

                using (AppDbContext db = new AppDbContext())
                {
                    db.Entry(questDb).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                    db.Entry(questDb).Property(nameof(QuestDb.Progress)).IsModified = true;

                    bool success = db.SaveChangesEx();

                    if (!success)
                    {
                        //예외처리
                        Console.WriteLine("notsucess");
                    }
                    else
                    {
                        Console.WriteLine("success");
                    }
                }

                if (record.Status == QuestStatus.QuestAccepted && record.IsObjectiveComplete())
                {
                    record.Status = QuestStatus.QuestCompleted;
                    questDb.Status = QuestStatus.QuestCompleted;
                    DbQuestStatusChange(record);
                    SendQuestStatus(record);
                }
                else if (record.Status == QuestStatus.QuestCompleted && !record.IsObjectiveComplete())
                {
                    record.Status = QuestStatus.QuestAccepted;
                    questDb.Status = QuestStatus.QuestAccepted;
                    DbQuestStatusChange(record);
                    SendQuestStatus(record);
                }
            }
        }

        public void GiveQuestReward(int questId)
        {
            QuestRecord record = GetRecord(questId);
            if (record == null)
                return;

            if (!record.IsObjectiveComplete())
                return;
            
            DBTransaction.GiveQuestReward(player, record, player.Room);
        }

        public void ToRewared(QuestRecord record)
        {
            int questId = record.Id;
            _onGoingQuests.Remove(questId);
            _rewardedQuests.Add(questId, record);
            record.Status = QuestStatus.QuestRewarded;
            SendQuestStatus(record);
        }

        public void SendQuestStatus(QuestRecord record)
        {
            S_QuestStatus questPacket = new S_QuestStatus();
            questPacket.QuestInfo = new QuestInfo();
            questPacket.QuestInfo.MergeFrom(record.Info);
            player.Session.Send(questPacket);
        }

        public void DbQuestStatusChange(QuestRecord record)
        {
            QuestDb questDb = new QuestDb()
            {
                QuestDbId = record.DbId,
                Status = record.Status
            };
            using (AppDbContext db = new AppDbContext())
            {
                db.Entry(questDb).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                db.Entry(questDb).Property(nameof(QuestDb.Status)).IsModified = true;

                bool success = db.SaveChangesEx();
                if (!success)
                {
                    //예외처리
                }
            }
        }
    }
}
