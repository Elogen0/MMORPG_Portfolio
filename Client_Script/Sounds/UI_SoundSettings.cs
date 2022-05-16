using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Kame.Define;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kame.Sounds
{
   public class UI_SoundSettings : MonoBehaviour
   {
      [SerializeField] private SoundSetting[] settings;

      [System.Serializable]
      public class SoundSetting
      {
         public SoundPlayType type;
         public Slider slider;
         public TMP_InputField valueInputField;
         public Button muteIcon;
         public Button unmuteIcon;
      }

      private void Start()
      {
         Init();
         AddListener();
      }

      private void Init()
      {
         //Init
         foreach (var setting in settings)
         {
            float volume = SoundManager.Instance.GetVolume(setting.type);
            setting.slider.normalizedValue = Mathf.Clamp(volume, setting.slider.minValue, setting.slider.maxValue);;;
            if (setting.valueInputField)
            {
               setting.valueInputField.text = ((int)(volume * 100)).ToString("D", CultureInfo.InvariantCulture);
            }
            if (setting.muteIcon == null || setting.unmuteIcon == null)
               continue;
            SetMuteIcon(setting, SoundManager.Instance.IsMute(setting.type));
         }
      }

      private void AddListener()
      {
         foreach (var setting in settings)
         {
            setting.slider.onValueChanged.AddListener((value) =>
            {
               SoundManager.Instance.SetVolume(setting.type, value);
               Debug.Log(value);

               if (setting.valueInputField)
               {
                  setting.valueInputField.text = ((int)(setting.slider.normalizedValue * 100)).ToString("D");
               }
               // if (setting.muteIcon.IsActive() && isSoundOn)
               // {
               //    SoundManager.Instance.SetMute(setting.type, false);
               // }
               //
               // if (setting.unmuteIcon.IsActive() && !isSoundOn)
               // {
               //    SoundManager.Instance.SetMute(setting.type, true);
               // }

               bool isSoundOn = setting.slider.value > setting.slider.minValue;
               if (Mathf.Approximately(setting.slider.minValue ,value) )
               {
                  SoundManager.Instance.SetMute(setting.type, !isSoundOn);
               }
               SetMuteIcon(setting, !isSoundOn);
            });

            if (setting.valueInputField)
            {
               setting.valueInputField.onEndEdit.AddListener(value =>
               {
                  float v = float.Parse(value);
                  if (v < 0 || v > 100)
                  {
                     v = Mathf.Clamp(v, 0, 100);
                     setting.valueInputField.text = v.ToString("F1");
                  }
                  v /= 100;
                  v = Mathf.Clamp01(v);
                  setting.slider.normalizedValue = v;
                  if (v == 0)
                  {
                     SoundManager.Instance.SetMute(setting.type, true);
                     SetMuteIcon(setting, true);
                  }
                  else
                  {
                     SoundManager.Instance.SetMute(setting.type, false);
                     SetMuteIcon(setting, false);
                  }
               });
            }
            

            if (setting.muteIcon == null || setting.unmuteIcon == null)
               continue;
            setting.muteIcon.onClick.AddListener(() =>
            {
               SoundManager.Instance.SetMute(setting.type, false);
               SetMuteIcon(setting, false);
            });

            setting.unmuteIcon.onClick.AddListener(() =>
            {
               SoundManager.Instance.SetMute(setting.type, true);
               SetMuteIcon(setting, true);
            });
         }
      }

      private void SetMuteIcon(SoundSetting setting, bool mute)
      {
         if (setting.muteIcon == null || setting.unmuteIcon == null)
            return;
         setting.muteIcon.gameObject.SetActive(mute);
         setting.unmuteIcon.gameObject.SetActive(!mute);
      }
   }
}
