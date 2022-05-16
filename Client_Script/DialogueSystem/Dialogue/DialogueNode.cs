using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Core;
using UnityEngine;
using UnityEditor;

namespace Kame.Dialogue
{
    [Serializable]
    public class DialogueSentenceDelay
    {
        public int position;
        public float delayTime;
    }
    
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private bool isChoiceNode = false;
        [SerializeField] private string speakerName;
        [TextArea(3, 10)]
        [SerializeField] private string sentence;
        // todo : [SerializeField] private DialogueSentenceDelay _sentenceDelay; 
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private string onEnterAction;
        [SerializeField] private string onExitAction;
        [SerializeField] private Condition condition;
        //todo Condition        
        public string SpeakerName => speakerName;   
        
        public string Sentence => sentence;  
        public List<string> Children => children;
        public bool IsChoiceNode => isChoiceNode;           
        public string OnEnterAction => onEnterAction;
        public string OnExitAction => onExitAction;
        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators) => condition.Check(evaluators);
#if UNITY_EDITOR
        [Header("Node Position and Size")]
        [SerializeField] private Rect rect = new Rect(0, 0, 200, 100);
        public Rect GetRect() => rect;

        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }
        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }

        public void SetChoiceNode(bool isChoice)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            isChoiceNode = isChoice;
            EditorUtility.SetDirty(this);
        }
#endif
    } 
}
