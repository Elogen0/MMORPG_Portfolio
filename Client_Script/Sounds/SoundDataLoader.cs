using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.IO;
using Kame.Define;

/// <summary>
/// 사운드 클립데이터베이스.
/// Xml로 사운드 데이터를 Serialization해서 정보를 저장한다.
/// 
/// 사운드 클립을 배열로 소지, 사운드 데이터를 저장하고 로드하고,
/// 프리로딩을 갖고 있다.
/// Tool 용 Data
/// </summary>
public class SoundDataLoader : BaseDataLoader
{
    public SoundClip[] soundClips = new SoundClip[0];

    private string clipPath = "Sound/";
    private string xmlFilePath = "";//저장용
    private string xmlFileName = "soundData.xml";//저장용
    private string dataPath = "Data/soundData"; //리소스 로드 용(확장자 x) : xml파일 경로
    private static string SOUND = "sound"; //저장 키
    private static string CLIP = "clip"; //저장 키

    private const string TYPE_LENGTH         = "length";
    private const string TYPE_ID             = "id";
    private const string TYPE_NAME           = "name";
    private const string TYPE_LOOPS          = "loops";
    private const string TYPE_LOOP           = "loop";
    private const string TYPE_MAXVOL         = "maxvol"; 
    private const string TYPE_PITCH          = "pitch";
    private const string TYPE_DOPPLERLEVEL   = "dopplerlevel"; 
    private const string TYPE_ROLLOFFMODE    = "rolloffmode";
    private const string TYPE_MINDISTANCE    = "mindistance";
    private const string TYPE_MAXDISTANCE    = "maxdistance"; 
    private const string TYPE_SPARTIALBLEND  = "spartialblend"; 
    private const string TYPE_CLIPPATH       = "clippath";
    private const string TYPE_CLIPNAME       = "clipname";
    private const string TYPE_CHECKTIMECOUNT = "checktimecount"; 
    private const string TYPE_CHECKTIME      = "checktime";
    private const string TYPE_SETTIMECOUNT   = "settimecount";
    private const string TYPE_SETTIME        = "settime";
    private const string TYPE_TYPE           = "type";

    public SoundDataLoader() { }

    
    public void SaveData()
    {
        using (XmlTextWriter xml = new XmlTextWriter(xmlFilePath + xmlFileName, System.Text.Encoding.Unicode))
        {
            xml.WriteStartDocument();
            xml.WriteStartElement(SOUND);
            {
                xml.WriteElementString(TYPE_LENGTH, GetDataCount().ToString());
                xml.WriteWhitespace("\n");
                for (int i = 0; i < this.names.Length; i++)
                {
                    SoundClip clip = this.soundClips[i];
                    xml.WriteStartElement(CLIP);
                    {
                        xml.WriteElementString(TYPE_ID, i.ToString());
                        xml.WriteElementString(TYPE_NAME, this.names[i]);
                        xml.WriteElementString(TYPE_LOOPS, clip.loopLastTime.Length.ToString());
                        xml.WriteElementString(TYPE_MAXVOL, clip.maxVolume.ToString());
                        xml.WriteElementString(TYPE_PITCH, clip.pitch.ToString());
                        xml.WriteElementString(TYPE_DOPPLERLEVEL, clip.dopplerLevel.ToString());
                        xml.WriteElementString(TYPE_ROLLOFFMODE, clip.rolloffMode.ToString());
                        xml.WriteElementString(TYPE_MINDISTANCE, clip.minDistance.ToString());
                        xml.WriteElementString(TYPE_MAXDISTANCE, clip.maxDistance.ToString());
                        xml.WriteElementString(TYPE_SPARTIALBLEND, clip.spartialBlend.ToString());
                        if (clip.isLoop == true)
                        {
                            xml.WriteElementString(TYPE_LOOP, "true");
                        }

                        xml.WriteElementString(TYPE_CLIPPATH, clip.clipPath);
                        xml.WriteElementString(TYPE_CLIPNAME, clip.clipName);
                        xml.WriteElementString(TYPE_CHECKTIMECOUNT, clip.loopLastTime.Length.ToString());
                        string str = "";
                        foreach (float t in clip.loopLastTime)
                        {
                            str += t.ToString() + "/";
                        }

                        xml.WriteElementString(TYPE_CHECKTIME, str);

                        str = "";
                        xml.WriteElementString(TYPE_SETTIMECOUNT, clip.loopStartTime.Length.ToString());
                        foreach (float t in clip.loopStartTime)
                        {
                            str += t.ToString() + "/";
                        }

                        xml.WriteElementString(TYPE_SETTIME, str);

                        xml.WriteElementString(TYPE_TYPE, clip.playType.ToString());
                    }
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndElement();
            xml.WriteEndDocument();
        }
    }
    
    public void LoadData()
    {
        xmlFilePath = Application.dataPath + dataDirectory;
        TextAsset asset = ResourceLoader.Load<TextAsset>(dataPath);
        if(asset == null || asset.text == null)
        {
            this.AddData("NewSound");
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
                        case TYPE_LENGTH:
                            int length = int.Parse(reader.ReadString());
                            this.names = new string[length];
                            this.soundClips = new SoundClip[length];
                            break;
                        case "clip":
                            break;
                        case TYPE_ID:
                            currentID = int.Parse(reader.ReadString());
                            soundClips[currentID] = new SoundClip();
                            soundClips[currentID].id = currentID;
                            break;
                        case TYPE_NAME:
                            this.names[currentID] = reader.ReadString();
                            break;
                        case TYPE_LOOPS:
                            int count = int.Parse(reader.ReadString());
                            soundClips[currentID].loopLastTime = new float[count];
                            soundClips[currentID].loopStartTime = new float[count];
                            break;
                        case TYPE_MAXVOL:
                            soundClips[currentID].maxVolume = float.Parse(reader.ReadString());
                            break;
                        case TYPE_PITCH:
                            soundClips[currentID].pitch = float.Parse(reader.ReadString());
                            break;
                        case TYPE_DOPPLERLEVEL:
                            soundClips[currentID].dopplerLevel = float.Parse(reader.ReadString());
                            break;
                        case TYPE_ROLLOFFMODE:
                            soundClips[currentID].rolloffMode = (AudioRolloffMode)
                                Enum.Parse(typeof(AudioRolloffMode), reader.ReadString());
                            break;
                        case TYPE_MINDISTANCE:
                            soundClips[currentID].minDistance = float.Parse(reader.ReadString());
                            break;
                        case TYPE_MAXDISTANCE:
                            soundClips[currentID].maxDistance = float.Parse(reader.ReadString());
                            break;
                        case TYPE_SPARTIALBLEND:
                            soundClips[currentID].spartialBlend = float.Parse(reader.ReadString());
                            break;
                        case TYPE_LOOP:
                            soundClips[currentID].isLoop = true;
                            break;
                        case TYPE_CLIPPATH:
                            soundClips[currentID].clipPath = reader.ReadString();
                            break;
                        case TYPE_CLIPNAME:
                            soundClips[currentID].clipName = reader.ReadString();
                            break;
                        case TYPE_CHECKTIMECOUNT:
                            break;
                        case TYPE_CHECKTIME:
                            SetLoopTime(true, soundClips[currentID], reader.ReadString());
                            break;
                        case TYPE_SETTIME:
                            SetLoopTime(false, soundClips[currentID], reader.ReadString());
                            break;
                        case TYPE_TYPE:
                            soundClips[currentID].playType = (SoundPlayType)
                                Enum.Parse(typeof(SoundPlayType), reader.ReadString());
                            break;
                    }
                }
            }
        }
        //preloading test (todo: 너무 무거우면 뺄것)
        // foreach(SoundClip clip in soundClips)
        // {
        //     clip.PreLoad();
        // }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isCheck"></param>
    /// <param name="clip"></param>
    /// <param name="timeString"></param>
    void SetLoopTime(bool isCheck, SoundClip clip, string timeString)
    {
        string[] time = timeString.Split('/');
        for(int i = 0; i < time.Length;i++)
        {
            if(time[i] != string.Empty)
            {
                if(isCheck == true)
                {
                    clip.loopLastTime[i] = float.Parse(time[i]);
                }
                else
                {
                    clip.loopStartTime[i] = float.Parse(time[i]);
                }
            }
        }
    }

