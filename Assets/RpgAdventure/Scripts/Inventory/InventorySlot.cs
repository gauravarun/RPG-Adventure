using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgAdventure
{
    [System.Serializable]
    public class InventorySlot
    {
        public int index;
        public string itemName;
        public GameObject itemPrefab;

        public InventorySlot(int index)
        {
            this.index = index;
        }

        public void Place(GameObject item)
        {
            itemName = item.name;
            itemPrefab = item;
        }
    }
}

