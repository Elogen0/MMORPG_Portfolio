using System.Collections;
using System.Collections.Generic;
using Kame;
using UnityEngine;

public class DamageTextTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                Vector3 position = hit.point;
                Debug.Log(position);
                ObjectSpawner.Instance.SpawnAsync(gameObject, DamageText.Path, position, Quaternion.identity, UIManager.Instance.WorldSpaceCanvas.transform, "100");
            }
        }
    }
}
