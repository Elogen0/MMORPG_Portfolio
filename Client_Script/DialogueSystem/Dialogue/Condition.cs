using System.Collections;
using System.Collections.Generic;
using Kame.Core;
using UnityEngine;

namespace Kame.Dialogue
{
    //Conjunction : A && B ...
    //Disjunction : A || B ...
    // 모든 술어논리는 형태로 바꿀수 있다 (A || B) && (C || D)
    // ex ) HQ && (!CQ || !(HI || CO))
    // =   HQ && (!CQ || (!HI && !CO)
    // =   HQ && (!CQ || !HI) && (!CQ ||!CO)
    [System.Serializable]
    public class Condition
    {
        //논리곱
        [SerializeField] private Disjunction[] and;
        
        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (Disjunction disjunction in and)
            {
                if (!disjunction.Check(evaluators))
                    return false;
            }
            return true;
        }
        
        //논리합
        [System.Serializable]
        class Disjunction
        {
            [SerializeField] private Predicate[] or;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (Predicate predicate in or)
                {
                    if (predicate.Check(evaluators))
                        return true;
                }
                return false;
            }
        }

        //술어 논리
        [System.Serializable]
        class Predicate
        {
            [SerializeField] private string predicate;
            [SerializeField] private string[] parameters;
            [SerializeField] private bool negate = false;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (var evaluator in evaluators)
                {
                    bool? result = evaluator.Evaluate(predicate, parameters);
                    if (result == null)
                        continue;

                    if (result == negate)
                        return false;
                }

                return true;
            }
        }

        
        
    }
 
}
