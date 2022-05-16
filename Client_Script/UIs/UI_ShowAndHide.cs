using System;
using System.IO;
using UnityEngine;

namespace Kame
{
    public class UI_ShowAndHide : MonoBehaviour
    {
        [SerializeField] private bool keyOff = false;
        [SerializeField] private KeyCode toggleKey = KeyCode.Escape;
        [SerializeField] private GameObject uiContainer;

        // Start is called before the first frame update
        void Start()
        {
            uiContainer.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (!keyOff && Input.GetKeyDown(toggleKey))
            {
                Act();
            }
        }

        public void Act()
        {
            uiContainer.SetActive(!uiContainer.activeSelf);
        }
    }
}