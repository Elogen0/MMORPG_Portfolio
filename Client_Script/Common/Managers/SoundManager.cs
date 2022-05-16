using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Define;
using Kame.Game.Data;
using UnityEngine;
using UnityEngine.Audio;

namespace Kame.Sounds
{
    public class SoundManager : SingletonMono<SoundManager>
    {
        public enum MusicPlayingStatus
        {
            None = 0,
            SourceA = 1,
            SourceB = 2,
            AtoB = 3,
            BtoA = 4
        }

        private const string MasterGroupName = "Master";
        private const string EffectGroupName = "Effect";
        private const string BGMGroupName = "BGM";
        private const string UIGroupName = "UI";
        private const string MixerName = "SoundManagerMixer";
        private const string ContainerName = "SoundContainer";
        private const string BGM_Channel_A = "BGM_Channel_A";
        private const string BGM_Channel_B = "BGM_Channel_B";
        private const string UI = "UI";
        private const string MasterVolumeParam = "Volume_Master";
        private const string EffectVolumeParam = "Volume_Effect";
        private const string BGMVolumeParam = "Volume_BGM";
        private const string UIVolumeParam = "Volume_UI";

        //fade에 필요한 인자


        private AudioMixer mixer = null;
        private Transform audioRoot = null;
        private AudioSource BGM_A_audio = null;
        private AudioSource BGM_B_audio = null;
        private AudioSource[] effect_audios = null; //이펙트 오디오 채널 제한 5개이상 동시재생하면 찢어진다고함
        private AudioSource UI_audio = null;

        private float[] effect_PlayStartTime = null; //이펙트 채널이 꽉차면 오래된거 끄기위해 저장.
        private int EffectChannelCount = 5; //이펙트 채널 개수
        private MusicPlayingStatus _currentPlayingStatus = MusicPlayingStatus.None;
        private bool isTicking = false;
        private SoundClip _currentMusicClip = null;
        private SoundClip _lastMusicClip = null;
        private float _minVolume = -80f;
        private float _maxVolume = 0f;

        private Dictionary<int, SoundClip> _soundClips = new Dictionary<int, SoundClip>();

        public SoundClip Load(SoundList sound)
        {
            return GetSoundClip(sound);
        }

        public void LoadAsync(SoundList sound, Action<SoundClip> completed)
        {
            if (_soundClips.ContainsKey((int) sound))
            {
                completed?.Invoke(_soundClips[(int) sound]);
                return;
            }
            else
            {
                DataManager.Sound().GetClipCopyAsync((int) sound, (clip) =>
                {
                    if (_soundClips.ContainsKey((int) sound))
                        return;
                    if (clip.playType != SoundPlayType.BGM)
                    {
                        _soundClips.Add((int) sound, clip);
                    }

                    completed?.Invoke(clip);
                });
            }
        }


        public AudioClip GetAudioClip(SoundList soundList)
        {
            return GetSoundClip(soundList).GetAudio();
        }

        public SoundClip GetSoundClip(SoundList sound)
        {
            if (_soundClips.ContainsKey((int) sound))
            {
                return _soundClips[(int) sound];
            }
            else
            {
                SoundClip clip = DataManager.Sound().GetClipCopy((int) sound);
                if (clip.playType != SoundPlayType.BGM)
                {
                    _soundClips.Add((int) sound, clip);
                }

                return clip;
            }
        }

        public void Clear(bool clearCache = true)
        {
            Stop(true);
             BGM_A_audio.clip = null;
             BGM_B_audio.clip = null;
             UI_audio.clip = null;
             foreach (var source in effect_audios)
             {
                 source.clip = null;
             }
             
            
            if (clearCache)
            {
                foreach (var clip in _soundClips.Values)
                {
                    clip.ReleaseClip();
                }
                _soundClips.Clear();    
            }
        }

