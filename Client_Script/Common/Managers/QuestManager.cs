using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Google.Protobuf.Protocol;
using Kame.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kame.Quests
{
    public class QuestManager : SingletonMono<QuestManager>
    {
        private Dictionary<int, QuestRecord> _onGoingQuests = new Dictionary<int, QuestRecord>();
        private Dictionary<int, QuestRecord> _rewardedQuests = new Dictionary<int, QuestRecord>();
        public event Action<QuestRecord> OnProcessQuest;
        public event Action<QuestRecord> OnQuestStatusChanged;

        public Dictionary<int, QuestRecord> OnGoingQuests => _onGoingQuests;
        public Dictionary<int, QuestRecord> RewardedQuests => _rewardedQuests;

        public static string Path = "Managers/QuestManager";

// #if UNITY_EDITOR
//         [MenuItem("Tools/Managers/QuestManager")]
//         static void Create()
//         {
//             GameUtil.Editors.CreateAsset<QuestManager>("Assets/Resources/Managers/", "QuestManager.asset");
//         }
// #endif
//         private static QuestManager _instance;
//         public static QuestManager Instance
//         {
//             get
//             {
//                 if (_instance == null)
//                 {
//                     _instance = ResourceLoader.Load<QuestManager>("Managers/QuestManager");
//                     _instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
//                 }
//                 return _instance;
//             }
//         }
        public void Init(S_QuestList questPacket)
        {
            foreach (var questInfo in questPacket.QuestList)
            {
                if (HasQuest(questInfo.TemplateId))
                    continue;

                var record = QuestRecord.MakeRecord(questInfo);
                if (record.Status == QuestStatus.QuestRewarded)
                    _rewardedQuests.Add(record.Id, record);
                else if (record.Status == QuestStatus.QuestAccepted || record.Status == QuestStatus.QuestCompleted)
                    _onGoingQuests.Add(record.Id, record);
                OnQuestStatusChanged?.Invoke(record);
            }
        }

        public bool HasQuest(int questId)
        {
            return (_onGoingQuests.ContainsKey(questId) || _rewardedQuests.ContainsKey(questId));
        }

        public bool IsQuestCompleted(int questId)
        {
            if (_onGoingQuests.TryGetValue(questId, out QuestRecord record))
            {
                return record.IsObjectiveComplete();
            }
            return false;
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
                OnProcessQuest?.Invoke(record);
            }
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
        
        #region Send
        private void SendQuestStatus(QuestStatus status, int questId)
        {
            C_QuestStatus packet = new C_QuestStatus
            {
                Status = status,
                QuestId = questId,
            };
            NetworkManager.Instance.Send(packet);
        }
        
        public void SendAcceptQuest(int questId)
        {
            if (HasQuest(questId))
                return;

            SendQuestStatus(QuestStatus.QuestAccepted, questId);
        }

        
        public void SendQuitQuest(int questId)
        {
            QuestRecord record = GetRecord(questId);
            if (record == null)
                return;
            if (record.Status == QuestStatus.QuestRewarded || record.Status == QuestStatus.QuestNone)
                return;
            SendQuestStatus(QuestStatus.QuestNone, questId);
        }
        
        public bool SendGiveQuestReward(int questId)
        {
            QuestRecord record = GetRecord(questId);
            if (record == null)
                return false;

            if (!record.IsObjectiveComplete())
                return false;
            
            SendQuestStatus(QuestStatus.QuestRewarded, questId);
            return true;
        }
        #endregion
        
        #region Receive
        public void RecvQuestPacket(QuestInfo info)
        {
            Debug.Log($"QuestStatus Changed({info.TemplateId}) : {info.Status.ToString()}");
            switch (info.Status)
            {
                case QuestStatus.QuestNone:
                    RecvQuitQuest(info);
                    break;
                case QuestStatus.QuestAccepted:
                    RecvAcceptQuest(info);
                    break;
                case QuestStatus.QuestCompleted:
                    RecvQuestCompleted(info);
                    break;
                case QuestStatus.QuestRewarded:
                    RecvGiveQuestReward(info);
                    break;
            }
        }
        
        private void RecvAcceptQuest(QuestInfo info)
        {
            if (!HasQuest(info.TemplateId))
            {
                QuestRecord record = QuestRecord.MakeRecord(info);
                _onGoingQuests.Add(record.Id, record);
                OnQuestStatusChanged?.Invoke(record);    
            }
            else
            {
                QuestRecord record = GetRecord(info.TemplateId);
                record.Info.MergeFrom(info);
                OnQuestStatusChanged?.Invoke(record);    
            }
        }

        private void RecvQuitQuest(QuestInfo info)
        {
            QuestRecord record = GetRecord(info.TemplateId);
            if (record == null)
                return;
            _onGoingQuests.Remove(info.TemplateId);
            record.Status = QuestStatus.QuestNone;
            OnQuestStatusChanged?.Invoke(record);
        }
        
        
        private void RecvGiveQuestReward(QuestInfo info)
        {
            QuestRecord record = GetRecord(info.TemplateId);
            if (record == null)
                return;
            record.Info.MergeFrom(info);
            _onGoingQuests.Remove(record.Id);
            if (!_rewardedQuests.ContainsKey(record.Id))
                _rewardedQuests.Add(record.Id, record);
            OnQuestStatusChanged?.Invoke(record);
        }

        private void RecvQuestCompleted(QuestInfo info)
        {
            var record = GetRecord(info.TemplateId);
            if (record == null)
                return;
            record.Info.MergeFrom(info);
            OnQuestStatusChanged?.Invoke(record);    
        }
        #endregion
    }

}
