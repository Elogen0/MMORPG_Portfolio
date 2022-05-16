using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNode : MonoBehaviour
{
    public StageNode prevNode;
    public StageNode nextNode;


    private void Start()
    {
        
    }

    public void ShowNodeInfo()
    {
        Debug.Log("GetNodeInfo");
    }

    private void Update()
    {
    }
}
