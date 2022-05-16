using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Game.Data;
using UnityEngine;

[Serializable]
public class DialogueData : IManagedData
{
    public int ID => id;
    public int id;
    public string reference;
    public string objectPath;
}