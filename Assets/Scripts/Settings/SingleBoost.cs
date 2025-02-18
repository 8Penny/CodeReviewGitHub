using System;
using Services.Boosts;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class SingleBoost
    {
        public BoostType BoostType;
        public Sprite Sprite;
        public string Name;
        public string Description;
        public int Price;
        public int AdPrice;
    }
}