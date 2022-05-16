using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ItemSampleProvider))]
public class ItemSampleProviderEditor : Editor
{
    [MenuItem("Assets/UI Mock-up Data")]
    public static void OpenInspector()
    {
        Selection.activeObject = ItemSampleProvider.Instance;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ItemSampleProvider provider = target as ItemSampleProvider;
        if (GUILayout.Button("Add Random Data"))
        {
            provider.AppendNewRandomData(provider.addSampleCount);
        }
        else if (GUILayout.Button("Clear Data"))
        {
            provider.ClearData();
        }

                    
    }
    
}
#endif

[System.Serializable]
public class ItemSampleData
{
    public string displayName;
    public string description;
    public string id;
    public int cost;
}

public class ItemSampleProvider : ScriptableObject
{
    public int addSampleCount = 0;
    private const string SettingFileDirectory = "Assets/Resources";

    private const string SettingFilePath = "Assets/Resources/ItemSampleData.asset";

    private static ItemSampleProvider _instance;

    public static ItemSampleProvider Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = Resources.Load<ItemSampleProvider>("ItemSampleData");
#if UNITY_EDITOR
            if (_instance == null)
            {
                if (!AssetDatabase.IsValidFolder(SettingFileDirectory))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }

                _instance = (ItemSampleProvider) AssetDatabase.LoadAssetAtPath(SettingFilePath,
                    typeof(ItemSampleProvider));

                if (_instance == null)
                {
                    _instance = CreateInstance<ItemSampleProvider>();
                    AssetDatabase.CreateAsset(_instance, SettingFilePath);
                }
            }
#endif
            return _instance;
        }
    }

    [SerializeField] private List<ItemSampleData> _sampleDatas = new List<ItemSampleData>();

    public ItemSampleData GetRandomData() => _sampleDatas[Random.Range(0, _sampleDatas.Count)];
    public ItemSampleData GetData(string id) => _sampleDatas.FirstOrDefault(item => item.id == id);
    public ItemSampleData[] GetAll() => _sampleDatas.ToArray();
    
    public void AppendNewRandomData(int count)
    {
        var webReq = UnityWebRequest.Get($"http://names.drycodes.com/{count * 2}");
        webReq.SendWebRequest().completed += (handle) =>
        {
            if (handle.isDone)
            {
                List<string> result = JsonConvert.DeserializeObject<List<string>>(webReq.downloadHandler.text);

                for (var i = 0; i < result.Count; ++i)
                {
                    _sampleDatas.Add(new ItemSampleData
                    {
                        displayName = result[i],
                        description = result[++i],
                        id = Guid.NewGuid().ToString(),
                        cost =  Random.Range(1000, 3000)
                    });
                }
            }
        };
    }

    public void ClearData()
    {
        _sampleDatas.Clear();
    }
}
