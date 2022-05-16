using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_CharacterSelectItem : UI_Base
{
    public Image portraitImage;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nameText;
    [NonSerialized] public PlayerCharacterInfo info;
    public override void Init()
    {
        GetComponent<Button>().onClick.AddListener(RequestEnter);
    }
    
    private void RequestEnter()
    {
        C_EnterGame enterGame = new C_EnterGame();
        enterGame.Name = info.Name;
        enterGame.SceneName = "town";
        
        SceneLoader.Instance.LoadScene("town", () =>
        {
            ObjectManager.Instance.MyPlayerInfo = info;
            Portal portal = Portal.GetPortal(0);
            Vector3 curPos = portal.spawnPoint.position;
            Vector3 dirVec = portal.spawnPoint.TransformDirection(Vector3.forward);
            enterGame.PosInfo = new PositionInfo()
            {
                PosX = curPos.x,
                PosY = curPos.z,
                DirX = dirVec.x,
                DirY = dirVec.z,
                State = CreatureState.Idle
            };
            NetworkManager.Instance.Send(enterGame);    
        });
        
    }
}
