using Google.Protobuf.Protocol;
using InflearnServer.DB;
using InflearnServer.Game.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace InflearnServer.Game
{
	public class Item
	{
		public ItemInfo Info { get; } = new ItemInfo();

		public int ItemDbId
		{
			get { return Info.ItemDbId; }
			set { Info.ItemDbId = value; }
		}

		public int TemplateId
		{
			get { return Info.TemplateId; }
			set { Info.TemplateId = value; }
		}

		public int Amount
		{
			get { return Info.Amount; }
			set { Info.Amount = value; }
		}

		public int SlotIndex
		{
			get { return Info.SlotIndex; }
			set { Info.SlotIndex = value; }
		}

		public int InventoryType
		{
			get { return Info.InventoryType; }
			set { Info.InventoryType = value; }
		}
		public ItemData Data { get; protected set; } = ItemData.NullItemData;
		public ItemType ItemType => Data.type;
		public bool Stackable => Data.stackable;
		public bool IsEquippable => (ItemType >= ItemType.Helmet && ItemType < ItemType.Extras);

		public Item()
		{
			Clear(false);
		}

		public Item(ItemData itemData)
		{
			Clear(false);
			Data = itemData;
			TemplateId = itemData.id;
		}

		public Item(int templateId)
		{
			Clear(false);
			DataManager.ItemDict.TryGetValue(templateId, out ItemData itemData);
			Data = itemData;
			TemplateId = itemData != null ? itemData.id : -1;
		}

		public Item(Item other)
        {
			Copy(other);
        }

		public void Copy(Item other)
        {
			ItemDbId = other.ItemDbId;
			TemplateId = other.TemplateId;
			Amount = other.Amount;
			SlotIndex = other.SlotIndex;
			InventoryType = other.InventoryType;
			Data = other.Data;
        }

		public static Item MakeItem(ItemDb itemDb)
		{
			if (itemDb == null)
				return new Item();
			if (!DataManager.ItemDict.TryGetValue(itemDb.TemplateId, out ItemData itemData))
				return new Item();

			Item item = new Item(itemData);
			item.ItemDbId = itemDb.ItemDbId;
			item.TemplateId = itemDb.TemplateId;
			item.Amount = itemDb.Amount;
			item.SlotIndex = itemDb.SlotIndex;
			item.InventoryType = itemDb.InventoryType;

			return item;
		}

		public void Clear(bool clearData = true)
		{
			TemplateId = -1;
			ItemDbId = -1;
			Amount = 0;
			SlotIndex = -1;
			InventoryType = (int)global::InventoryType.None;
			if (clearData)
				Data = ItemData.NullItemData;
		}

		public static void Swap(Item itemA, Item itemB)
		{
			Item tempItem = new Item(itemA);
			itemA.Copy(itemB);
			itemB.Copy(tempItem);
		}
	}
}
