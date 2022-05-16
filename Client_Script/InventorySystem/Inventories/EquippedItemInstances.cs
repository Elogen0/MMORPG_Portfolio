using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 컴바인된 아이템들의 리스트를 관리
/// </summary>
public class EquippedItemInstances
{
    public List<Transform> itemTransforms = new List<Transform>();

    public void OnDestroy()
    {
        foreach (Transform item in itemTransforms)
        {
            GameObject.Destroy(item.gameObject);
        }
    }
}
