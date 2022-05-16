using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kame.Battles
{

    public class ManaUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] Image mpBar;

        Mana mana;

        // Start is called before the first frame update
        void Start()
        {
            mana = GameObject.FindWithTag("Player").GetComponent<Mana>();
        }

        // Update is called once per frame
        void Update()
        {
            text.text = String.Format("{0:0}/{1:0}", mana.MP, mana.MaxMP);
            mpBar.fillAmount = mana.MP / mana.MaxMP;
        }
    }

}