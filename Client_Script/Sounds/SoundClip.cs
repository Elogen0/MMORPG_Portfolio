using System;
using System.Collections;
using System.Collections.Generic;
using Kame;
using Kame.Define;
using UnityEditor.Experimental;
using UnityEngine;
//루프, 페이드 관련 속성, 오디오 클립 속성들.
/// <summary>
/// 사운드 관련 속성들이 저장되어있음
/// Load해야 AudioClip에 메모리에 올라간다.
/// </summary>
public class SoundClip
{
    public SoundPlayType playType = SoundPlayType.None;
    public int id = 0;
    public string clipName = string.Empty;
    public string clipPath = string.Empty;
    public float maxVolume = 1.0f; //100%
    public bool isLoop = false; //전체 자동반복
    public float[] loopLastTime = new float[0]; //구간반복 끝시간. 길이가 0이 아니라면 루프구간이 있음
    public float[] loopStartTime = new float[0]; //구간반복 시작시간
    public int currentLoopIndex = 0; // 반복구간 index

    private AudioClip clip = null;
    public float pitch = 1.0f;
    public float dopplerLevel = 1.0f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
    public float minDistance = 10000.0f; //사운드가 퍼져갈 최소거리
    public float maxDistance = 50000.0f; //사운드가 퍼져갈 최대거리
    public float spartialBlend = 1.0f; //blendTime

    public float elapsedFadeTime = 0.0f; 
    public float MaxFadeTime = 0.0f;
    public Interpolate.Function interpolate_Func;
    public bool isFadeIn = false;
    public bool isFadeOut = false;
    public bool isLoading = false;

    public SoundClip() { }
    public SoundClip(string clipPath, string clipName)
    {
        this.clipPath = clipPath;
        this.clipName = clipName;
    }

    /// <summary>
    /// ResourceManger를통해 Load하여 clip에 세팅
    /// </summary>
    public void Load()
    {
        if(this.clip == null)
        {
            string fullPath = this.clipPath;
            AddressableLoader.LoadAssetAsync<AudioClip>(fullPath, clip =>
            {
                this.clip = clip;
            });
        }
    }

    public void LoadAsync(Action<SoundClip> completed)
    {
        if (this.clip != null) return;
        if (isLoading) return;
        
        isLoading = true;
        AddressableLoader.LoadAssetAsync<AudioClip>(this.clipPath, clip =>
        {
            this.clip = clip;
            isLoading = false;
            completed?.Invoke(this);
        });
    }
    
    /// <summary>
    /// GetClip
    /// </summary>
    /// <returns>if not exist return null</returns>
    public AudioClip GetAudio()
    {
        if(this.clip == null)
        {
            Load();
        }
        if(this.clip == null && this.clipName != string.Empty)
        {
            Debug.LogWarning($"Can not load audio clip Resouce {this.clipName}");
            return null;
        }
        return this.clip;
    }

    /// <summary>
    /// clip을 null로 세팅하여 나중에 가비지콜렉터가 수거해가도록함
    /// </summary>
    public void ReleaseClip()
    {
        AddressableLoader.Release(this.clip);
        this.clip = null;
    }
    
    // --- 반복기능 ---
    /// <summary>
    /// 구간반복이 있는가? (CheckTime이 없다면 No Loop)
    /// </summary>
    /// <returns></returns>
    public bool HasLoop()
    {
        return this.loopLastTime != null && this.loopLastTime.Length > 0;
    }
    
    /// <summary>
    /// 다음번 Loop구간으로 이동, Max Loop를 넘으면 처음 Loop구간으로
    /// </summary>
    public void NextLoop()
    {
        this.currentLoopIndex++;
        if(this.currentLoopIndex >= this.loopLastTime.Length)
        {
            ResetLoop();
        }
    }

    public void ResetLoop()
    {
        this.currentLoopIndex = 0;
    }
    
    /// <summary>
    /// 반복구간을 체크하여 반복구간이 넘었으면 다시재생
    /// SoundMangaer의 LoopCheckProcess Coroutine으로 체크함
    /// todo: 이상하게 동작함
    /// 
    /// </summary>
    /// <param name="source">clip을 출력중인 AudioSource</param>
    public void CheckLoop(AudioSource source)
    {
        if(HasLoop() && source.time >= this.loopLastTime[this.currentLoopIndex]) 
        {
            source.time = this.loopStartTime[this.currentLoopIndex];
        }
    }
    
    /// <summary>
    /// Loop구간을 추가
    /// chekTime의 배열길이가 0이 아니라면 루프구간이 있음
    /// </summary>
    public void AddLoop()
    {
        this.loopLastTime = ArrayHelper.Add(0.0f, this.loopLastTime);
        this.loopStartTime = ArrayHelper.Add(0.0f, this.loopStartTime);
    }

    /// <summary>
    /// Loop구간을 제거
    /// </summary>
    /// <param name="index">Loop index</param>
    public void RemoveLoop(int index)
    {
        this.loopLastTime = ArrayHelper.Remove(index, this.loopLastTime);
        this.loopStartTime = ArrayHelper.Remove(index, this.loopStartTime);
    }

    //-- 반복기능 end --
    /// <summary>
    /// 해당클립을 fadeIn상태로 변경 (상태를 변경하면 DoFade가 처리)
    /// </summary>
    /// <param name="time">Fade되는 시간</param>
    /// <param name="easeType">보간용 곡선</param>
    public void SetFadeIn(float time, Interpolate.EaseType easeType)
    {
        this.isFadeOut = false;
        this.elapsedFadeTime = 0.0f;
        this.MaxFadeTime = time;
        this.interpolate_Func = Interpolate.Ease(easeType);
        this.isFadeIn = true;
    }

    /// <summary>
    /// 해당 클립을 FadeOut상태로 변경 (상태를 변경하면 DoFade가 처리)
    /// </summary>
    /// <param name="time">Fade되는 시간</param>
    /// <param name="easeType">보간용 곡선</param>
    public void SetFadeOut(float time, Interpolate.EaseType easeType)
    {
        this.isFadeIn = false;
        this.elapsedFadeTime = 0.0f;
        this.MaxFadeTime = time;
        this.interpolate_Func = Interpolate.Ease(easeType);
        this.isFadeOut = true;
    }

    /// <summary>
    /// 페이드인,아웃 효과 프로세스. (SoundManager의 Update함수에서 Fade처리)
    /// </summary>
    /// <param name="deltaTime"> deltaTime </param>
    /// <param name="audio"> 출력 AudioSource </param>
    public void DoFade(float deltaTime, AudioSource audio)
    { 
        if(this.isFadeIn == true)
        {
            this.elapsedFadeTime += deltaTime;
            audio.volume = Interpolate.Ease(this.interpolate_Func, 0, maxVolume,
                elapsedFadeTime, MaxFadeTime);
            if(this.elapsedFadeTime >= this.MaxFadeTime)
            {
                this.isFadeIn = false;
            }
        }
        else if(this.isFadeOut == true)
        {
            this.elapsedFadeTime += deltaTime;
            audio.volume = Interpolate.Ease(this.interpolate_Func, maxVolume, 0 - maxVolume,
                elapsedFadeTime, MaxFadeTime);
            if(this.elapsedFadeTime >= this.MaxFadeTime)
            {
                this.isFadeOut = false;
                audio.Stop();
            }
        }
    }
}
