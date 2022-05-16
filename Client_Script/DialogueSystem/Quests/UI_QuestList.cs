using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using Kame.Define;
using Kame.Quests;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

namespace Kame.Quests
{
    public class UI_QuestList : UI_Base
    {
        private IResourceLocation uiQuestLocation;
        private Dictionary<int, UI_QuestItem> cache = new Dictionary<int, UI_QuestItem>();

        enum GameObjects
        { 
            ItemContainer,
            quest_detail,
        }
        public override void Init()
        {
            AddressableLoader.GetLocationAsync("Assets/Game/Prefab/UI/Contents/btn_quest_item.prefab", result =>
            {
                uiQuestLocation = result;
            });
            Bind<GameObject>(typeof(GameObjects));
        }
        
        private void OnEnable()
        {
            ReDrawAll();
            QuestManager.Instance.OnProcessQuest += RedrawItem;
        }

        private void OnDisable()
        {
            if (!QuestManager.shuttingDown)
                QuestManager.Instance.OnProcessQuest -= RedrawItem;
        }

        private void RedrawItem(QuestRecord questRecord)
        {
            StartCoroutine(CoRedrawItem(questRecord));
        }

        private IEnumerator CoRedrawItem(QuestRecord questRecord)
        {
            switch (questRecord.Status)
            {
                case QuestStatus.QuestAccepted:
                case QuestStatus.QuestCompleted:
                    if (!cache.ContainsKey(questRecord.Id))
                    {
                        var request = AddressableLoader.InstantiatePooling(uiQuestLocation, GetObject((int) GameObjects.ItemContainer).transform);
                        yield return request.Wait();
                        var result = request.Result;
                        UI_QuestItem item = result.GetComponent<UI_QuestItem>();
                        item.Setup(questRecord);
                        if (result.TryGetComponent(out Button button))
                        {
                            button.onClick.AddListener(()=>
                            {
                                 ViewDetail(questRecord);
                            });
                        }
                        cache[questRecord.Id] = item;
                    }
                    break;
                case QuestStatus.QuestRewarded:
                case QuestStatus.QuestNone:
                    if (cache.ContainsKey(questRecord.Id))
                    {
                        AddressableLoader.ReleaseInstance(cache[questRecord.Id].gameObject);
                    }
                    break;
                default:
                    if (cache.ContainsKey(questRecord.Id))
                    {
                        cache[questRecord.Id].Setup(questRecord);
                    }
                    break;
            }
            yield return null;
        }

        private void ReDrawAll()
        {
            foreach (var item in cache.Values)
            {
                AddressableLoader.ReleaseInstance(item.gameObject);
            }
            cache.Clear();
            
            List<QuestRecord> questList = QuestManager.Instance.OnGoingQuests.Values.ToList();
            questList.Sort((a, b) => a.Info.AcceptDate.CompareTo(b.Info.AcceptDate));
            foreach (var quest in questList)
            {
                RedrawItem(quest);
            }
        }

        private void ViewDetail(QuestRecord record)
        {
            Get<GameObject>((int)GameObjects.quest_detail).GetComponent<UI_QuestTooltip>().Setup(record);
        }
        
    }    
}