        #region Volume Settings
        public float GetVolume(SoundPlayType type)
        {
            string parameterName = GetParameterName(type);
            if (PlayerPrefs.HasKey(parameterName))
            {
                return PlayerPrefs.GetFloat(parameterName); //todo: 저장할때는 값을 저장하는데 불로올때 lerp를 이용하여 load?
            }
            else
            {
                SetVolume(type, 1);
                return 1;
            }
        }

        public void SetVolume(SoundPlayType type, float currentRatio)
        {
            string parameterName = GetParameterName(type);
            currentRatio = Mathf.Clamp01(currentRatio); //0과1사이 값 고정
            PlayerPrefs.SetFloat(parameterName, currentRatio); //값저장
            
            //float volume = Mathf.Lerp(_minVolume, _maxVolume, currentRatio); //슬라이더바로 조절하기때문에 비율을 쓴다.
            float volume = Mathf.Log10(currentRatio) * 20f;
            this.mixer.SetFloat(parameterName, volume); //믹서에 세팅
        }

        public void SetMute(SoundPlayType type, bool mute)
        {
            string parameterName = GetParameterName(type);
            if (mute)
            {
                mixer.SetFloat(parameterName, _minVolume);
            }
            else
            {
                var volume = Mathf.Lerp(_minVolume, _maxVolume, GetVolume(type));
                mixer.SetFloat(parameterName, volume);
            }

            PlayerPrefs.SetString(parameterName + "Mute", mute.ToString());
        }

        public bool IsMute(SoundPlayType type)
        {
            string parameterName = GetParameterName(type) + "Mute";
            if (!PlayerPrefs.HasKey(parameterName))
                return false;
            return Boolean.Parse(PlayerPrefs.GetString(parameterName));
        }
        
        /// <summary>
        /// 저장된 볼륨으로 초기화
        /// </summary>
        void VolumeInit()
        {
            if (this.mixer != null)
            {
                this.mixer.SetFloat(MasterVolumeParam,
                    Mathf.Lerp(_minVolume, _maxVolume, GetVolume(SoundPlayType.None)));
                this.mixer.SetFloat(BGMVolumeParam, Mathf.Lerp(_minVolume, _maxVolume, GetVolume(SoundPlayType.BGM)));
                this.mixer.SetFloat(EffectVolumeParam,
                    Mathf.Lerp(_minVolume, _maxVolume, GetVolume(SoundPlayType.Effect)));
                this.mixer.SetFloat(UIVolumeParam, Mathf.Lerp(_minVolume, _maxVolume, GetVolume(SoundPlayType.UI)));
            }
        }

        private string GetParameterName(SoundPlayType type)
        {
            string parameterName = "";
            switch (type)
            {
                case SoundPlayType.BGM:
                    parameterName = BGMVolumeParam;
                    break;
                case SoundPlayType.Effect:
                    parameterName = EffectVolumeParam;
                    break;
                case SoundPlayType.UI:
                    parameterName = UIVolumeParam;
                    break;
                default:
                    parameterName = MasterVolumeParam;
                    break;
            }

            return parameterName;
        }

        #endregion
        
        #region PlaySound

        public void Play(SoundList sound)
        {
            Play(GetSoundClip(sound), Vector3.zero, false);
        }

        public void Play(SoundList sound, Vector3 position)
        {
            Play(GetSoundClip(sound), position, true);
        }

        public void PlayAsync(SoundList sound)
        {
            LoadAsync(sound, clip => { Play(clip, Vector3.zero, false); });
        }

        public void PlayAsync(SoundList sound, Vector3 position)
        {
            LoadAsync(sound, clip => { Play(clip, position, true); });
        }

        private void Play(SoundClip clip, Vector3 position, bool playAtPoint)
        {
            if (clip == null)
            {
                return;
            }

            switch (clip.playType)
            {
                case SoundPlayType.Effect:
                    PlayEffectSound(clip, position, playAtPoint);
                    break;
                case SoundPlayType.BGM:
                    PlayBGM(clip);
                    break;
                case SoundPlayType.UI:
                    PlayUISound(clip);
                    break;
            }
        }

