using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Abilities;
using Kame.Sounds;
using UnityEngine;

public class SoundCue : MonoBehaviour
{
    enum SequenceMode
    {
        Random,
        RandomNoImmediateRepeat,
        Sequential,
    }

    [SerializeField] private bool playOnEnable;
    [SerializeField] private SequenceMode sequenceMode = SequenceMode.RandomNoImmediateRepeat;
    [SerializeField] private SoundList[] soundList;
    
    private int _nextClipToPlay = -1;
    private int _lastClipPlayed = -1;

    public void PlaySound()
    {
        SoundManager.Instance.PlayAsync(GetNextClip());
    }
    
    private SoundList GetNextClip()
    {
        // Fast out if there is only one clip to play
        if (soundList.Length == 1)
            return soundList[0];

        if (_nextClipToPlay == -1)
        {
            // Index needs to be initialised: 0 if Sequential, random if otherwise
            _nextClipToPlay = (sequenceMode == SequenceMode.Sequential) ? 0 : UnityEngine.Random.Range(0, soundList.Length);
        }
        else
        {
            // Select next clip index based on the appropriate SequenceMode
            switch (sequenceMode)
            {
                case SequenceMode.Random:
                    _nextClipToPlay = UnityEngine.Random.Range(0, soundList.Length);
                    break;

                case SequenceMode.RandomNoImmediateRepeat:
                    do
                    {
                        _nextClipToPlay = UnityEngine.Random.Range(0, soundList.Length);
                    } while (_nextClipToPlay == _lastClipPlayed);
                    break;

                case SequenceMode.Sequential:
                    _nextClipToPlay = (int)Mathf.Repeat(++_nextClipToPlay, soundList.Length);
                    break;
            }
        }

        _lastClipPlayed = _nextClipToPlay;

        return soundList[_nextClipToPlay];
    }

    private void OnEnable()
    {
        if (playOnEnable)
            PlaySound();
    }
}
