using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Kame;
using Kame.Define;
using Kame.Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

public class ObjectManager : SingletonMono<ObjectManager>
{
    public MyPlayerController MyPlayer { get; private set; }
    public PlayerCharacterInfo MyPlayerInfo { get; set; }
    private Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
    private bool _isCoroutineRunning;
    private Queue<InfoWrapper> _messageQueue = new Queue<InfoWrapper>();
    private TransformAnchor _playerTransformAnchor;
    private struct InfoWrapper
    {
        public ObjectInfo info;
        public bool myPlayer;
    }
    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    public void Add(ObjectInfo info, bool myPlayer = false)
    {
        _messageQueue.Enqueue(new InfoWrapper(){info = info, myPlayer = myPlayer});
        ExecuteRoutine();
    }

    private void ExecuteRoutine()
    {
        if (_isCoroutineRunning)
            return;
        if (_messageQueue.Count > 0)
        {
            _isCoroutineRunning = true;
            InfoWrapper wrapper = _messageQueue.Dequeue();
            StartCoroutine(CoAdd(wrapper.info, wrapper.myPlayer));
        }
    }

    IEnumerator CoAdd(ObjectInfo info, bool myPlayer = false)
    {
        do
        {
            if (MyPlayer != null && MyPlayer.Id == info.ObjectId)
                break;
            if (_objects.ContainsKey(info.ObjectId))
                break;
            GameObject go = null;

            GameObjectType objectType = GetObjectTypeById(info.ObjectId);
            if (objectType == GameObjectType.Player)
            {
                if (myPlayer)
                {
                    if (info.StatInfo.CharacterId == 1)
                    {
                        var request = AddressableLoader.Instantiate("Assets/Game/Prefab/Controller/Player/Player_Warrior.prefab");
                        yield return request.Wait();
                        go = request.Result;
                    }

                    if (go == null)
                        break;
                    go.transform.position = new Vector3(info.PosInfo.PosX, info.PosInfo.PosH, info.PosInfo.PosY);
                    go.name = info.Name;
                    _objects.Add(info.ObjectId, go);

                    MyPlayer = go.GetOrAddComponent<MyPlayerController>();
                    MyPlayer.Id = info.ObjectId;
                    MyPlayer.PosInfo = info.PosInfo;
                    MyPlayer.InitStat(info.StatInfo);
                    MyPlayer.tag = TagAndLayer.TagName.Player;
                    MyPlayer.SyncPos();
                    _playerTransformAnchor.Provide(go.transform);
                }
                else
                {
                    if (info.StatInfo.CharacterId == 1)
                    {
                        var request = AddressableLoader.Instantiate("Assets/Game/Prefab/Controller/Player/Player_Warrior.prefab");
                        yield return request.Wait();
                        go = request.Result;
                    }

                    if (go == null)
                        break;
                    go.transform.position = new Vector3(info.PosInfo.PosX, info.PosInfo.PosH, info.PosInfo.PosY);

                    // Ray ray = new Ray(new Vector3(info.PosInfo.PosX, info.PosInfo.PosH + 1000, info.PosInfo.PosY),
                    //     Vector3.down);
                    // Physics.Raycast(ray, out RaycastHit hit, Single.MaxValue, TagAndLayer.LayerMasking.Ground);
                    // NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, 1f, NavMesh.AllAreas)
                    // GameObject go = Instantiate(prefab, navMeshHit.position, Quaternion.identity);
                    go.name = info.Name;
                    _objects.Add(info.ObjectId, go);

                    PlayerController pc = go.GetOrAddComponent<PlayerController>();
                    pc.Id = info.ObjectId;
                    pc.PosInfo = info.PosInfo;
                    pc.InitStat(info.StatInfo);
                    pc.SyncPos();
                }
            }
            else if (objectType == GameObjectType.Monster)
            {
                var request = AddressableLoader.Instantiate("Assets/Game/Prefab/Controller/Enemy/Goblin.prefab");
                yield return request.Wait();
                go = request.Result;
                go.transform.position = new Vector3(info.PosInfo.PosX, info.PosInfo.PosH, info.PosInfo.PosY);

                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                MonsterController mc = go.GetOrAddComponent<MonsterController>();
                mc.Id = info.ObjectId;
                mc.PosInfo = info.PosInfo;
                mc.InitStat(info.StatInfo);
                mc.SyncPos();
            }
            else if (objectType == GameObjectType.Projectile)
            {
                var request = AddressableLoader.Instantiate("Assets/Game/Prefab/Controller/Enemy/Goblin.prefab");
                yield return request.Wait();
                go = request.Result;
                //todo: 비트플래그를 좀더 세분화시켜서 Projectile 세분화
                go.transform.position = new Vector3(info.PosInfo.PosX, info.PosInfo.PosH, info.PosInfo.PosY);
                go.name = "Arrow";
                _objects.Add(info.ObjectId, go);

                ArrowController ac = go.GetOrAddComponent<ArrowController>();
                ac.PosInfo = info.PosInfo;
                ac.InitStat(info.StatInfo);
                ac.SyncPos();
            }
        } while (false);
        
        _isCoroutineRunning = false;
        ExecuteRoutine();
        yield return null;
    }

    public void Remove(int id)
    {
        if (MyPlayer != null && MyPlayer.Id == id)
            return;
        if (!_objects.ContainsKey((id)))
            return;
        
        GameObject go = FindById(id);
        if (go == null)
            return;
        
        _objects.Remove(id);

        AddressableLoader.ReleaseInstance(go);
    }

    public void RemoveMyPlayer()
    {
        if (MyPlayer == null)
            return;

        if (_objects.Remove(MyPlayer.Id))
        {
            AddressableLoader.ReleaseInstance(MyPlayer.gameObject);
            MyPlayer = null;
        }
    }

    public GameObject FindCreature(Vector3 pos)
    {
        foreach (var objectsValue in _objects.Values)
        {
            
        }
        return null;
    }

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (var obj in _objects.Values)
        {
            if (condition.Invoke(obj))
                return obj;
        }

        return null;
    }

    public GameObject FindById(int id)
    {
        _objects.TryGetValue(id, out GameObject go);
        return go;
    }

    public void Clear()
    {
        foreach (var obj in _objects.Values)
        {
            Destroy(obj);
        }
        _objects.Clear();
        MyPlayer = null;
    }

    protected override void Awake()
    {
        base.Awake();
        _playerTransformAnchor = TransformAnchor.Get<TransformAnchor>(ResourcePath.PlayerTransformAnchor);
    }
}
