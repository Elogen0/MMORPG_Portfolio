using Google.Protobuf.Protocol;
using InflearnServer.Game;
using InflearnServer.Game.Data;
using InflearnServer.Game.Job;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InflearnServer.DB
{
    //DB에 던지고 응답요청이 필요없다
    public partial class DBTransaction : JobSerializer
    {
        //public static void UpdateSlot(Player player, Item item, GameRoom room)
        //{
        //    if (player == null || room == null)
        //        return;

        //    Instance.Push(() =>
        //    {
        //        using (AppDbContext db = new AppDbContext())
        //        {
        //            ItemDb dbItem = db.Items.Single(i => i.ItemDbId == item.ItemDbId);
        //            if (dbItem != null)
        //            {
        //                dbItem.InventoryType = item.InventoryType;
        //                dbItem.Amount = item.Amount;
        //                dbItem.
        //                dbItem.SlotIndex = item.SlotIndex;
        //            }

        //            bool success = db.SaveChangesEx();
        //            if (success)
        //            {
        //                room.Push(() =>
        //                {
        //                    Item newItemA = Item.MakeItem(dbItem);
        //                    player.GetInventoryOfType(dbItem.InventoryType).slots[dbItem.SlotIndex].
        //                    UpdateSlot(newItemA, newItemA.Amount);


        //                    //Client Noti
        //                    {
        //                        S_SwapSlot swapSlot = new S_SwapSlot();
        //                        swapSlot.ItemA = newItemA.Info;
        //                        swapSlot.ItemB = newItemB.Info;
        //                        player.Session.Send(swapSlot);
        //                    }
        //                });
        //            }
        //        }
        //    });
        //}

        
    }
}
