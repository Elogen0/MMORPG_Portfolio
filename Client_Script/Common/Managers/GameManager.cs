using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Kame;
using Kame.Core.Job;
using UnityEngine;

#region Kame
public class GameManager : SingletonMono<GameManager>
{
    
    protected override void Awake()
    {
        base.Awake();
        Screen.SetResolution(1920, 1080, false);
        
    }
}
#endregion
