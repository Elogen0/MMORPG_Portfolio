using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : BaseController
{
    protected override void Awake()
    {
        base.Awake();
        _health.showHealthBar = false;
    }

    protected override void Update()
    {
    }
}
