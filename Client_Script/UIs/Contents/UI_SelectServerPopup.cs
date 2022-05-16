using System.Collections;
using System.Collections.Generic;
using Kame;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectServerPopup : MonoBehaviour
{
    public List<UI_SelectServerPopup_Item> Items { get; } = new List<UI_SelectServerPopup_Item>();

    private UI_Login login;

    public void SetServers(List<ServerInfo> servers)
    {
        StartCoroutine(CoSetServers(servers));
    }
    
    public IEnumerator CoSetServers(List<ServerInfo> servers)
    {
        foreach (var item in Items)
        {
            if (item)
                Destroy(item.gameObject);
        }

        if (Items.Count > 0)
            Items.Clear();
        
        Transform verticalGroup = GetComponentInChildren<VerticalLayoutGroup>().transform;
        verticalGroup.RemoveAllChildren();
        for (int i = 0; i < servers.Count; i++)
        {
            var request = AddressableLoader.Instantiate("Assets/Game/Prefab/UI/Contents/btn_server_select_item.prefab", verticalGroup);
            yield return request.Wait();
            GameObject go = request.Result;
            UI_SelectServerPopup_Item item = go.GetComponent<UI_SelectServerPopup_Item>();
            item.Info = servers[i];
            Items.Add(item);
        }
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (Items.Count == 0)
            return;

        foreach (var item in Items)
        {
            item.RefreshUI();
        }
    }

    public void SetParent(UI_Login uiLogin)
    {
        login = uiLogin;
    }
}
