using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using Kame.Define;
using UnityObject = UnityEngine.Object;


public class EditorHelper
{
    const string templateFilePath = "Assets/Scripts/EditorTools/Editor/Template/EnumTemplate.txt";
    const string targetFolderPath = AssetPath.Define;

    /// <summary>
    /// 경로 계산 함수.
    /// </summary>
    /// <param name="p_clip"></param>
    /// <returns></returns>
    public static string GetPath(UnityEngine.Object p_clip)
    {
        string retString = string.Empty;
        retString = AssetDatabase.GetAssetPath(p_clip);
        string[] path_node = retString.Split('/'); //Assets/9.ResourcesData/Resources/Sound/BGM.wav
        bool findResource = false;
        for (int i = 0; i < path_node.Length - 1; i++)
        {
            if (findResource == false)
            {
                if (path_node[i] == "Resources")
                {
                    findResource = true;
                    retString = string.Empty;
                }
            }
            else
            {
                retString += path_node[i] + "/";
            }
        }

        return retString;
    }

    /// <summary>
    /// Data 리스트를 enum structure로 뽑아주는 함수.
    /// </summary>
    public static void CreateEnumStructure(string enumName, StringBuilder data)
    {
        string entityTemplate = File.ReadAllText(templateFilePath);

        entityTemplate = entityTemplate.Replace("$DATA$", data.ToString());
        entityTemplate = entityTemplate.Replace("$ENUM$", enumName);
        if (Directory.Exists(targetFolderPath) == false)
        {
            Directory.CreateDirectory(targetFolderPath);
        }

        string FilePath = targetFolderPath + enumName + ".cs";
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
        File.WriteAllText(FilePath, entityTemplate);
    }

    /// <summary>
    /// 에디터상의 Add, Remove, Copy 공용버튼
    /// </summary>
    /// <param name="dataLoader"></param>
    /// <param name="selection"></param>
    /// <param name="source"></param>
    /// <param name="uiWidth"></param>
    public static void EditorToolTopLayer(BaseDataLoader dataLoader, ref int selection,
        ref UnityObject source, int uiWidth)
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("ADD", GUILayout.Width(uiWidth)))
            {
                dataLoader.AddData("New Data");
                selection = dataLoader.GetDataCount() - 1; //최종 리스트를 선택.
                source = null;
            }

            if (GUILayout.Button("Copy", GUILayout.Width(uiWidth)))
            {
                dataLoader.Copy(selection);
                source = null;
                selection = dataLoader.GetDataCount() - 1;
            }

            if (dataLoader.GetDataCount() > 1) //데이터 2개 이상이면
            {
                if (GUILayout.Button("Remove", GUILayout.Width(uiWidth)))
                {
                    source = null;
                    dataLoader.RemoveData(selection);
                }
            }

            if (selection > dataLoader.GetDataCount() - 1)
            {
                selection = dataLoader.GetDataCount() - 1;
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 데이터 리스트를 보여주는 스크롤뷰
    /// </summary>
    /// <param name="ScrollPosition"></param>
    /// <param name="dataLoader"></param>
    /// <param name="selection"></param>
    /// <param name="source"></param>
    /// <param name="uiWidth"></param>
    public static void EditorToolListLayer(ref Vector2 ScrollPosition, BaseDataLoader dataLoader, ref int selection,
        ref UnityObject source, int uiWidth)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(uiWidth));
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical("box");
            {
                ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
                {
                    if (dataLoader.GetDataCount() > 0)
                    {
                        int lastSelection = selection;
                        selection = GUILayout.SelectionGrid(selection,
                            dataLoader.GetNameList(true), 1); // true : name + 0,1,2.. 아이디 붙여줌
                        if (lastSelection != selection)
                        {
                            source = null;
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }
}