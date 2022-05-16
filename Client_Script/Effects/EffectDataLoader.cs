using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.IO;
using Kame.Define;

/// <summary>
/// 이펙트 클립 리스트와 이펙트 파일 이름과 경로를 가지고 있으며 파일을 읽고 쓰는 기능을 가지고 있다.
/// 데이터를 저장, 삭제하고 , 특정 클립을 얻어오고, 복사하는 기능
/// Effect Tool용 Data
/// </summary>
public class EffectDataLoader : BaseDataLoader
{
    public EffectClip[] effectClips = new EffectClip[0];

    public string clipPath = "Effects/";
    private string xmlFilePath = ""; //AppDirectory + dataDirectory
    private string xmlFileName = "effectData.xml";
    private string loadDataPath = "Data/effectData"; //resourceLoad 할때 사용하는 Path
    //XML 구분자.
    private const string EFFECT = "effect"; //저장 키.
    private const string CLIP = "clip"; //저장 키.

    private const string KEY_LENGTH = "length";
    private const string KEY_ID = "id";
    private const string KEY_NAME = "name";
    private const string KEY_EFFECT_TYPE = "effectType";
    private const string KEY_EFFECT_NAME = "effectName";
    private const string KEY_EFFECT_PATH = "effectPath";

    private EffectDataLoader() { }
    
    /// <summary>
    /// Xml에 저장된 데이터를 통하여
    /// 이펙트 Clip List를 만들어, 
    /// </summary>
    public void LoadData()
    {
        Debug.Log($"xmlFilePath = {Application.dataPath}  + {dataDirectory}");
        this.xmlFilePath = Application.dataPath + dataDirectory;
        TextAsset asset = (TextAsset)ResourceLoader.Load<TextAsset>(loadDataPath);
        if (asset == null || asset.text == null) //데이터가 없다.
        {
            this.AddData("New Effect");
            return;
        }

        using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
        {
            int currentID = 0;
            while(reader.Read())
            {
                if(reader.IsStartElement())
                {
                    switch(reader.Name)
                    {
                        case KEY_LENGTH:
                            int length = int.Parse(reader.ReadString());
                            this.names = new string[length];
                            this.effectClips = new EffectClip[length];
                            break;
                        case KEY_ID:
                            currentID = int.Parse(reader.ReadString());
                            this.effectClips[currentID] = new EffectClip();
                            this.effectClips[currentID].realId = currentID;
                            break;
                        case KEY_NAME:
                            this.names[currentID] = reader.ReadString();
                            break;
                        case KEY_EFFECT_TYPE:
                            this.effectClips[currentID].effectType = (EffectType)Enum.Parse(typeof(EffectType), reader.ReadString());
                            break;
                        case KEY_EFFECT_NAME:
                            this.effectClips[currentID].effectName = reader.ReadString();
                            break;
                        case KEY_EFFECT_PATH:
                            this.effectClips[currentID].effectPath = reader.ReadString();
                            break;
                    }
                }
            }

        }
    }

    /// <summary>
    /// EffectClip List를 Xml로 저장
    /// </summary>
    public void SaveData()
    {
        using (XmlTextWriter xml = new XmlTextWriter(xmlFilePath + xmlFileName, System.Text.Encoding.Unicode))
        {
            xml.WriteStartDocument();
            xml.WriteStartElement(EFFECT);
            xml.WriteElementString(KEY_LENGTH, GetDataCount().ToString());
            for(int i  = 0; i < this.names.Length; i++)
            {
                EffectClip clip = this.effectClips[i];
                xml.WriteStartElement(CLIP);
                xml.WriteElementString(KEY_ID, i.ToString());
                xml.WriteElementString(KEY_NAME, this.names[i]);
                xml.WriteElementString(KEY_EFFECT_TYPE, clip.effectType.ToString());
                xml.WriteElementString(KEY_EFFECT_PATH, clip.effectPath);
                xml.WriteElementString(KEY_EFFECT_NAME, clip.effectName);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
            xml.WriteEndDocument();
        }
    }

    /// <summary>
    /// New EffectClip을 생성
    /// </summary>
    /// <param name="newName">new Name</param>
    /// <returns></returns>
    public override int AddData(string newName)
    {
        if(this.names == null) // 막지우다 뻑날수 있으니까 적어도 하나는 있어라
        {
            this.names = new string[] { newName };
            this.effectClips = new EffectClip[] { new EffectClip() };
        }
        else
        {
            this.names = ArrayHelper.Add(newName, this.names);
            this.effectClips = ArrayHelper.Add(new EffectClip(), this.effectClips);
        }

        return GetDataCount();
    }

    /// <summary>
    /// EffectClip List에서 삭제
    /// </summary>
    /// <param name="index">삭제할 index</param>
    public override void RemoveData(int index)
    {
        this.names = ArrayHelper.Remove(index, this.names);
        if(this.names.Length == 0)
        {
            this.names = null;
        }
        this.effectClips = ArrayHelper.Remove(index, this.effectClips);
    }

    /// <summary>
    /// 모든 EffectClip삭제
    /// </summary>
    public void ClearData()
    {
        foreach(EffectClip clip in this.effectClips)
        {
            clip.ReleaseEffect();
        }
        this.effectClips = null;
        this.names = null;
    }

    /// <summary>
    /// EffectClip의 복사본을 반환
    /// </summary>
    /// <param name="index">EffectList에 설정된 Enum의 index</param>
    /// <returns></returns>
    public EffectClip GetClipCopy(int index)
    {
        if(index< 0 || index >= this.effectClips.Length)
        {
            return null;
        }
        EffectClip original = this.effectClips[index];
        EffectClip clip = new EffectClip();
        clip.effectFullPath = original.effectFullPath;
        clip.effectName = original.effectName;
        clip.effectType = original.effectType;
        clip.effectPath = original.effectPath;
        clip.realId = this.effectClips.Length;
        return clip;
    }
    
    /// <summary>
    /// 원하는 인덱스를 프리로딩해서 찾아준다.
    /// </summary>    
    public EffectClip GetClip(int index) //todo : enum으로 바꾸기
    {
        if(index < 0 || index >= this.effectClips.Length)  //out of index
        {
            return null;
        }
        effectClips[index].PreLoad();
        return effectClips[index];
    }

    /// <summary>
    /// index의 EffectClip을 복제하여 리스트에 추가
    /// </summary>
    /// <param name="index">복제할 index</param>
    public override void Copy(int index)
    {
        this.names = ArrayHelper.Add(this.names[index], this.names);
        this.effectClips = ArrayHelper.Add(GetClipCopy(index), this.effectClips);
    }


}
