using System;
using System.Collections.Generic;
using System.Text;

namespace Kame.Game.Data
{
    public enum ItemType : int
    {
        None = -1,
        Normal = 0,
        Money = 1,
        Exp = 2,
        Consumable = 1000000,
        Material = 2000000,
        Helmet = 3000000,
        Chest = 4000000,
        Shoulders = 5000000,
        Gloves = 6000000,
        Pants = 7000000,
        Boots = 8000000,
        RightWeapon = 9000000,
        LeftWeapon = 10000000,
        Extras = 11000000,
    }

    public enum ItemGrade
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
    }
}
