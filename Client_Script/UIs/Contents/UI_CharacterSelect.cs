using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Kame;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UI_CharacterSelect : UI_Base
{
    private List<PlayerCharacterInfo> infoList = new List<PlayerCharacterInfo>();
    private PlayerCharacterInfo selectCharacter;
    enum GameObjects
    {
        item_container
    }

    enum Buttons
    {
        btn_create,
        btn_remove,
        btn_change_server,
        btn_quit_game,
        btn_game_start
    }
    
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        //Get<Button>((int) Buttons.btn_game_start).onClick.AddListener(EnterGame);
        Get<Button>((int) Buttons.btn_create).onClick.AddListener(OnClickCreateButton);
    }

    public void LoadCharacter(IEnumerable<PlayerCharacterInfo> characterInfos)
    {
        infoList.Clear();
        foreach (var info in characterInfos)
        {
            infoList.Add(info);
        }
        StartCoroutine(CoRefresh(characterInfos));
    }

    IEnumerator CoRefresh(IEnumerable<PlayerCharacterInfo> characterInfos)
    {
        GameObject container = GetObject((int) GameObjects.item_container);
        container.transform.RemoveAllChildren();

        foreach (var info in characterInfos)
        {
            var request = AddressableLoader.Instantiate("Assets/Game/Prefab/UI/Contents/character_select_item.prefab", container.transform);
            yield return request.Wait();
            GameObject go = request.Result; 
            UI_CharacterSelectItem item = go.GetComponent<UI_CharacterSelectItem>();
            item.levelText.text = info.StatInfo.Level.ToString();
            item.nameText.text = info.Name;
            item.info = info;
        }
    }

    private void OnClickCreateButton()
    {
        UI_View view = UI_ViewNavigation.Instance.Show("CharCreate");
    }
}
