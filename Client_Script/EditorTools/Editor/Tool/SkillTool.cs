using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Kame.Game.Data;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SkillTool : EditorWindow
{
    public const string dataDirectory = "SkillData";
    
    public int selection = 0;
    public Vector2 listScrollPosition = Vector2.zero;
    public Vector2 contentsScrollPosition = Vector2.zero;
    
    public const int uiWidthSmall = 100;
    public const int uiWidthMiddle = 200;
    public const int uiWidthLarge = 300;
    public static Vector2 subRectSize = Vector2.one * 500f;
    public static Rect _listRect = new Rect(Vector2.zero, subRectSize);
    
    
    private SkillDataLoader loader;
    private List<string> skillNames;
    private string[] skillNameArray;
    public SkillData skillData = null;

    private SerializedObject _serializedObject;
    private SerializedProperty _serializedProperty;
    
    
    [MenuItem("Tools/Skill Tool")]
    static void Init()
    {
        SkillTool window = GetWindow<SkillTool>(false, "SkillTool");
        window.Show();
    }

    private void OnEnable()
    {
        Load();
        ScriptableObject target = this;
        _serializedObject = new SerializedObject(target);
        _serializedProperty = _serializedObject.FindProperty("skillData");
    }
    
    private void Load()
    {
        skillData = null;
        string filePath = "Assets/Resources/Data/SkillData.json";
        if (File.Exists(filePath) == false)
        {
            Debug.LogWarning("File not exist");
            string initText = "{list:[]}";
            File.WriteAllText(filePath, initText);
        }
        loader = DataManager.LoadJson<SkillDataLoader, int, SkillData>(dataDirectory);
        if (loader == null || loader.list == null)
        {
            
            loader = DataManager.LoadJson<SkillDataLoader, int, SkillData>(dataDirectory);
        }
        loader.list.Sort((l, r) => l.id.CompareTo(r.id) );
        skillNames = loader.list.ConvertAll(skill => skill.name);
        skillNameArray = skillNames.ToArray();

        SelectionChanged();
    }

    private void SelectionChanged()
    {
        if (loader.list.Count > 0 && selection == -1)
            selection = 0;
        if (loader.list.Count == 0)
        {
            skillData = null;
            return;
        }

        selection = Math.Max(0, selection);
        selection = Math.Min(loader.list.Count - 1, selection);
        
        skillData = loader.list[selection];
        EditorGUI.FocusTextInControl("");
        GUI.changed = true;
        Repaint();
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            DrawList();
            DrawContents();
        }
        EditorGUILayout.EndHorizontal();
        DrawBottomLayer();
    }

    private void DrawListTopLayer()
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("-"))
            {
                loader.RemoveData(selection);
                skillNames = loader.list.ConvertAll(skill => skill.name);
                skillNameArray = skillNames.ToArray();

                SelectionChanged();
            }
            if (GUILayout.Button("Copy"))
            {
                loader.Copy(selection);
                skillNames = loader.list.ConvertAll(skill => skill.name);
                skillNameArray = skillNames.ToArray();
                selection = loader.list.Count - 1;
                
                SelectionChanged();
            }
            
            if (GUILayout.Button("+"))
            {
                loader.AddData("New Skill");
                skillNames = loader.list.ConvertAll(skill => skill.name);
                skillNameArray = skillNames.ToArray();
                _serializedObject.Update();
                selection = loader.list.Count - 1;
                SelectionChanged();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    string filterWord = String.Empty;
    private void DrawList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(uiWidthMiddle));
        {
            DrawListTopLayer();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical("box");
            {
                filterWord = EditorGUILayout.TextField(filterWord, GUILayout.Width(uiWidthMiddle));
                listScrollPosition = EditorGUILayout.BeginScrollView(listScrollPosition);
                {
                    if (loader.list.Count > 0)
                    {
                        int lastSelection = selection;
                        for (int i = 0; i < skillNameArray.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(filterWord) &&
                                !skillNameArray[i].ToLower().Contains(filterWord.ToLower()))
                                continue;
                            if (selection == i)
                            {
                                var style = new GUIStyle(GUI.skin.button);
                                style.normal.background = Texture2D.grayTexture;
                                if (GUILayout.Button(skillNameArray[i], style,GUILayout.Width(uiWidthMiddle)))
                                {
                                    selection = i;
                                }    
                            }
                            else
                            {
                                if (GUILayout.Button(skillNameArray[i],GUILayout.Width(uiWidthMiddle)))
                                {
                                    selection = i;
                                }    
                            }
                        }
                        
                        if (lastSelection != selection)
                        {
                            skillData = loader.list[selection];
                            SelectionChanged();
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }


    private void DrawContents()
    {
        if (loader.list.Count == 0)
        {
            Debug.LogWarning("list count 0");
            return;
        }
        if (skillData == null)
        {
            Debug.LogWarning("QuestData is null");
            return;
        }
        _serializedObject.Update();
        EditorGUILayout.BeginVertical();
        {
            contentsScrollPosition = EditorGUILayout.BeginScrollView(contentsScrollPosition);
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.PropertyField(_serializedProperty, true);
                    EditorGUILayout.Separator();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
        _serializedObject.ApplyModifiedProperties();
    }

    private void DrawBottomLayer()
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Reload"))
            {
                Load();
                SelectionChanged();
            }

            if (GUILayout.Button("Save"))
            {
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(loader);
                File.WriteAllText(Application.dataPath + "/Resources/Data/SkillData.json", jsonString);
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                Load();
                SelectionChanged();
                Debug.Log("Save Skill");
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
