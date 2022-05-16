using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Kame.AI;
using UnityEngine;
using UnityEngine.Playables;

public class StageManager : MonoBehaviour
{
    [SerializeField] StageSector[] stageSectors;
    [SerializeField] GameObject boss;
    [SerializeField] private GameObject flockLeader;
    [SerializeField] private float recommendClearTime = 90f;
    [SerializeField] private float gameOverTime = 180f;

    private PlayableDirector director;
    public bool IsStageClear { get; set; } = false;
    public bool IsStageCanceled { get; set; } = false;

    private int currentSectorIndex = 0;

    public float ElapsedTime { get; set; }= 0;
    private IEnumerator Start()
    {
        foreach (var sector in stageSectors)
        {
            sector.OnDeadEvent += OnEnemyDead;
            sector.OnClearSectorEvent += OnClearSector;
        }
        yield return StageStart();
        yield return StagePlaying();
        yield return StageFinished();
    }

    IEnumerator StageStart()
    {
        //Display Start Animation
        stageSectors[currentSectorIndex].Spawn();
        yield return null;
    }

    IEnumerator StagePlaying()
    {
        while (!IsStageClear && !IsStageCanceled && ElapsedTime < gameOverTime)
        {
            ElapsedTime += Time.deltaTime;
            
            flockLeader.transform.position = Vector3.MoveTowards(flockLeader.transform.position,
                stageSectors[currentSectorIndex].transform.position, 1f);
            yield return null;
        }
    }

    IEnumerator StageFinished()
    {
        if (IsStageClear)
        {
            Debug.Log("Stage All Clear");
            //Display Clear
        }
        else if (IsStageCanceled)
        {
            Debug.Log("Stage Failed");
            //Display failed
        }
        else
        {
            //Stage Failed
        }
        yield return null;
        //Exit from Current Stage
    }

    private void OnEnemyDead(Health enemyHealth)
    {
        if (boss && enemyHealth.gameObject == boss)
        {
            IsStageClear = true;
        }
    }

    private void OnClearSector()
    {
        ++currentSectorIndex;
        if (currentSectorIndex >= stageSectors.Length)
        {
            currentSectorIndex = stageSectors.Length - 1;
        }
        else
        {
            stageSectors[currentSectorIndex].Spawn();
        }
        
        bool isStageAllClear = true;
        foreach (var sector in stageSectors)
        {
            if (!sector.IsCleared)
                isStageAllClear = false;
        }
        IsStageClear = isStageAllClear;
    }

    public void StartPlay()
    {
        
    }
}
