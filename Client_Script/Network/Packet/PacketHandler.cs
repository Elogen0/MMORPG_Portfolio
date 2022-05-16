using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using DummyClient;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Kame.Game;
using Kame.Game.Data;
using Kame;
using Kame.Quests;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using Random = UnityEngine.Random;

class PacketHandler
{
    public static void S_ChatHandler(PacketSession session, IMessage packet)
    {
        S_Chat pkt = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;
        Debug.Log($"{pkt.Name}: {pkt.Msg}");
    }
    
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_EnterGame pkt = packet as S_EnterGame;
        ObjectManager.Instance.Add(pkt.Player, myPlayer: true);
    }
    
    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame pkt = packet as S_LeaveGame;
        //ObjectManager.Instance.Clear();

        ObjectManager.Instance.RemoveMyPlayer();
    }
    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        //Debug.Log("S_Spawn");
        S_Spawn pkt = packet as S_Spawn;
        foreach (var obj in pkt.Objects)
        {
            ObjectManager.Instance.Add(obj, myPlayer: false);
        }
    }
    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn pkt = packet as S_Despawn;
        foreach (var id in pkt.ObjectIds)
        {
            ObjectManager.Instance.Remove(id);
        }
    }
    
    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;
        GameObject go = ObjectManager.Instance.FindById(movePacket.ObjectId);
        if (go == null)
            return;
        if (ObjectManager.Instance.MyPlayer.Id == movePacket.ObjectId)
        {
            return;
        }
        if (go.TryGetComponent(out BaseController bc))
        {
            bc.PosInfo = movePacket.PosInfo;
        }
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        S_Skill skillPacket = packet as S_Skill;
        Debug.Log($"S_SkillHandler : {skillPacket.Info.SkillId}");

        GameObject go = ObjectManager.Instance.FindById(skillPacket.ObjectId);
        if (go == null)
            return;
        if (go == ObjectManager.Instance.MyPlayer.gameObject)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.UseSkill(skillPacket.Info);
        }
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp hpPacket = packet as S_ChangeHp;
        GameObject go = ObjectManager.Instance.FindById(hpPacket.ObjectId);

        if (go == null)
            return;
        
        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            float changedHp = cc.Stat.Hp;
            //Debug.Log($"ChnageHp :{hpPacket.ObjectId} :: {hpPacket.Hp}");
            cc.Stat.Hp = hpPacket.Hp;
            
            cc.TakeDamage(ObjectManager.Instance.FindById(hpPacket.AttackerId), hpPacket.Damage);
            //ObjectSpawner.Instance.SpawnAsync(null, DamageText.Path, cc.transform.position + Vector3.up * 2f, Quaternion.identity, null,((int)changedHp).ToString());
        }
    }
    
    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        S_Die diePacket = packet as S_Die;
        GameObject go = ObjectManager.Instance.FindById(diePacket.ObjectId);

        if (go == null)
            return;
        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.OnDead(cc.GetComponent<Health>());
        }
    }

    public static void S_ConnectedHandler(PacketSession session, IMessage packet)
    {
        Debug.Log("S_ConnectedHandler");
        C_Login loginPacket = new C_Login();
        
        string path = Application.dataPath;
        // loginPacket.UniqueId = SystemInfo.deviceUniqueIdentifier;
        loginPacket.UniqueId = path.GetHashCode().ToString();
        NetworkManager.Instance.Send(loginPacket);
    }

    //로그인 OK + 캐릭터 목록
    public static void S_LoginHandler(PacketSession session, IMessage packet)
    {
        S_Login loginPacket = packet as S_Login;
        Debug.Log($"LoginOk({loginPacket})");

        UI_View view =  UI_ViewNavigation.Instance.Show("CharSelect");
        view.GetComponent<UI_CharacterSelect>().LoadCharacter(loginPacket.Players);
        
        // //Todo: 로비에서 UI캐릭터 보여주고, 선택할 수 있도록
        // if (loginPacket.Players == null || loginPacket.Players.Count == 0)
        // {
        //     C_CreatePlayer createPacket = new C_CreatePlayer();
        //     createPacket.Name = $"Player_{Random.Range(0, 10000):0000}";
        //     NetworkManager.Instance.Send(createPacket);
        // }
        // else
        // {
        //     //임시 : 무조건 첫번째 캐릭으로 로그인
        //     PlayerCharacterInfo info = loginPacket.Players[0];
        //     C_EnterGame enterGamePacket = new C_EnterGame();
        //     enterGamePacket.Name = info.Name;
        //     NetworkManager.Instance.Send(enterGamePacket);
        // }
    }

    public static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        S_CreatePlayer createOKPacket = (S_CreatePlayer) packet;
        if (!createOKPacket.CreateOK)
        {
            UI_Popup popup = UIManager.Instance.ShowPopup<UI_Popup>("canvas_poopup");
            popup.SetMessage("cannot create Character");
            popup.SetButton(0,"OK");
        }
        else
        {
            UI_View view = UI_ViewNavigation.Instance.Show("CharSelect");
            view.GetComponent<UI_CharacterSelect>().LoadCharacter(createOKPacket.Players);
        }
    }

    public static void S_ItemListHandler(PacketSession session, IMessage packet)
    {
        S_ItemList itemList = (S_ItemList) packet;
        
        InventoryObject equip = ResourceLoader.Load<InventoryObject>("ScriptableObjects/Equipment");
        InventoryObject inv = ResourceLoader.Load<InventoryObject>("ScriptableObjects/Inventory");
        equip.Clear();
        inv.Clear();
        foreach (ItemInfo itemInfo in itemList.Items)
        {
            Item item = Item.MakeItem(itemInfo);
            if ((InventoryType)item.InventoryType == InventoryType.Equipment)
            {
                equip.PlaceItem(item.SlotIndex, item);
            }
            else
            {
                inv.PlaceItem(item.SlotIndex, item);
            }
        }
    }
    
    public static void S_AddItemHandler(PacketSession session, IMessage packet)
    {
        S_AddItem itemList = (S_AddItem) packet;

        MyPlayerController player = ObjectManager.Instance.MyPlayer;
        foreach (var itemInfo in itemList.Items)
        {
            Item item = Item.MakeItem(itemInfo);
            player.Inven.Slots[item.SlotIndex].UpdateSlot(item, item.Amount);
            Debug.Log($"아이템을 획득했습니다. {item.Info.TemplateId}, {item.SlotIndex}");
        }
    }
    
    public static void S_SwapSlotHandler(PacketSession session, IMessage packet)
    {
        S_SwapSlot swapSlot = (S_SwapSlot) packet;

        MyPlayerController player = ObjectManager.Instance.MyPlayer;
        //메모리에 아이템 정보 적용
        Inventory invA = player.GetInventoryOfType((InventoryType)swapSlot.ItemA.InventoryType);
        Inventory invB = player.GetInventoryOfType((InventoryType)swapSlot.ItemB.InventoryType);

        invA.PlaceItem(swapSlot.ItemA.SlotIndex, Item.MakeItem(swapSlot.ItemA));
        invB.PlaceItem(swapSlot.ItemB.SlotIndex,Item.MakeItem(swapSlot.ItemB));
    }
    
    public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
    {
        S_ChangeStat changeStat = (S_ChangeStat) packet;
        MyPlayerController player = ObjectManager.Instance.MyPlayer;
    }

    public static void S_PingHandler(PacketSession session, IMessage packet)
    {
        C_Pong pongPacket = new C_Pong();
        Debug.Log("[server] PingCheck");
        NetworkManager.Instance.Send(pongPacket);
    }

    public static void S_QuestListHandler(PacketSession session, IMessage packet)
    {
        S_QuestList questPacket = (S_QuestList) packet;
        QuestManager.Instance.Init(questPacket);
    }
    
    public static void S_QuestStatusHandler(PacketSession session, IMessage packet)
    {
        S_QuestStatus quest = ( S_QuestStatus) packet;
        if (quest.QuestInfo == null)
            return;
        QuestManager.Instance.RecvQuestPacket(quest.QuestInfo);
    }
    
    public static void S_ChangeStateHandler(PacketSession session, IMessage packet)
    {
        S_ChangeState parameter = (S_ChangeState) packet;
    }
    
    public static void S_AnimParameterHandler(PacketSession session, IMessage packet)
    {
        S_AnimParameter parameter = (S_AnimParameter) packet;
        GameObject go = ObjectManager.Instance.FindById(parameter.ObjectId);
        if (go == null)
            return;
        if (go == ObjectManager.Instance.MyPlayer.gameObject)
            return;
        CreatureController cc = go.GetComponent<CreatureController>();
        cc.ReceiveAnimParameter(parameter);
    }
    
    public static void S_ShopTradeHandler(PacketSession session, IMessage packet)
    {
        S_ShopTrade parameter = (S_ShopTrade) packet;
        
    }

    public static void S_ErrorHandler(PacketSession session, IMessage packet)
    {
        S_Error error = (S_Error) packet;
        Debug.LogError($"Server ErrorCode = {error.ErrorCode}");
        //todo : error Popup
    }
} 