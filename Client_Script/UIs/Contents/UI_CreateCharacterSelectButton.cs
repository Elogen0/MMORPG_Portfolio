using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreateCharacterSelectButton : UI_Base
{
    public Image selectFrame;
    [SerializeField] private TextMeshProUGUI classNameText;
    public string className;
    public int classTemplateId;
    public int statStrength;
    public int statRange;
    public int statHealth;
    
    
    public override void Init()
    {
        classNameText.text = className;
    }
}
