using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheetLocalization : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private Langs langs;
    [NonSerialized]
    Dictionary<int, Dictionary<string, string>> cache = new Dictionary<int, Dictionary<string, string>>();
    private int currentLanguageHash = "english".GetHashCode();

    private const string sheetURL =
        "https://docs.google.com/spreadsheets/d/1D5Ww29SOgS5V_CyaKjvkLhiTcIvCI2qO_rxlxvLoJgA/";
    private const string exportFormat = "export?format=tsv";
    private const string requestMessage = sheetURL + exportFormat;
    void Awake()
    {
        
        LoadSetting();
    }

    [ContextMenu("언어 가져오기")]
    void GetLanguage()
    {
        StartCoroutine(CoGetLang());
    }

    IEnumerator CoGetLang()
    {
        UnityWebRequest www = UnityWebRequest.Get(requestMessage);
        yield return www.SendWebRequest();
        print(www.downloadHandler.text);
        Convert(www.downloadHandler.text);
    }

    void Convert(string tsv)
    {
        //txv -> matrix
        string[] splitEnter = tsv.Split('\n');
        int rowSize = splitEnter.Length;
        int columnSize = splitEnter[0].Split('\t').Length;
        string[,] matrix = new string[rowSize, columnSize];
        for (int rowIndex = 0; rowIndex < rowSize; ++rowIndex)
        {
            string[] splitTab = splitEnter[rowIndex].Split('\t');
            for (int columnIndex = 0; columnIndex < columnSize; columnIndex++)
            {
                matrix[rowIndex, columnIndex] = splitTab[columnIndex];
            }
        }


        //matrix -> List of language Country
        langs = new Langs();
        for (int col = 0; col < columnSize; col++)
        {
            LanguageCountry languageCountry = new LanguageCountry();
            languageCountry.lang = matrix[0, col];
            languageCountry.langLocalize = matrix[1, col];

            for (int row = 2; row < rowSize; row++)
                languageCountry.value.Add(matrix[row, col]);
            langs.list.Add(languageCountry);
        }

        string json = JsonUtility.ToJson(langs);
        Debug.Log(json);
        
        // for (int col = 1; col < columnSize; col++)
        // {
        //     Dictionary<string, string> dicOfOneLanguage = new Dictionary<string, string>();
        //     cache.Add(matrix[0, col].ToLower(), dicOfOneLanguage);
        //     for (int row = 0; row < rowSize; row++)
        //     {
        //         dicOfOneLanguage.Add(matrix[row, 0], matrix[row, col]);
        //     }
        // }
    }


    string Localize(string key)
    {
        if (cache[currentLanguageHash].TryGetValue(key, out string localizedString))
        {
            return localizedString;
        }

        return "Localize-Not Found Key";
    }
    
    void LoadSetting()
    {
        int languageSetting = PlayerPrefs.GetInt("LanguageIndex", string.Empty.GetHashCode());
        if (languageSetting == string.Empty.GetHashCode())
        {
            int systemLanguage = Application.systemLanguage.ToString().ToLower().GetHashCode();
            if (cache.ContainsKey(systemLanguage))
                languageSetting = systemLanguage;
            else
            {
                languageSetting = "english".GetHashCode();
            }
        }
        SetLanguage(languageSetting);
    }

    public void SetLanguage(int languageHash)
    {
        currentLanguageHash = languageHash;
        PlayerPrefs.SetInt("LanguageIndex", languageHash);
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
    }
}

[System.Serializable]
public class LanguageCountry
{
    public string lang, langLocalize;
    public List<string> value = new List<string>();
}

[System.Serializable]
public class Langs
{
    public List<LanguageCountry> list = new List<LanguageCountry>();
}