using UnityEngine;
using UnityEditor;
using System.Text;
using UnityObject = UnityEngine.Object;
using UnityEditor.EditorTools;
using System;
using Kame;
using Kame.Define;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 이펙트데이터를 편집하는 툴
/// </summary>
public class EffectTool : EditorWindow
{
    //UI 그리는데 필요한 변수들.
    public int uiWidthLarge = 300;
    public int uiWidthMiddle = 200;
    private int selection = 0;
    private Vector2 listScrollPosition = Vector2.zero;
    private Vector2 contentScrollPosition = Vector2.zero;
    //이펙트 클립
    private GameObject effectSource = null;
    //이펙트 데이터
    private static EffectDataLoader _effectDataLoader;

    [MenuItem("Tools/Effect Tool")] //상단바 메뉴
    static void Init()
    {
        _effectDataLoader = ScriptableObject.CreateInstance<EffectDataLoader>();
        _effectDataLoader.LoadData();

        EffectTool window = GetWindow<EffectTool>(false, "Effect Tool");
        window.Show();
    }

    private void OnGUI()
    {
        if(_effectDataLoader == null)
        {
            return;
        }
        EditorGUILayout.BeginVertical();
        {
            //-------------------------------------- 상단(add, remove . copy) --------------------------------------
            //UnityObject로 하는 이유: GameObject, AudioClip, AnimationClip이 들어갈수 있기때문
            UnityObject source = effectSource;
            //Add, Remove, Copy 공용 기능
            EditorHelper.EditorToolTopLayer(_effectDataLoader, ref selection, ref source, this.uiWidthMiddle);
            effectSource = (GameObject)source;

            // ------------------------------------ 중단 -------------------------------------------
            EditorGUILayout.BeginHorizontal();
            {   
                //vertical1 : 데이터 목록
                EditorHelper.EditorToolListLayer(ref listScrollPosition, _effectDataLoader, ref selection, ref source, this.uiWidthLarge);
                effectSource = (GameObject)source;

                //vertical2 : 설정부분
                EditorGUILayout.BeginVertical();
                {
                    contentScrollPosition = EditorGUILayout.BeginScrollView(this.contentScrollPosition);
                    {
                        if(_effectDataLoader.GetDataCount() > 0) //선택했다면
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("ID:", selection.ToString(), GUILayout.Width(uiWidthLarge));
                                _effectDataLoader.names[selection] = EditorGUILayout.TextField("이름:", _effectDataLoader.names[selection], GUILayout.Width(uiWidthLarge * 1.5f));
                                _effectDataLoader.effectClips[selection].effectType = (EffectType)EditorGUILayout.EnumPopup("이펙트 타입:", _effectDataLoader.effectClips[selection].effectType, GUILayout.Width(uiWidthLarge));
                                
                                EditorGUILayout.Separator();
                                //Resource에서 Clip을 받아와 Data에 Set
                                if(effectSource == null && _effectDataLoader.effectClips[selection].effectName != string.Empty)
                                {
                                    _effectDataLoader.effectClips[selection].PreLoad();
                                    AddressableLoader.LoadAssetAsync<GameObject>(_effectDataLoader.effectClips[selection].effectPath + _effectDataLoader.effectClips[selection].effectName, result =>
                                    {
                                        effectSource = result;
                                    });
                                }
                                effectSource = (GameObject)EditorGUILayout.ObjectField("이펙트:", this.effectSource, typeof(GameObject), false, GUILayout.Width(uiWidthLarge * 1.5f));
                                
                                if(effectSource != null)
                                {
                                    _effectDataLoader.effectClips[selection].effectPath = EditorHelper.GetPath(this.effectSource);
                                    _effectDataLoader.effectClips[selection].effectName = effectSource.name;
                                }
                                else
                                {
                                    _effectDataLoader.effectClips[selection].effectPath = string.Empty;
                                    _effectDataLoader.effectClips[selection].effectName = string.Empty;
                                    effectSource = null;
                                }
                                EditorGUILayout.Separator();
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
        //--------------------------------- 하단 --------------------------------
        EditorGUILayout.BeginHorizontal();
        {
            if(GUILayout.Button("Reload Settings"))
            {
                _effectDataLoader = CreateInstance<EffectDataLoader>();
                _effectDataLoader.LoadData();
                selection = 0;
                this.effectSource = null;
            }
            if(GUILayout.Button("Save"))
            {
                EffectTool._effectDataLoader.SaveData();
                CreateEnumStructure();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// EffectName으로 Enum을 만들어주는 함수
    /// </summary>
    public void CreateEnumStructure()
    {
        string enumName = "EffectList";
        StringBuilder builder = new StringBuilder();
        builder.AppendLine();
        for(int i = 0; i < _effectDataLoader.names.Length; i++)
        {
            if(_effectDataLoader.names[i] != string.Empty)
            {
                builder.AppendLine("     " + _effectDataLoader.names[i] + " =  " + i + ","); //tab + name = i,
            }
        }
        EditorHelper.CreateEnumStructure(enumName, builder);
    }
}
