using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Kame.Game.Data;
using UnityEngine;

namespace Kame.Quests
{
    // [Serializable]
    // public class QuestInfo
    // {
    //     public int id;
    //     public QuestStatus status;
    //     public List<int> progress = new List<int>();
    //     public DateTime acceptDate;
    // }
    
    public class QuestRecord 
    {
        public QuestInfo Info { get; private set; }
        public QuestData Data { get; private set; }

        
        public int DbId 
        {   
            get => Info.QuestDbId;
            set => Info.QuestDbId = value;
        }
        public int Id => Data.ID;
        public QuestStatus Status
        {
            get => Info.Status;
            set => Info.Status = value;
        }
        public QuestObjective Objective => Data.objective;
        public int CompletionCount => Objective.completionCount;
        public int Progress => Info.Progress;
        public List<RewardData> Rewards => Data.rewards;
        
        public bool IsObjectiveComplete() => Progress >= CompletionCount;

        public bool HasObjective(QuestType type, int targetId)
        {
            if (Objective.type == type && Objective.targetId == targetId)
                return true;
            return false;
        }

        public int ProcessObjective(int count)
        {
            return Info.Progress += count;
        }

        public static QuestRecord MakeRecord(QuestInfo questInfo)
        {
            QuestRecord newQuest = new QuestRecord
            {
                Info = questInfo,
                Data = DataManager.QuestDict[questInfo.TemplateId]
            };

            return newQuest;
        }

        public static QuestRecord MakeRecord(int questId)
        {
            QuestRecord newQuest = new QuestRecord();
            newQuest.Info = new QuestInfo();
            newQuest.Info.TemplateId = questId;
            newQuest.Info.Status = QuestStatus.QuestNone;
            //newQuest.Info.AcceptDate = DateTime.UtcNow;
            newQuest.Info.AcceptDate = Timestamp.FromDateTime(DateTime.UtcNow);

            newQuest.Data = DataManager.QuestDict[questId];
            return newQuest;
        }
    }
}