    public override int AddData(string newName)
    {
        if(this.names == null)
        {
            this.names = new string[] { newName };
            this.soundClips = new SoundClip[] { new SoundClip() };
        }
        else
        {
            this.names = ArrayHelper.Add(newName, names);
            this.soundClips = ArrayHelper.Add(new SoundClip(), soundClips);
        }
        return GetDataCount();
    }

    public override void RemoveData(int index)
    {
        this.names = ArrayHelper.Remove(index, this.names);
        if(this.names.Length == 0)
        {
            this.names = null;
        }
        this.soundClips = ArrayHelper.Remove(index, this.soundClips);
    }
    
    public SoundClip GetClipCopy(int index)
    {
        if(index < 0 || index >= soundClips.Length) //out of index
        {
            return null;
        }
        SoundClip original = soundClips[index];
        SoundClip clip = CopyData(original, index);

        clip.Load();
        
        return clip;
    }

    public void GetClipCopyAsync(int index, Action<SoundClip> completed)
    {
        if(index < 0 || index >= soundClips.Length) //out of index
        {
            return;
        }
        SoundClip original = soundClips[index];
        SoundClip clip = CopyData(original, index);
        clip.LoadAsync(completed);
    }
    
    private SoundClip CopyData(SoundClip original, int index)
    {
        SoundClip clip = new SoundClip();
        clip.id = index;
        clip.clipPath = original.clipPath;
        clip.clipName = original.clipName;
        clip.maxVolume = original.maxVolume;
        clip.pitch = original.pitch;
        clip.dopplerLevel = original.dopplerLevel;
        clip.rolloffMode = original.rolloffMode;
        clip.minDistance = original.minDistance;
        clip.maxDistance = original.maxDistance;
        clip.spartialBlend = original.spartialBlend;
        clip.isLoop = original.isLoop;
        clip.loopLastTime = new float[original.loopLastTime.Length];
        clip.loopStartTime = new float[original.loopStartTime.Length];
        clip.playType = original.playType;
        for(int i  = 0; i < clip.loopLastTime.Length;i++)
        {
            clip.loopLastTime[i] = original.loopLastTime[i];
            clip.loopStartTime[i] = original.loopStartTime[i];
        }

        return clip;
    }

    public override void Copy(int index)
    {
        this.names = ArrayHelper.Add(this.names[index], this.names);
        this.soundClips = ArrayHelper.Add(GetClipCopy(index), soundClips);
    }
}
