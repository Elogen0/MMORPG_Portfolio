using System.Collections;
using System.Collections.Generic;
using Kame.Game.Data;
using Kame;
using Kame.Items;
using UnityEngine;

[CreateAssetMenu(fileName = "New EquipItem", menuName = "Kame/InventorySystem/EquipItem", order = 1)]
public class EquipItemObject : ItemObject
{
    public GameObject equippedPrefab;
    public List<string> boneNames = new List<string>();
    private void OnValidate()
    {
        boneNames.Clear();

        if (equippedPrefab == null)
            return;
        
        if (equippedPrefab.GetComponentInChildren<SkinnedMeshRenderer>() == null)
        {
            //boneNames.Add(equippedPrefab.transform.root.ToString());
        }
        else
        {
            ExtractBoneNames();
        }
    }
    
    private void ExtractBoneNames()
    {
        Transform[] bones = equippedPrefab.GetComponentInChildren<SkinnedMeshRenderer>().bones;
        foreach (Transform t in bones)
        {
            //본네임 추출
            boneNames.Add(t.name);
        }
    }

    public override void Use(Item item, GameObject User)
    {
        base.Use(item, User);
        // if (User.TryGetComponent(out EquimentMeshApplier equipment))
        // {
        //     
        // }
    }
}
