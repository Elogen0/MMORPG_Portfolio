using Google.Protobuf.Protocol;
using InflearnServer.DB;
using InflearnServer.Game.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace InflearnServer.Game
{
    public class Player : GameObject
    {
        public int PlayerDbId { get; set; }
        public ClientSession Session { get; set; }
        public VisionCube Vision { get; private set; }

        public Inventory Inven { get; set; } = new Inventory(20);
        public Inventory Equip { get; set; } = new Inventory(6);
        public Quest Quest { get; set; }

        public float attackRange = 2.5f;
        public Player()
        {
            ObjectType = GameObjectType.Player;
            Vision = new VisionCube(this);

            Inven.type = InventoryType.Inventory;
            Equip.type = InventoryType.Equipment;

            Equip.RegisterPreUpdate(RemoveStat);
            Equip.RegisterPostUpdate(AddStat);

            Quest = new Quest(this);
        }

        public override void OnDamaged(GameObject attacker, float damage)
        {
            base.OnDamaged(attacker, damage);
        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);
        }

        public void OnLeaveGame()
        {
            //DB연동
            // 1) 서버 다운되면 아직 저장되지 않은 정보 날아감
            // 2) 코드 흐름을 다 막아버린다!! -> DB접근한다고 같은 Room의 다른 플레이어 시간을 잡아먹음
            // 해결?
            // - 비동기(Async) 방법 사용?
            // - 다른 쓰레드로 DB 일감을 던져버리면?
            // -- 결과를 받아서 처리하는 부분이 어려움
            // -- 아이템 생성할때 : DB처리가 먼저되고 그다음에 메모리 처리되는게 맞음
            DBTransaction.SavePlayerStatus_AllInOne(this, Room);
        }

        public Inventory GetInventoryOfType(InventoryType type)
        {
            if (type == InventoryType.Equipment)
                return Equip;
            if (type == InventoryType.Inventory)
                return Inven;
            return null;
        }

        private void AddStat(InventorySlot slot)
        {
            List<ItemBuff> buffs = slot.Item.Data.buffs;
            foreach (var buff in buffs)
            {
                Stat.AddModifier(buff.statType, buff);
            }
        }

        private void RemoveStat(InventorySlot slot)
        {
            List<ItemBuff> buffs = slot.Item.Data.buffs;
            foreach (var buff in buffs)
            {
                Stat.RemoveModifier(buff.statType, buff);
            }
        }
    }
}
