using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InflearnServer.DB
{
    [Table("Account")]
    public class AccountDb
    {
        public int AccountDbId { get; set; }
        public string AccountName { get; set; }
        public ICollection<PlayerDb> Players { get; set; }
    }
    
    [Table("Player")]
    public class PlayerDb
    {
        public int PlayerDbId { get; set; }
        public string PlayerName { get; set; }

        [ForeignKey("Account")]
        public int AccoundDbId { get; set; }
        public AccountDb Account { get; set; }

        public ICollection<ItemDb> Items { get; set; }
        public ICollection<QuestDb> Quests { get; set; }
        public int CharacterId { get; set; }
        public int Level { get; set; }
        public float Hp { get; set; }
        public string Exp { get; set; }
    }

    public class ItemDb
    {
        public int ItemDbId { get; set; }
        public int TemplateId { get; set; } = -1;
        public int Amount { get; set; } = 0;
        public int SlotIndex { get; set; } = -1;
        public int InventoryType { get; set; } = -1;
        [ForeignKey("Owner")]
        public int? OwnerDbId { get; set; }
        public PlayerDb Owner { get; set; }
    }

    public class QuestDb
    {
        public int QuestDbId { get; set; }
        public int TemplateId { get; set; }
        public QuestStatus Status { get; set; } = QuestStatus.QuestNone;
        public int Progress { get; set; }
        public DateTime AccecptedDate { get; set; }

        [ForeignKey("Owner")]
        public int? OwnerDbId { get; set; }
        public PlayerDb Owner { get; set; }
    }
}
