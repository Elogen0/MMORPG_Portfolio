using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kame.Core;
using Kame.Define;
using Kame.Quests;
using Kame.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kame.Dialogue
{
    public class DialogueManager : SingletonMono<DialogueManager>
    {
        private Dialogue _currentDialogue = null;
        private DialogueNode _currentNode;
        private string _currentSentence;
        private GameObject _dialogueSpeaker = null;

        public bool IsChoosing { get; private set; } = false;
        public bool IsTalking => _currentDialogue != null;
        public string CurrentSpeakerName => _currentNode.SpeakerName;

        private AsyncOperationHandle _handle;
        private BoolEventChannelSO _controlLockEvent;
        private VoidEventChannelSO _conversationUpdatedEvent;

// #if UNITY_EDITOR
//         [MenuItem("Tools/Managers/DialogueManager")]
//         static void Create()
//         {
//             GameUtil.Editors.CreateAsset<DialogueManager>("Assets/Resources/Managers/", "DialogueManager.asset");
//         }
// #endif
        
        // private static DialogueManager _instance;
        // public static DialogueManager Instance
        // {
        //     get
        //     {
        //         if (_instance == null)
        //         {
        //             _instance = ResourceLoader.Load<DialogueManager>("Managers/DialogueManager");
        //             _instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
        //         }
        //         return _instance;
        //     }
        // }
        
        public string GetSentence()
        {
            if (_currentDialogue == null || _currentNode == null)
                return string.Empty;
                
            return  _currentNode.Sentence;
        }

        protected void Awake()
        {
            _controlLockEvent = EventChannelSO.Get<BoolEventChannelSO>(ResourcePath.ControlLock);
            _conversationUpdatedEvent = EventChannelSO.Get<VoidEventChannelSO>(ResourcePath.ConversationUpdated);
        }

        public void StartDialogue(GameObject caller, Dialogue dialogue)
        {
            _dialogueSpeaker = caller;
            _currentDialogue = dialogue;
            _currentNode = _currentDialogue.GetRootNode();
            TriggerEnterAction();

            _conversationUpdatedEvent.RaiseEvent();
            _controlLockEvent.RaiseEvent(true);
        }

        public void StartDialogue(GameObject caller, string dialogueName)
        {
            string dialoguePath = AssetPath.Dialogue + dialogueName;
            Addressables.LoadAssetAsync<Dialogue>(dialoguePath).Completed += handle =>
            {
                _handle = handle;
                StartDialogue(caller, handle.Result);
            };
        }

        public void Quit()
        {
            if (_dialogueSpeaker.TryGetComponent(out DialogueSpeaker speaker))
            {
                speaker.EndDialogue();
            }
            AddressableLoader.Release(_handle);
            _currentDialogue = null;
            TriggerExitAction();
            _currentNode = null;
            _currentSentence = "";
            IsChoosing = false;
            _dialogueSpeaker = null;
            
            _controlLockEvent.RaiseEvent(false);
            _conversationUpdatedEvent.RaiseEvent();
        }

        public bool HasNext()
        {
            return FilterOnCondition(_currentDialogue.GetAllChildren(_currentNode)).Count() > 0;
        }

        public void Next()
        {
            //선택노드라면
            int numberPlayerResponses = GetChoiceNodes().Count();
            if (numberPlayerResponses > 0)
            {
                IsChoosing = true;
                TriggerExitAction();
                _conversationUpdatedEvent.RaiseEvent();
                return;
            }

            //대화노드라면
            DialogueNode[] children = FilterOnCondition(_currentDialogue.GetConversationChildren(_currentNode)).ToArray();
            int randomIndex = UnityEngine.Random.Range(0, children.Length);
            TriggerExitAction();
            _currentNode = children[randomIndex];
            TriggerEnterAction();
            _conversationUpdatedEvent.RaiseEvent();
        }
        
        public IEnumerable<DialogueNode> GetChoiceNodes()
        {
            return FilterOnCondition(_currentDialogue.GetChoiceChildren(_currentNode));
        }
        
        public void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            TriggerEnterAction();
            IsChoosing = false;
            Next();
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (var node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return _dialogueSpeaker.GetComponents<IPredicateEvaluator>();
        }
        
        private void TriggerEnterAction()
        {
            if (_currentNode != null)
            {
                ExecuteTrigger(_currentNode.OnEnterAction);
            }
        }
        
        private void TriggerExitAction()
        {
            if (_currentNode != null)
            {
                ExecuteTrigger(_currentNode.OnExitAction);
            }
        }

        private void ExecuteTrigger(string triggerId)
        {
            if (triggerId == String.Empty)
                return;
            
            foreach (var trigger in _dialogueSpeaker.GetComponents<DialogueTrigger>())
            {
                trigger.Execute(triggerId);
            }
        }
       
    }   
}
