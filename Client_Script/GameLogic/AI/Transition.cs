using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.AI
{
    [System.Serializable]
    public class Transition
    {
        public DecisionStrategy decision;
        public StateSO trueState;
        public StateSO falseState;
    } 
}