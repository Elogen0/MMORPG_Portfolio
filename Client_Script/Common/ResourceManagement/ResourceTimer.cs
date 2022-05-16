using System;
using System.Collections;
using System.Collections.Generic;
using Kame;
using Kame.Core.Job;
using Unity.Collections;
using UnityEngine;

public class ResourceTimer : SingletonMono<ResourceTimer>
{
    private JobTimer _timer = new JobTimer();
    public IJob PushAfter(int tickAfter, Action action) { return PushAfter(tickAfter, new Job(action)); }
    public IJob PushAfter<T1>(int tickAfter, Action<T1> action, T1 t1) { return PushAfter(tickAfter, new Job<T1>(action, t1)); }
    public IJob PushAfter<T1, T2>(int tickAfter, Action<T1, T2> action, T1 t1, T2 t2) { return PushAfter(tickAfter, new Job<T1, T2>(action, t1, t2)); }
    public IJob PushAfter<T1, T2, T3>(int tickAfter, Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { return PushAfter(tickAfter, new Job<T1, T2, T3>(action, t1, t2, t3)); }

    public IJob PushAfter(int tickAfter, IJob job)
    {
        _timer.Push(job, tickAfter);
        return job;
    }

    public void ExecuteAll()
    {
        _timer.ForceExecuteAll();
    }
    
    private void Update()
    {
        if (shuttingDown)
            return;
        _timer.Flush();
    }
}
