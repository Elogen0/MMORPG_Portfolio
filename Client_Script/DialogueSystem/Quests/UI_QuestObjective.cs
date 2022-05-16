using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_QuestObjective : UI_Base
{
    enum Texts
    {
        tmp_title,
        tmp_progress,
    }

    // enum GameObjects
    // {
    //     Complete,
    // }
    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        // Bind<GameObject>(typeof(GameObjects));
    }
    
    public void SetComplete(bool complete)
    {
        GetText((int) Texts.tmp_title).color = complete ? Color.black : Color.gray;
        GetText((int) Texts.tmp_progress).color = complete ? Color.black : Color.gray;
        
        //GetObject((int)GameObjects.Complete).SetActive(complete);
    }

    public void SetDescription(string description)
    {
        SetText((int)Texts.tmp_title, description);
    }

    public void SetProgress(int progress, int completeProgress)
    {
        SetText((int)Texts.tmp_progress, $"{progress}/{completeProgress}");
    }
}
