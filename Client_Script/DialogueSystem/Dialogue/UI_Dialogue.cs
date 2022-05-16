using System;
using System.Collections;
using Kame.Define;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kame.Dialogue
{
    public class UI_Dialogue : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI speakerName;
        [SerializeField] private TextMeshProUGUI displayTextUI;
        [SerializeField] private Button nextButton;
        [SerializeField] private GameObject responseGroup;
        [SerializeField] private Transform choiceGroup;
        [SerializeField] private GameObject choiceButtonPrefab;
        [SerializeField] private Button quitButton;

        private bool _isTyping = false;
        private string _nowSentence = string.Empty;
        private float _elapsedClickTime = 0f;
        private const float _mouseClickInterval = 0.5f;
        private WaitForSeconds _typingInterval = new WaitForSeconds(0.05f);
        private WaitForSeconds _startTimeDelay = new WaitForSeconds(0.25f);
        private VoidEventChannelSO _conversationUpdatedEvent;
        private UIViewEventChannelSO _uiViewEventChannel;

        private DialogueManager _manager;
        
        #region MonoBehaviour
        private void Awake()
        {
            _conversationUpdatedEvent = EventChannelSO.Get<VoidEventChannelSO>(ResourcePath.ConversationUpdated);
            _uiViewEventChannel = EventChannelSO.Get<UIViewEventChannelSO>(ResourcePath.UIViewEventChannel);
        }

        void Start()
        {
            _conversationUpdatedEvent.OnEventRaised += OnConversationUpdated;
            //nextButton.onClick.AddListener(() => _manager.Next());
            _manager = DialogueManager.Instance;
            quitButton.onClick.AddListener(() => _manager.Quit());
            OnConversationUpdated();
        }

        private void OnDestroy()
        {
            _conversationUpdatedEvent.OnEventRaised -= OnConversationUpdated;
        }

        private void Update()
        {
            if (!_manager.IsTalking || _manager.IsChoosing)
                return;
            
            _elapsedClickTime += Time.deltaTime;
            if (_elapsedClickTime < _mouseClickInterval)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                _elapsedClickTime = 0f;
                // if (_isTyping)
                // {
                //     StopAllCoroutines();
                //     displayTextUI.text = _nowSentence;
                //     _isTyping = false;
                // }
                // else
                {
                    if (_manager.HasNext())
                    {
                        _manager.Next();
                    }
                    else
                    {
                        _manager.Quit();
                    }
                }
            }
            
        }
        #endregion

        void OnConversationUpdated()
        {
            if (_manager.IsTalking && !gameObject.activeSelf)
            {
                _uiViewEventChannel.Show("Dialogue");
            }
            else if (!_manager.IsTalking && gameObject.activeSelf)
            {
                _uiViewEventChannel.Home();
            }
            
            // gameObject.SetActive(DialogueManager.Instance.IsTalking);
            if (!_manager.IsTalking)
            {
                return;
            }

            //responseGroup.SetActive(!manager.IsChoosing);
            choiceGroup.gameObject.SetActive(_manager.IsChoosing);
            if (_manager.IsChoosing)
            {
                BuildChoiceList();
            }
            else
            {
                Debug.Log(_manager.GetSentence());
                speakerName.text = _manager.CurrentSpeakerName;
                displayTextUI.text = _manager.GetSentence();
                //nextButton.gameObject.SetActive(_manager.HasNext());
            }
        }

        private void BuildChoiceList()
        {
            //이전 버튼 지우기
            choiceGroup.RemoveAllChildren();

            foreach (DialogueNode choice in _manager.GetChoiceNodes())
            {
                GameObject choiceInstance = Instantiate(choiceButtonPrefab, choiceGroup);
                var buttonText = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = choice.Sentence;
                choiceInstance.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    _manager.SelectChoice(choice);
                });
            }
        }

        IEnumerator CoTypingSentence(string sentence)
        {
            speakerName.text = string.Empty;
            _nowSentence = sentence;
            _isTyping = true;
            yield return _startTimeDelay;

            for (int i = 0; i < sentence.Length; i++)
            {
                displayTextUI.text = sentence.Substring(0, i);
                yield return _typingInterval;
            }

            _isTyping = false;
        }
    }
}