using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Sounds;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreatureSound : MonoBehaviour
{
    [SerializeField] private SoundList[] footstepSound;
    [SerializeField] private SoundList[] dashSounds;
    [SerializeField] private SoundList[] hitVoices;
    
    public void PlayFootStepSound()
    {
        if (footstepSound.Length == 0)
            return;
        int index = Random.Range(0, footstepSound.Length);
        SoundManager.Instance.PlayAsync(footstepSound[index], transform.position);
    }
    
    public void PlayDashSound()
    {
        if (dashSounds.Length == 0)
            return;
        int index = Random.Range(0, dashSounds.Length);
        SoundManager.Instance.PlayAsync(dashSounds[index], transform.position + transform.forward *1.3f);
    }

    public void PlayHitVoice()
    {
        if (hitVoices.Length == 0)
            return;
        int index = Random.Range(0, hitVoices.Length);
        SoundManager.Instance.PlayAsync(hitVoices[index], transform.position);
    }
}
