using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_CreateCharacter : UI_Base
{
    public Button[] selectButtons;
    private int selectCharacterTemplateId = 1;

    enum Texts
    {
        tmp_class_name,
        tmp_stats
    }

    enum GameObjects
    {
        btn_confirm,
        input_name
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        for (int i = 0; i < selectButtons.Length; i++)
        {
            UI_CreateCharacterSelectButton otherInfo = selectButtons[i].GetComponent<UI_CreateCharacterSelectButton>();
            if (i == 0)
            {
                otherInfo.selectFrame.gameObject.SetActive(true);
            }
            else
            {
                otherInfo.selectFrame.gameObject.SetActive(false);
            }
        }
        foreach (var button in selectButtons)
        {
            
        }
        
        BindSelectButtonListener();
        GetObject((int) GameObjects.btn_confirm).GetComponent<Button>().onClick.AddListener(Confirm);
    }

    private void BindSelectButtonListener()
    {
        foreach (var button in selectButtons)
        {
            button.onClick.AddListener(() =>
            {
                foreach (var button in selectButtons)
                {
                    UI_CreateCharacterSelectButton otherInfo = button.GetComponent<UI_CreateCharacterSelectButton>();
                    otherInfo.selectFrame.gameObject.SetActive(false);
                }
                
                UI_CreateCharacterSelectButton info = button.GetComponent<UI_CreateCharacterSelectButton>();
                info.selectFrame.gameObject.SetActive(true);
                selectCharacterTemplateId = info.classTemplateId;
                GetText((int) Texts.tmp_class_name).text = info.className;
                GetText((int) Texts.tmp_stats).text = 
                    $"Strength : {info.statStrength}/10\n Range : {info.statRange}/10\n Health : {info.statHealth}/10\n";
            });
        }
    }

    private void Confirm()
    {
        string confirmName = GetObject((int) GameObjects.input_name).GetComponent<TMP_InputField>().text;
        if (!string.IsNullOrEmpty(confirmName) && selectCharacterTemplateId != -1)
        {
            C_CreatePlayer createPacket = new C_CreatePlayer();
            createPacket.Name = confirmName;
            createPacket.CharacterId = selectCharacterTemplateId;
            NetworkManager.Instance.Send(createPacket);
        }
    }

    public void CreateCharacterFailed()
    {
        
    }

    public void CreateCharacterSuccess()
    {
        
    }
}
