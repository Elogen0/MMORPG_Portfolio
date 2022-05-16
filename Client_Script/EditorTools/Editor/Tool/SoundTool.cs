using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using UnityObject = UnityEngine.Object;
using Kame.Define;
using Kame.Utilcene;
using Kame.Utils;
using UnityEngine.AddressableAssets;

public class SoundTool : EditorWindow
{
    public int uiWidthLarge = 450;
    public int uiWidthMiddle = 300;
    public int uiWidthSmall = 200;
    private int selection = 0;
    private Vector2 listScrollPosition = Vector2.zero;
    private Vector2 contentScrollPosition = Vector2.zero;
    private AudioClip soundSource;
    private static SoundDataLoader _soundDataLoader;

    [MenuItem("Tools/Sound Tool")]
    static void Init()
    {
        _soundDataLoader = CreateInstance<SoundDataLoader>();
        _soundDataLoader.LoadData();

        SoundTool window = GetWindow<SoundTool>(false, "Sound Tool");
        window.Show();
    }
    
    private void OnGUI()
    {
        if(_soundDataLoader == null)
        {
            return;
        }
        EditorGUILayout.BeginVertical();
        {
            // --------------- 상단 -------------------------
            UnityObject source = soundSource;
            //Add, Remove, Copy 공용 기능
            EditorHelper.EditorToolTopLayer(_soundDataLoader, ref selection, ref source, uiWidthMiddle);
            soundSource = (AudioClip)source;
            
            // ------------------ 중단 --------------------------
            EditorGUILayout.BeginHorizontal();
            {
                //---- Vertical1: Data list ---
                EditorHelper.EditorToolListLayer(ref listScrollPosition, _soundDataLoader, ref selection, ref source, uiWidthMiddle);
                SoundClip sound = _soundDataLoader.soundClips[selection];
                soundSource = (AudioClip)source;
                
                // --- Vertical2: content ------
                EditorGUILayout.BeginVertical();
                {
                    this.contentScrollPosition = EditorGUILayout.BeginScrollView(this.contentScrollPosition);
                    {
                        if(_soundDataLoader.GetDataCount() > 0)
                        {
                            //-------------- 2 -----------
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("ID", selection.ToString(), GUILayout.Width(uiWidthLarge));
                                _soundDataLoader.names[selection] = EditorGUILayout.TextField("Name", _soundDataLoader.names[selection], GUILayout.Width(uiWidthLarge));
                                sound.playType = (SoundPlayType)EditorGUILayout.EnumPopup("PlayType", sound.playType, GUILayout.Width(uiWidthLarge));
                                sound.maxVolume = EditorGUILayout.FloatField("Max Volume", sound.maxVolume, GUILayout.Width(uiWidthLarge));
                                sound.isLoop = EditorGUILayout.Toggle("LoopClip", sound.isLoop, GUILayout.Width(uiWidthLarge));
                                
                                EditorGUILayout.Separator();
                                if(this.soundSource == null && sound.clipName != string.Empty) // 한번 로드
                                {
                                    this.soundSource = AssetDatabase.LoadAssetAtPath<AudioClip>(sound.clipPath);
                                    //this.soundSource = ResourceLoader.Load<AudioClip>(sound.clipPath);
                                }
                                this.soundSource = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", this.soundSource, typeof(AudioClip), false, GUILayout.Width(uiWidthLarge));
#if USE_ADDRESSABLES
                                
                                if (GUILayout.Button("Register Asset", GUILayout.Width(uiWidthSmall)))
                                {
                                    if (soundSource)
                                    {
                                        AddressableUtil.RegisterAsset(soundSource, "Audios");
                                        AddressableUtil.SetLabel(soundSource, "Audios");    
                                    }
                                }
#endif
                                if(soundSource != null) //soundsource가 있으면 사운드 설정 다 들고옴
                                {
                                    sound.clipPath = EditorHelper.GetPath(soundSource);

                                    sound.clipName = soundSource.name;
                                    sound.pitch = EditorGUILayout.Slider("Pitch", sound.pitch, -3.0f, 3.0f, GUILayout.Width(uiWidthLarge));
                                    sound.dopplerLevel = EditorGUILayout.Slider("Doppler", sound.dopplerLevel, 0.0f, 5.0f, GUILayout.Width(uiWidthLarge));
                                    sound.rolloffMode = (AudioRolloffMode)EditorGUILayout.EnumPopup("volume Rolloff", sound.rolloffMode, GUILayout.Width(uiWidthLarge));
                                    sound.minDistance = EditorGUILayout.FloatField("min Distance", sound.minDistance, GUILayout.Width(uiWidthLarge));
                                    sound.maxDistance = EditorGUILayout.FloatField("Max Distance", sound.maxDistance, GUILayout.Width(uiWidthLarge));
                                    sound.spartialBlend = EditorGUILayout.Slider("PanLevel", sound.spartialBlend, 0.0f, 1.0f, GUILayout.Width(uiWidthLarge));
                                }
                                else
                                {
                                    sound.clipName = string.Empty;
                                    sound.clipPath = string.Empty;
                                }
                                
                                EditorGUILayout.Separator();
                                if(GUILayout.Button("Add Loop", GUILayout.Width(uiWidthMiddle)))
                                {
                                    _soundDataLoader.soundClips[selection].AddLoop();
                                }
                                for(int i = 0; i < _soundDataLoader.soundClips[selection].loopLastTime.Length; i++)
                                {
                                    EditorGUILayout.BeginVertical("box");
                                    {
                                        GUILayout.Label("Loop Step " + i, EditorStyles.boldLabel);
                                        if(GUILayout.Button("Remove", GUILayout.Width(uiWidthMiddle)))
                                        {
                                            _soundDataLoader.soundClips[selection].RemoveLoop(i);
                                            return;
                                        }
                                        
                                        sound.loopLastTime[i] = EditorGUILayout.FloatField("check Time", sound.loopLastTime[i], GUILayout.Width(uiWidthMiddle));
                                        sound.loopStartTime[i] = EditorGUILayout.FloatField("Set Time", sound.loopStartTime[i], GUILayout.Width(uiWidthMiddle));
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();


        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();
        // ----------------- 하단 ---------------------
        EditorGUILayout.BeginHorizontal();
        {
            if(GUILayout.Button("Reload"))
            {
                _soundDataLoader = CreateInstance<SoundDataLoader>();
                _soundDataLoader.LoadData();
                selection = 0;
                this.soundSource = null;
            }
            if(GUILayout.Button("Save"))
            {
                _soundDataLoader.SaveData();
                CreateEnumStructure();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                //AddressableUtil.BuildPlayerContents();
            }

        }
        EditorGUILayout.EndHorizontal();
    }
    
    /// <summary>
    /// SoundList Enum생성(Template파일필요)
    /// </summary>
    public void CreateEnumStructure()
    {
        string enumName = "SoundList";
        StringBuilder builder = new StringBuilder();
        for(int i = 0; i < _soundDataLoader.names.Length;i++)
        {
            if(!_soundDataLoader.names[i].ToLower().Contains("none"))
            {
                builder.AppendLine("    "+_soundDataLoader.names[i] + " = " +i.ToString()+"," );
            }
        }
        EditorHelper.CreateEnumStructure(enumName, builder);
    }

}