        public void Stop(bool allStop = false)
        {
            if (allStop)
            {
                this.BGM_A_audio.Stop();
                this.BGM_B_audio.Stop();
                foreach (var source in effect_audios)
                {
                    source.Stop();
                }
                UI_audio.Stop();
            }

            this.FadeOut(0.5f, Interpolate.EaseType.Linear);
            this._currentPlayingStatus = MusicPlayingStatus.None;
            StopAllCoroutines();
        }
        
        //todo: stopBGM
        //다시 Start 할 시 구간반복이 적용된 상태이므로 이부분 수정이 필요
        public void StopBGM()
        {
            this.BGM_A_audio.Stop();
            this.BGM_B_audio.Stop();
        }

        

        
        public void PlayBGM(SoundClip clip, float time = 0.1f, Interpolate.EaseType type = Interpolate.EaseType.Linear)
        {
            if (this.IsDifferentSound(clip))
            {
                FadeTo(clip, time, type);
            }
        }

        public void PlayBGMAsync(SoundList sound, float time = 0.1f,
            Interpolate.EaseType type = Interpolate.EaseType.Linear)
        {
            LoadAsync(sound, clip => { PlayBGM(clip, time, type); });
        }

        /// <summary>
        /// Fade없이 BGM 바로재생
        /// </summary>
        /// <param name="clip">New SoundClip</param>
        public void PlayBGMInstant(SoundClip clip)
        {
            if (this.IsDifferentSound(clip))
            {
                this.BGM_B_audio.Stop();
                this._lastMusicClip = this._currentMusicClip;
                this._currentMusicClip = clip;
                PlayAudioSource(BGM_A_audio, clip, clip.maxVolume);
                this._currentPlayingStatus = MusicPlayingStatus.SourceA; //
                if (_currentMusicClip.HasLoop())
                {
                    this.isTicking = true;
                    DoLoopCheck();
                }
            }
        }

        
        
        /// <summary>
        /// 사운드를 플레이를 시키는 ****핵심함수****
        /// </summary>
        /// <param name="source">재생시킬 audio채널</param>
        /// <param name="clip">재생시킬 SoundClip</param>
        /// <param name="volume">Volume</param>
        private void PlayAudioSource(AudioSource source, SoundClip clip, float volume)
        {
            if (source == null || clip == null)
            {
                return;
            }

            source.Stop(); //일단 끄고
            source.clip = clip.GetAudio(); //클립을 가져오고
            //각 값을 복사해옴
            source.volume = volume;
            source.loop = clip.isLoop;
            source.pitch = clip.pitch;
            source.dopplerLevel = clip.dopplerLevel;
            source.rolloffMode = clip.rolloffMode;
            source.minDistance = clip.minDistance;
            source.maxDistance = clip.maxDistance;
            //source.spatialBlend = clip.spartialBlend;
            //플레이
            source.Play();
        }

        private void PlayUISound(SoundClip clip)
        {
            PlayAudioSource(UI_audio, clip, clip.maxVolume);
        }

        private void PlayEffectSound(SoundClip clip, Vector3 position, bool playAtPoint = true)
        {
            AudioSource source = GetEffectAudioSource(clip, out int index);
            source.Stop();
            if (playAtPoint)
                source.transform.position = position;
            else
                source.transform.localPosition = Vector3.zero;
            PlayAudioSource(source, clip, clip.maxVolume);
            this.effect_PlayStartTime[index] = Time.realtimeSinceStartup;
        }

        private AudioSource GetEffectAudioSource(SoundClip clip, out int index)
        {
            for (int i = 0; i < this.EffectChannelCount; i++)
            {
                if (this.effect_audios[i].isPlaying == false || this.effect_audios[i].clip == clip.GetAudio())
                {
                    index = i;
                    return this.effect_audios[i];
                }
            }

            //재생된지 가장 오래된 채널에 재생
            float maxTime = 0.0f;
            int selectIndex = 0;
            for (int i = 0; i < EffectChannelCount; i++)
            {
                if (this.effect_PlayStartTime[i] > maxTime)
                {
                    maxTime = this.effect_PlayStartTime[i];
                    selectIndex = i;
                }
            }

            index = selectIndex;
            return effect_audios[selectIndex];
        }


