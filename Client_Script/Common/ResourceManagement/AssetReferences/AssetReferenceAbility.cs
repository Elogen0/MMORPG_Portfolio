using Kame.Abilities;
using UnityEngine.AddressableAssets;

namespace Kame.Abilities
{
    [System.Serializable]
    public class AssetReferenceAbility : AssetReferenceT<Ability>
    {
        public AssetReferenceAbility(string guid) : base(guid)
        {
        }
    }    
}

