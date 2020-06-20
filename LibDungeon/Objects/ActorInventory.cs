using System;
using System.Collections.Generic;
using System.Text;

namespace LibDungeon.Objects
{
    public  abstract partial class Actor
    {
        public LinkedList<BaseItem> Equipment { get; set; } = new LinkedList<BaseItem>();
        public LinkedList<BaseItem> Inventory { get; set; } = new LinkedList<BaseItem>();

        public bool UseItem(BaseItem item)
        {
            if (!Inventory.Contains(item))
                return false;
            item.Use(this);
            Inventory.Remove(item);
            if (!(item.OneTimeUse || item.RemoveOnPickup))
                Equipment.AddLast(item);
            return true;
        }

        public bool UnuseItem(BaseItem item)
        {
            if (!Equipment.Contains(item))
                return false;
            item.Unuse(this);
            Equipment.Remove(item);
            Inventory.AddLast(item);
            return true;
        }

        public void AddItem(BaseItem item, bool equip = false)
        {
            if (Inventory.Contains(item))
                return;
            Inventory.AddLast(item);
            if (equip)
                UseItem(item);
        }

        public void AddItem(IEnumerable<BaseItem> items, bool equip = false)
        {
            foreach (var item in items)
                AddItem(item, true);
        }
    }
}