        /// <summary>
        /// BGM이 플레이인지 확인 PlayingType이 None이 아니면 재생중
        /// </summary>
        /// <returns></returns>
        private bool IsPlaying()
        {
            return (int) this._currentPlayingStatus > 0;
        }

        /// <summary>
        /// 재생하는 사운드가 다른사운드인지
        /// </summary>
        /// <param name="clip">확인할 SoundClip</param>
        /// <returns>false :같은사운드, true : 다른사운드</returns>
        private bool IsDifferentSound(SoundClip clip)
        {
            if (clip == null)
            {
                return false;
            }

            if (_currentMusicClip != null && _currentMusicClip.id == clip.id &&
                IsPlaying() && _currentMusicClip.isFadeOut == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region LoopCheck

        /// <summary>
        /// 반복구간이 있으면 체크해서 반복하게 하는 코루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoopCheckProcess()
        {
            while (this.isTicking == true && IsPlaying() == true)
            {
                yield return new WaitForSeconds(0.05f);
                if (this._currentMusicClip.HasLoop())
                {
                    if (_currentPlayingStatus == MusicPlayingStatus.SourceA)
                    {
                        _currentMusicClip.CheckLoop(BGM_A_audio);
                    }
                    else if (_currentPlayingStatus == MusicPlayingStatus.SourceB)
                    {
                        _currentMusicClip.CheckLoop(BGM_B_audio);
                    }
                    else if (_currentPlayingStatus == MusicPlayingStatus.AtoB)
                    {
                        this._lastMusicClip.CheckLoop(this.BGM_A_audio);
                        this._currentMusicClip.CheckLoop(this.BGM_B_audio);
                    }
                    else if (_currentPlayingStatus == MusicPlayingStatus.BtoA)
                    {
                        this._lastMusicClip.CheckLoop(this.BGM_B_audio);
                        this._currentMusicClip.CheckLoop(this.BGM_A_audio);
                    }
                }
            }
        }

        private void DoLoopCheck()
        {
            StartCoroutine(LoopCheckProcess());
        }

        #endregion

        #region Fade

        /// <summary>
        /// 재생중인 BGM을 모두 멈추고 새 BGM을 BGM_A채널로 FadeIn(실제실행은 DoFade)
        /// </summary>
        /// <param name="clip">재생할 SoundClip</param>
        /// <param name="time">Fade Time</param>
        /// <param name="ease">Interpolate.EaseType</param>
        private void FadeIn(SoundClip clip, float time, Interpolate.EaseType ease)
        {
            if (this.IsDifferentSound(clip))
            {
                this.BGM_A_audio.Stop();
                this.BGM_B_audio.Stop();
                this._lastMusicClip = this._currentMusicClip;
                this._currentMusicClip = clip;
                PlayAudioSource(BGM_A_audio, _currentMusicClip, 0.0f);
                this._currentMusicClip.SetFadeIn(time, ease);
                this._currentPlayingStatus = MusicPlayingStatus.SourceA;
                if (this._currentMusicClip.HasLoop() == true)
                {
                    this.isTicking = true; //시간 틱틱틱
                    DoLoopCheck();
                }
            }
        }

        /// <summary>
        /// SoudList Enum으로 정의된 Index로 불러와 FadeIn 
        /// </summary>
        /// <param name="index">SoudList Enum으로 정의된 Index</param>
        /// <param name="time">Fade time</param>
        /// <param name="ease">Interpolate.EaseType </param>
        private void FadeIn(int index, float time, Interpolate.EaseType ease)
        {
            this.FadeIn(DataManager.Sound().GetClipCopy(index), time, ease);
        }


        /// <summary>
        /// 현재재생되고 있는 Sound FadeOut (실제실행은 DoFade)
        /// </summary>
        /// <param name="time">Fade time</param>
        /// <param name="ease"></param>
        private void FadeOut(float time, Interpolate.EaseType ease)
        {
            if (this._currentMusicClip != null)
            {
                this._currentMusicClip.SetFadeOut(time, ease);
            }
        }

        /// <summary>
        /// 이전Audio FadeOut, 현재Audio FadeInS
        /// </summary>
        /// <param name="clip">New SoundClip</param>
        /// <param name="time">Fade Time</param>
        /// <param name="ease">Interpolate.EaseType</param>
        private void FadeTo(SoundClip clip, float time, Interpolate.EaseType ease)
        {
            if (_currentPlayingStatus == MusicPlayingStatus.None)
            {
                FadeIn(clip, time, ease);
            }
            else if (this.IsDifferentSound(clip))
            {
                //페이드 되는 중간에 또 바뀐다면 전전꺼 바로 Stop
                if (this._currentPlayingStatus == MusicPlayingStatus.AtoB)
                {
                    this.BGM_A_audio.Stop();
                    this._currentPlayingStatus = MusicPlayingStatus.SourceB;
                }
                else if (this._currentPlayingStatus == MusicPlayingStatus.BtoA)
                {
                    this.BGM_B_audio.Stop();
                    this._currentPlayingStatus = MusicPlayingStatus.SourceA;
                }

                _lastMusicClip = _currentMusicClip;
                _currentMusicClip = clip;
                this._lastMusicClip.SetFadeOut(time, ease);
                this._currentMusicClip.SetFadeIn(time, ease);
                if (_currentPlayingStatus == MusicPlayingStatus.SourceA)
                {
                    PlayAudioSource(BGM_B_audio, _currentMusicClip, 0.0f);
                    _currentPlayingStatus = MusicPlayingStatus.AtoB;
                }
                else if (_currentPlayingStatus == MusicPlayingStatus.SourceB)
                {
                    PlayAudioSource(BGM_A_audio, _currentMusicClip, 0.0f);
                    _currentPlayingStatus = MusicPlayingStatus.BtoA;
                }

                if (_currentMusicClip.HasLoop())
                {
                    this.isTicking = true;
                    DoLoopCheck();
                }
            }
        }

        private void FadeTo(int index, float time, Interpolate.EaseType ease)
        {
            this.FadeTo(DataManager.Sound().GetClipCopy(index), time, ease);
        }
        #endregion Fade

        #region MonoBehaviour

        protected override void Awake()
        {
            base.Awake();

            //믹서 불러오기
            if (this.mixer == null)
            {
                this.mixer = ResourceLoader.Load<AudioMixer>(MixerName);
            }

            /*
             * Root
             * |- Manager
             * |- FadeA
             * |- FadeB
             * |- UI
             * |- Effect[n]
             * 
             */
            if (this.audioRoot == null)
            {
                //오디오 루트 오브젝트 생성후
                audioRoot = new GameObject(ContainerName).transform;
                //매니저를 오디오 루트에 붙여놓는다.
                audioRoot.SetParent(transform);
                audioRoot.localPosition = Vector3.zero;
            }

            //BGM A source
            if (BGM_A_audio == null)
            {
                //AudioSource를 붙어서 게임오브젝트 생성.
                GameObject fadeAObj = new GameObject(BGM_Channel_A, typeof(AudioSource));
                //매니저에 붙인다.
                fadeAObj.transform.SetParent(audioRoot);
                fadeAObj.transform.localPosition = Vector3.zero;
                this.BGM_A_audio = fadeAObj.GetComponent<AudioSource>();
                //자동재생 끄기
                this.BGM_A_audio.playOnAwake = false;
                BGM_A_audio.spatialBlend = 0f; //0인경우 어디서나 동일하게 들린다
                BGM_A_audio.reverbZoneMix = 0f;
                BGM_A_audio.priority = 0;
                BGM_A_audio.spread = 0;
                BGM_A_audio.panStereo = 0;
                BGM_A_audio.dopplerLevel = 0;
            }

            //BGM B source
            if (BGM_B_audio == null)
            {
                GameObject fadeBObj = new GameObject(BGM_Channel_B, typeof(AudioSource));
                fadeBObj.transform.SetParent(audioRoot);
                fadeBObj.transform.localPosition = Vector3.zero;
                BGM_B_audio = fadeBObj.GetComponent<AudioSource>();
                BGM_B_audio.playOnAwake = false;
                BGM_B_audio.spatialBlend = 0f; //0인경우 어디서나 동일하게 들린다
                BGM_B_audio.reverbZoneMix = 0f;
                BGM_B_audio.priority = 0;
                BGM_B_audio.spread = 0;
                BGM_B_audio.panStereo = 0;
                BGM_B_audio.dopplerLevel = 0;
            }

            //UI오디오
            if (UI_audio == null)
            {
                GameObject uiObj = new GameObject(UI, typeof(AudioSource));
                uiObj.transform.SetParent(audioRoot);
                UI_audio = uiObj.GetComponent<AudioSource>();
                UI_audio.playOnAwake = false;
            }

            //이펙트
            if (this.effect_audios == null || this.effect_audios.Length == 0)
            {
                //채널 카운트만큼 오디오소스 만들고
                this.effect_PlayStartTime = new float[EffectChannelCount];
                this.effect_audios = new AudioSource[EffectChannelCount];

                //이펙트오디오 채널 오브젝트 만든다.
                for (int i = 0; i < EffectChannelCount; i++)
                {
                    effect_PlayStartTime[i] = 0.0f;
                    GameObject effectObj = new GameObject("Effect" + i.ToString(), typeof(AudioSource));
                    effectObj.transform.SetParent(audioRoot);
                    this.effect_audios[i] = effectObj.GetComponent<AudioSource>();
                    this.effect_audios[i].playOnAwake = false;
                    this.effect_audios[i].spatialBlend = 1;
                }
            }
            
            

            // if (this.mixer != null)
            {
                //오디오믹서 그룹을 설정해야 Audio Mixer에서 설정한 볼륨조절이 먹는다.
                this.BGM_A_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(BGMGroupName)[0];
                this.BGM_B_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(BGMGroupName)[0];
                this.UI_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(UIGroupName)[0];
                for (int i = 0; i < this.effect_audios.Length; i++)
                {
                    this.effect_audios[i].outputAudioMixerGroup = mixer.FindMatchingGroups(EffectGroupName)[0];
                }
            }

            VolumeInit();
        }

        //실제로 Fade가 동작하도록 DoFade를 실행시킨다
        void Update()
        {
            if (_currentMusicClip == null)
            {
                return;
            }

            if (_currentPlayingStatus == MusicPlayingStatus.SourceA)
            {
                _currentMusicClip.DoFade(Time.deltaTime, BGM_A_audio);
            }
            else if (_currentPlayingStatus == MusicPlayingStatus.SourceB)
            {
                _currentMusicClip.DoFade(Time.deltaTime, BGM_B_audio);
            }
            else if (_currentPlayingStatus == MusicPlayingStatus.AtoB)
            {
                this._lastMusicClip.DoFade(Time.deltaTime, BGM_A_audio);
                this._currentMusicClip.DoFade(Time.deltaTime, BGM_B_audio);
            }
            else if (_currentPlayingStatus == MusicPlayingStatus.BtoA)
            {
                this._lastMusicClip.DoFade(Time.deltaTime, BGM_B_audio);
                this._currentMusicClip.DoFade(Time.deltaTime, BGM_A_audio);
            }

            //State변경
            //A는 플레이중이고 B는 멈춰있으면 플레이타입은 A
            if (BGM_A_audio.isPlaying && this.BGM_B_audio.isPlaying == false)
            {
                this._currentPlayingStatus = MusicPlayingStatus.SourceA;
            }
            else if (BGM_B_audio.isPlaying && BGM_A_audio.isPlaying == false)
            {
                this._currentPlayingStatus = MusicPlayingStatus.SourceB;
            }
            else if (BGM_A_audio.isPlaying == false && BGM_B_audio.isPlaying == false)
            {
                this._currentPlayingStatus = MusicPlayingStatus.None;
            }
        }

        #endregion
    }
}