using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Define;
using UnityEngine;

namespace Kame.Sounds
{
    
    [System.Serializable]
    public enum MusicPlayType
    {
        None = -1,
        OnStart,
        Stop,
        Triggered,
    }

    public class BgmPlayer : MonoBehaviour
    {
        [SerializeField] private SoundList sound;
        [SerializeField] private MusicPlayType playType = MusicPlayType.None;
        [SerializeField] private float fadeTime = 0.1f;
        [SerializeField] private Interpolate.EaseType easeType = Interpolate.EaseType.Linear;
        [SerializeField] private bool preload = false;
        private SoundClip _loadedClip;

        private void Start()
        {
            if (preload)
            {
                SoundManager.Instance.LoadAsync(sound, (clip) =>
                {
                    _loadedClip = clip;
                });
            }
            
            if (playType == MusicPlayType.OnStart)
            {
                PlayMusic();
            }
        }

        public void PlayMusic()
        {
            if (playType == MusicPlayType.Stop || sound == SoundList.None)
            {
                SoundManager.Instance.Stop();
                return;
            }

            if (_loadedClip != null)
            {
                SoundManager.Instance.PlayBGM(_loadedClip, fadeTime, easeType);
            }
            else
            {
                SoundManager.Instance.PlayBGMAsync(sound, fadeTime, easeType);
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(TagAndLayer.TagName.Player))
                return;
            if (playType == MusicPlayType.Triggered)
            {
                PlayMusic();
            }
        }
    }
}
