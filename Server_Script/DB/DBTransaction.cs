using Google.Protobuf.Protocol;
using InflearnServer.Game;
using InflearnServer.Game.Data;
using InflearnServer.Game.Job;
using InflearnServer.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace InflearnServer.DB
{
    public partial class DBTransaction : JobSerializer
    {
        public static DBTransaction Instance { get; } = new DBTransaction();

        // Me(GameRoom) -> You(Db) -> Me(GameRoom)
        public static void SavePlayerStatus_AllInOne(Player player, GameRoom room)
        {
            if (player == null || room == null)
                return;

            //Me (GameRoom)
            PlayerDb playerDb = new PlayerDb();
            playerDb.PlayerDbId = player.PlayerDbId;
            playerDb.Hp = player.Stat.Hp;

            //You 
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Entry(playerDb).State = EntityState.Unchanged;
                    db.Entry(playerDb).Property(nameof(PlayerDb.Hp)).IsModified = true;
                    if (db.SaveChangesEx())
                    {
                        //Me
                        room.Push(() =>
                        {
                            //Console.WriteLine($"Hp Saved({playerDb.Hp})");
                        });
                    }
                    else
                    {
                        player.Session.SendErrorCode(ServerErrorCode.DB_TRANSACTION_FAILED);
                    }
                }
            });
        }

        public static void RewardPlayer(Player player, RewardData rewardData, GameRoom room)
        {
            if (player == null || rewardData == null || room == null)
                return;

            //todo : 살짝 문제가 있긴 하다... 실제로 AddItem 하기전에 요청이 다시온다면?
            //1) DB에 저장요청
            //2) DB저장 OK
            //3) 메모리에 적용
            //==> todo : 예약상태로 
            Console.WriteLine($"Reward : {rewardData.itemId}/{rewardData.amount}");
            Item item = new Item(rewardData.itemId);
            item.Amount = rewardData.amount;
            InventorySlot slot = player.Inven.GetSuitableSlotAndLock(item);
            if (slot == null)
                return;
            ItemDb itemDb = null;
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    if (item.Stackable)
                    {
                        itemDb = db.Items.FirstOrDefault(i => i.TemplateId == rewardData.itemId);
                        if (itemDb == null)
                        {
                            itemDb = new ItemDb()
                            {
                                TemplateId = rewardData.itemId,
                                Amount = rewardData.amount,
                                SlotIndex = slot.Index,
                                OwnerDbId = player.PlayerDbId,
                                InventoryType = (int)InventoryType.Inventory
                            };
                            db.Items.Add(itemDb);
                        }
                        else
                        {
                            itemDb.Amount += rewardData.amount;
                        }
                    }
                    else
                    {
                        itemDb = new ItemDb()
                        {
                            TemplateId = rewardData.itemId,
                            Amount = rewardData.amount,
                            SlotIndex = slot.Index,
                            OwnerDbId = player.PlayerDbId,
                            InventoryType = (int)InventoryType.Inventory
                        };
                        db.Items.Add(itemDb);
                    }

                    if (db.SaveChangesEx())
                    {
                        room.Push(() =>
                        {
                            Item newItem = Item.MakeItem(itemDb);
                            InventorySlot slot = player.Inven.Slots[newItem.SlotIndex];
                            
                            slot.UpdateSlot(newItem, newItem.Amount);
                            slot.Release();

                            //Client Noti
                            {
                                S_AddItem itemPacket = new S_AddItem();
                                ItemInfo itemInfo = new ItemInfo();
                                itemInfo.MergeFrom(newItem.Info);
                                itemPacket.Items.Add(itemInfo);
                                player.Session.Send(itemPacket);
                            }
                        });
                    }
                    else
                    {
                        player.Session.SendErrorCode(ServerErrorCode.DB_TRANSACTION_FAILED);
                        room.Push(() =>
                        {
                            slot.Release();
                        });
                        
                    }
                }
            });
        }

        public static void ChangeSlot(Player player, ItemInfo itemA, ItemInfo itemB, GameRoom room)
        {
            if (player == null || room == null)
                return;

            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    ItemDb dbA = db.Items.SingleOrDefault(i => i.ItemDbId == itemA.ItemDbId);
                    if (dbA != null)
                    {
                        dbA.InventoryType = itemA.InventoryType;
                        dbA.SlotIndex = itemA.SlotIndex;
                    }

                    ItemDb dbB = db.Items.SingleOrDefault(i => i.ItemDbId == itemB.ItemDbId);
                    if (dbB != null)
                    {
                        dbB.InventoryType = itemB.InventoryType;
                        dbB.SlotIndex = itemB.SlotIndex;
                    }

                    bool success = db.SaveChangesEx();
                    if (success)
                    {
                        room.Push(() =>
                        {
                            Item newItemA = Item.MakeItem(dbA);
                            Inventory inven = player.GetInventoryOfType((InventoryType)itemA.InventoryType);
                            if (inven != null)
                            {
                                inven.PlaceItem(itemA.SlotIndex, itemA.TemplateId < 0 || dbA == null ? null : Item.MakeItem(dbA));
                            }

                            inven = player.GetInventoryOfType((InventoryType)itemB.InventoryType);
                            if (inven != null)
                            {
                                inven.PlaceItem(itemB.SlotIndex, itemB.TemplateId < 0  || dbB == null ? null : Item.MakeItem(dbB)); 
                            }

                            //Client Noti
                            {
                                S_SwapSlot swapSlot = new S_SwapSlot();
                                swapSlot.ItemA = itemA;
                                swapSlot.ItemB = itemB;
                                player.Session.Send(swapSlot);
                                //todo : 다른사람에게도 BroadCast => 장착아이템이 바뀔때..
                            }
                        });
                    }
                    else
                    {
                        player.Session.SendErrorCode(ServerErrorCode.DB_TRANSACTION_FAILED);
                    }
                }
            });
        }

        public static void AcceptQuest(Player player, QuestInfo questInfo, GameRoom room)
        {

        }

        public static void GiveQuestReward(Player player,QuestRecord record , GameRoom room)
        {
            if (player == null || record == null || room == null)
                return;

            List<Item> rewardItems = new List<Item>();
            foreach (var reward in record.Rewards)
            {
                Item item = new Item(reward.itemId);
                item.Amount = reward.amount;
                rewardItems.Add(item);
            }

            if (!player.Inven.HasSpaceFor(rewardItems))
            {
                Console.WriteLine($"HasNot spceFor Rewards {record.Id}");
                player.Session.SendErrorCode(ServerErrorCode.NOT_SUFFICIENT_INVENTORY);
                return;
            }
            
            List<ItemDb> itemDbs = new List<ItemDb>();
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    QuestDb questDb = db.Quests.SingleOrDefault(q => q.TemplateId == record.Id);
                    if (questDb == null || questDb.Status != QuestStatus.QuestCompleted)
                        return;
                    questDb.Status = QuestStatus.QuestRewarded;

                    foreach (var rewardItem in rewardItems)
                    {
                        ItemDb itemDb = null;
                        if (rewardItem.Stackable)
                        {
                            itemDb = db.Items.FirstOrDefault(i => i.TemplateId == rewardItem.TemplateId);
                            if (itemDb == null)
                            {
                                InventorySlot slot = player.Inven.GetEmptySlotAndLock();

                                itemDb = new ItemDb
                                {
                                    TemplateId = rewardItem.TemplateId,
                                    Amount = rewardItem.Amount,
                                    SlotIndex = slot.Index,
                                    OwnerDbId = player.PlayerDbId,
                                    InventoryType = (int)InventoryType.Inventory
                                };
                                db.Items.Add(itemDb);
                            }
                            else
                            {
                                itemDb.Amount += rewardItem.Amount;
                            }
                        }
                        else
                        {
                            InventorySlot slot = player.Inven.GetEmptySlotAndLock();
                            itemDb = new ItemDb
                            {
                                TemplateId = rewardItem.TemplateId,
                                Amount = rewardItem.Amount,
                                SlotIndex = slot.Index,
                                OwnerDbId = player.PlayerDbId,
                                InventoryType = (int)InventoryType.Inventory
                            };
                            db.Items.Add(itemDb);
                        }
                        itemDbs.Add(itemDb);
                    }

                    if (db.SaveChangesEx())
                    {
                        room.Push(() =>
                        {
                            S_AddItem itemPacket = new S_AddItem();
                            foreach (var itemDb in itemDbs)
                            {
                                if (itemDb == null)
                                    continue;
                                Item newItem = Item.MakeItem(itemDb);
                                InventorySlot slot = player.Inven.Slots[newItem.SlotIndex];

                                slot.UpdateSlot(newItem, newItem.Amount);
                                //Client Noti
                                {
                                    ItemInfo itemInfo = new ItemInfo();
                                    itemInfo.MergeFrom(newItem.Info);
                                    itemPacket.Items.Add(itemInfo);
                                }
                                slot.Release();

                            }
                            player.Session.Send(itemPacket);
                            player.Quest.ToRewared(record);
                        });
                    }
                    else
                    {
                        room.Push(() =>
                        {
                            foreach (var item in itemDbs)
                            {
                                if (item != null)
                                    player.Inven.Slots[item.SlotIndex].Release();
                            }
                        });
                        player.Session.SendErrorCode(ServerErrorCode.DB_TRANSACTION_FAILED);
                    }
                }
            });
        }

        public static void AcceptQuest(Player player, int questId, GameRoom room)
        {
            if (player == null || room == null)
                return;
            var questData = DataManager.QuestDict[questId];
            if (questData == null)
                return;
            QuestDb questDb = new QuestDb()
            {
                TemplateId = questData.ID,
                Status = QuestStatus.QuestAccepted,
                Progress = 0,
                AccecptedDate = DateTime.UtcNow,
                OwnerDbId = player.PlayerDbId,
            };

            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Add(questDb);
                    var success = db.SaveChangesEx();
                    if (success)
                    {
                        room.Push(() =>
                        {
                            var record = player.Quest.Add(questDb);
                            S_QuestStatus questPacket = new S_QuestStatus();
                            questPacket.QuestInfo = new QuestInfo();
                            questPacket.QuestInfo.MergeFrom(record.Info);
                            player.Session.Send(questPacket);
                        });
                    }
                    else
                    {
                        player.Session.SendErrorCode(ServerErrorCode.DB_TRANSACTION_FAILED);
                    }
                }
            });
        }

    }
}
