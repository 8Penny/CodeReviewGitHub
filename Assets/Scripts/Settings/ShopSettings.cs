using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "ShopSettings", menuName = "Configs/ShopSettings")]
    public class ShopSettings : ScriptableObject
    {
        [SerializeField]
        public List<ShopSlotSettings> _shopSlots;

        public Dictionary<string, ShopSlotSettings> Slots = new Dictionary<string, ShopSlotSettings>();

        public void Init()
        {
            foreach (var slot in _shopSlots)
            {
                Slots[slot.Id] = slot;
            }
        }
    }

    [Serializable]
    public class ShopSlotSettings
    {
        public string Id;
        public Sprite Icon;
    }
}