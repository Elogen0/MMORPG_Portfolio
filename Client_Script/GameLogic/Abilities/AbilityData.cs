using Kame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.Abilities
{
    public class AbilityData : IAction
    {
        public GameObject User { get; set; }
        public Vector3 TargetedPoint { get; set; }
        public IEnumerable<GameObject> Targets { get; set; }

        bool cancelled = false;

        public AbilityData(GameObject user)
        {
            this.User = user;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            User.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
        }

        public void Cancel()
        {
            cancelled = true;
        }

        public bool IsCancelled()
        {
            return cancelled;
        }
    }

}