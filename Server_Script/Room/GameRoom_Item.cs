using Google.Protobuf;
using Google.Protobuf.Protocol;
using InflearnServer.DB;
using InflearnServer.Game.Data;
using InflearnServer.Game.Job;
using System;
using System.Collections.Generic;


namespace InflearnServer.Game
{
    public partial class GameRoom : JobSerializer
    {
        public void HandleChangeSlot(Player player, C_SwapSlot swapSlotPacket, GameRoom room)
        {
            if (player == null || room == null)
                return;
            ItemInfo ItemA = swapSlotPacket.ItemA;
            ItemInfo ItemB = swapSlotPacket.ItemB;
            
            //todo : slot검사해서 똑같은 아이템인지 검사(다른데서 Add Item하면서 갑자기 바뀔수도)
            // DB에 Noti
            DBTransaction.ChangeSlot(player, ItemA, ItemB, room);
        }

        public void HandleQuest(Player player, C_QuestStatus questPacket)
        {
            if (questPacket.Status == QuestStatus.QuestAccepted)
            {
                player.Quest.AcceptQuest(questPacket.QuestId);
            }
            else if (questPacket.Status == QuestStatus.QuestNone)
            {
                player.Quest.QuitQuest(questPacket.QuestId);
            }
            else if (questPacket.Status == QuestStatus.QuestRewarded)
            {
                player.Quest.GiveQuestReward(questPacket.QuestId);
            }
        }

        
    }

    
}
