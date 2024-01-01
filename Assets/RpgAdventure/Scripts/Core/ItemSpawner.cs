using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace RpgAdventure
{
    public class ItemSpawner : MonoBehaviour
    {
        public GameObject itemPrefab;
        public LayerMask targetLayer;
        public UnityEvent<ItemSpawner> onItemPickup; 
        void Awake()
        {
            Instantiate(itemPrefab, transform);
            Destroy(transform.GetChild(0).gameObject);

            onItemPickup.AddListener(FindObjectOfType<InventoryManager>().OnItemPickup);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (0 != (targetLayer.value & 1 << other.gameObject.layer))
            {
                onItemPickup.Invoke(this);
            }
        }

    }
}

