using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_SelectServerPopup_Item : UI_Base
{
    public ServerInfo Info { get; set; }

    enum Texts
    {
        tmp_btn_text
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        GetComponent<Button>().onClick.AddListener(OnClickButton);
    }

    public void RefreshUI()
    {
        if (Info == null)
            return;
        TextMeshProUGUI t = GetText((int) Texts.tmp_btn_text);
        t.text = Info.Name;
    }
    
    void OnClickButton()
    {
        NetworkManager.Instance.ConnectToGame(Info);
    }
}
