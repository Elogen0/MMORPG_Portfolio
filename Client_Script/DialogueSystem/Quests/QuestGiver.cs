using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Google.Protobuf.Protocol;
using Kame.Core;
using Kame.Define;
using Kame.Dialogue;
using Kame.Game;
using UnityEngine;
using UnityEngine.Networking;

namespace Kame.Quests
{
    public class QuestGiver : MonoBehaviour, IPredicateEvaluator
    {
        [SerializeField] private int[] questIds;

        private GameObject _currentMark;
        private void Start()
        {
            StartCoroutine(RefreshMark());
        }

        private void OnEnable()
        {
            QuestManager.Instance.OnQuestStatusChanged += OnQuestStatusChanged;
        }

        private void OnDisable()
        {
            if (!QuestManager.shuttingDown)
                QuestManager.Instance.OnQuestStatusChanged -= OnQuestStatusChanged;
        }
        
        public void GiveQuest(int questId)
        {
            foreach (var id in questIds)
            {
                if (questId == id)
                {
                    QuestManager.Instance.SendAcceptQuest(questId);
                }
            }

            StartCoroutine(RefreshMark());
        }

        public void GiveReward(int questId)
        {
            foreach (var id in questIds)
            {
                if (id == questId)
                {
                    QuestManager.Instance.SendGiveQuestReward(questId);
                    break;
                }
            }
        }
        
        private void OnQuestStatusChanged(QuestRecord completeQuestRecord)
        {
            if (Array.Exists(questIds, questId => questId == completeQuestRecord.Id))
            {
                StartCoroutine(RefreshMark());
            }
        }

        private IEnumerator RefreshMark()
        {
            QuestStatus status = QuestStatus.QuestRewarded;
            foreach (var questId in questIds)
            {
                QuestRecord record = QuestManager.Instance.GetRecord(questId);
                if (record == null)
                    continue;
                if (record.Status == QuestStatus.QuestRewarded || record.Status == QuestStatus.QuestAccepted)
                    continue;
                if (record.Status == QuestStatus.QuestNone)
                {
                    status = QuestStatus.QuestNone;
                    //todo : 퀘스트 받을 수 있는 조건이면 노란색 물음표
                    continue;
                }

                if (record.Status == QuestStatus.QuestCompleted)
                {
                    status = QuestStatus.QuestCompleted;
                    break;
                }
            }

            DestroyMark();
            if (status == QuestStatus.QuestRewarded)
                yield break;
            
            if (status == QuestStatus.QuestCompleted)
            {
                var r = AddressableLoader.InstantiatePooling("Assets/Game/Prefab/Logic/ExclamationMark.prefab");
                yield return r.Wait();
                _currentMark = r.Result;
                SetMarkPosition(r.Result);
            }
            else if (status == QuestStatus.QuestNone)
            {
                var r = AddressableLoader.InstantiatePooling("Assets/Game/Prefab/Logic/QuestionMark.prefab");
                yield return r.Wait();
                _currentMark = r.Result;
                SetMarkPosition(r.Result);
            }
            yield return null;
        }

        private void SetMarkPosition(GameObject go)
        {
            var position = transform.position;
            position = new Vector3(position.x, position.y + GetComponent<Collider>().bounds.size.y, position.z);
            go.transform.position = position;
        }

        private void DestroyMark()
        {
            if (_currentMark)
            {
                AddressableLoader.ReleaseInstance(_currentMark);
                _currentMark = null;
            }
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch (predicate)
            {
                case "HasQuest":
                {
                    int questId = int.Parse(parameters[0]);
                    return QuestManager.Instance.HasQuest(questId);
                }
                case "CompleteQuest":
                {
                    int questId = int.Parse(parameters[0]);
                    return QuestManager.Instance.IsQuestCompleted(questId);
                }
            }
            return null;
        }
    }
}
