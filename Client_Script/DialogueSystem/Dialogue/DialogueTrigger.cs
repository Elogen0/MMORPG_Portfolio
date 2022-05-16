using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Kame.Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private Trigger[] triggers;
        
        [System.Serializable] 
        public class Trigger
        {
            public string id;
            public UnityEvent OnTrigger;
        }

        public void Execute(string triggerId)
        {
            foreach (var trigger in triggers)
            {
                if (trigger.id == triggerId)
                {
                    trigger.OnTrigger?.Invoke();
                }
            }
        }
    } 
}
