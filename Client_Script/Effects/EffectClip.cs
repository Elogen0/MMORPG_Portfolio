using System.Collections;
using System.Collections.Generic;
using Kame;
using Kame.Define;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 이펙트 프리팹과 경로와 타입등의 속성 데이터를 가지고 있게 되며
/// 프리팹 사전로딩 기능을 갖고 있고 - 풀링을 위한 기능이기도 합니다.
/// 이펙트 인스턴스 기능도 갖고 있으며 - 풀링과 연계해서 사용하기도 합니다.
/// </summary>
public class EffectClip
{   //추후 속성은 같지만 다른 이펙트 클립이 있을수 있어서 분별용.
    public int realId = 0;

    public EffectType effectType = EffectType.NORMAL;
    public GameObject effectPrefab = null;
    public string effectName = string.Empty; //이펙트이름
    public string effectPath = string.Empty; //이펙트 경로
    public string effectFullPath = string.Empty; //Path + Name
    
    public EffectClip() { }


    /// <summary>
    /// Effect를 ResourceManager에서 Load해서 effectPrefab에 세팅
    /// </summary>
    public void PreLoad()
    {
        this.effectFullPath = effectPath + effectName;
        //한번만 로딩
        if(this.effectFullPath != string.Empty && this.effectPrefab == null)
        {
            AddressableLoader.LoadAssetAsync<GameObject>(effectFullPath, result =>
            {
                this.effectPrefab = result;
            });
        }
    }
    
    /// <summary>
    /// effectprefab을 null로 설정하여 나중에 가비지컬렉터가 회수해가도록 함 
    /// </summary>
    public void ReleaseEffect()
    {
        AddressableLoader.Release(effectPrefab);
        effectPrefab = null;
    }

    /// <summary>
    /// 원하는 위치에 이펙트를 인스턴스합니다.
    /// </summary>    
    public GameObject Instantiate(Vector3 Pos)
    {
        if(this.effectPrefab == null)
        {
            this.PreLoad();
        }
        if(this.effectPrefab != null)
        {
            GameObject effect = GameObject.Instantiate(effectPrefab, Pos, Quaternion.identity);
            return effect;
        }
        return null;
    }



}
