using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.Abilities.Filters
{
    [CreateAssetMenu(fileName = "New TagFilter", menuName = "Abilities/Filters/TagFilter", order = 0)]
    public class TagFilter : FilterStrategy
    {
        [SerializeField] string tagToFilter = "";
        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter)
        {

            foreach (var gameObject in objectsToFilter)
            {
                if (gameObject.CompareTag(tagToFilter))
                {
                    yield return gameObject;
                }
            }
        }
    }
}