using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Kame.Core;
using Kame.Define;
using Kame.Quests;
using UnityEngine;

namespace Kame.Dialogue
{
    public class DialogueSpeaker : MonoBehaviour
    {
        [SerializeField] private Dialogue[] dialogue;
        [SerializeField] private string speakerName;
        [SerializeField] private CinemachineVirtualCamera dialogueCamera;
        public string GetName() => speakerName;
        private TransformAnchor mainCameraTransformAnchor;
        private Quaternion savedRotation = Quaternion.identity;
        private VoidEventChannelSO _conversationUpdatedEvent;
        private DialogueManager _manager;
        private void Start()
        {
            mainCameraTransformAnchor = TransformAnchor.Get<TransformAnchor>(ResourcePath.CameraTransformAnchor);
            _conversationUpdatedEvent = EventChannelSO.Get<VoidEventChannelSO>(ResourcePath.ConversationUpdated);
            _manager = DialogueManager.Instance;
        }

        public void StartDialogue(int index)
        {
            _conversationUpdatedEvent.OnEventRaised += TriggerTalkAnimation;

            _manager.StartDialogue(this.gameObject, dialogue[index]);
            savedRotation = transform.rotation;
            transform.DOLookAt(TransformAnchor.Get<TransformAnchor>(ResourcePath.PlayerTransformAnchor).Value.position, 0.3f);
            if (dialogueCamera)
                dialogueCamera.gameObject.SetActive(true);

            Transform uiCamera = mainCameraTransformAnchor.Value.GetChild(0);
            uiCamera.gameObject.SetActive(false);
        }

        public void StartDialogue(string name)
        {
            _manager.StartDialogue(gameObject, name);
            if (dialogueCamera)
                dialogueCamera.gameObject.SetActive(true);
        }

        public void EndDialogue()
        {
            _conversationUpdatedEvent.OnEventRaised -= TriggerTalkAnimation;

            transform.DORotate(savedRotation.eulerAngles, 0.3f);
            if (dialogueCamera)
                dialogueCamera.gameObject.SetActive(false);
            Transform uiCamera = mainCameraTransformAnchor.Value.GetChild(0);
            uiCamera.gameObject.SetActive(true);
        }

        public void TriggerTalkAnimation()
        {
            if (TryGetComponent(out Animator animator))
            {
                animator.SetTrigger(AnimatorHash.Talk);
            }
        }

    }
}
