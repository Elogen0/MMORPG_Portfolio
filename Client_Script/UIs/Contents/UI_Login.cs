using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Login : UI_Base
{
    [SerializeField] private UI_SelectServerPopup serverPopup;
    enum GameObjects
    {
        input_id,
        input_pw,
    }

    enum Buttons
    {
        btn_login,
        btn_sign_up
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.btn_login).onClick.AddListener(OnClickLoginButton);
        GetButton((int)Buttons.btn_sign_up).onClick.AddListener(OnClickCreateButton);
    }

    public void OnClickCreateButton()
    {
        TMP_InputField accountInput = Get<GameObject>((int) GameObjects.input_id).GetComponent<TMP_InputField>();
        TMP_InputField passwordInput = Get<GameObject>((int) GameObjects.input_pw).GetComponent<TMP_InputField>();

        string account = accountInput.text;
        string password = passwordInput.text;
        
        CreateAccountPacketReq packet = new CreateAccountPacketReq()
        {
            AccountName = account,
            Password = password
        };
        
        WebManager.Instance.SendPostRequest<CreateAccountPacketRes>("account/create", packet, 
            (res) => 
            {
                Debug.Log(res.CreateOk);

                accountInput.text = "";
                passwordInput.text = "";
            });
        
    }
    
    public void OnClickLoginButton()
    {
        TMP_InputField accountInput = Get<GameObject>((int) GameObjects.input_id).GetComponent<TMP_InputField>();
        TMP_InputField passwordInput = Get<GameObject>((int) GameObjects.input_pw).GetComponent<TMP_InputField>();
        string account = accountInput.text;
        string password = passwordInput.text;
        
        Debug.Log("OnClickLoginButton");
        LoginAccountPacketReq packet = new LoginAccountPacketReq()
        {
            AccountName = account,
            Password = password
        };

        WebManager.Instance.SendPostRequest<LoginAccountPacketRes>("account/login", packet,
            (res) =>
            {
                accountInput.text = "";
                passwordInput.text = "";
                Debug.Log($"Login Result : {res.LoginOk}");
                if (res.LoginOk)
                {
                    NetworkManager.Instance.AccountId = res.AccountId;
                    NetworkManager.Instance.Token = res.Token;
                    
                    serverPopup.gameObject.SetActive(true);
                    serverPopup.SetParent(this);
                    serverPopup.SetServers(res.ServerList);
                    
                    gameObject.SetActive(false);
                }
            });
    }
}
