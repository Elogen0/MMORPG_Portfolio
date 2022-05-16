using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kame.Game.Data
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

		public bool IsEquipable => (ItemType >= ItemType.Helmet && ItemType < ItemType.Extras);
		public ItemType ItemType => Data.type;
		public bool Stackable => Data.stackable;
		public ItemData Data { get; protected set; } = ItemData.NullItemData;

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

		public Item(int templateId, int amount)
		{
			Clear(false);
			DataManager.ItemDict.TryGetValue(templateId, out ItemData itemData);
			Data = itemData;
			TemplateId = itemData?.id ?? -1;
			Amount = amount;
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

		public static Item MakeItem(ItemInfo itemInfo)
		{
			if (!DataManager.ItemDict.TryGetValue(itemInfo.TemplateId, out ItemData itemData))
				return null;
			
			Item item		= new Item(itemData);
			item.ItemDbId	= itemInfo.ItemDbId;
			item.TemplateId = itemInfo.TemplateId;
			item.Amount		= itemInfo.Amount;
			item.SlotIndex	= itemInfo.SlotIndex;
			item.InventoryType = itemInfo.InventoryType;
			
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
