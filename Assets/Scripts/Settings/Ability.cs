using System;
using System.Collections.Generic;
using Services;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class Ability
    {
        public AbilityType Id;
        public bool Available;
        public string Name;
        public string Description;
        public Sprite Sprite;
        public List<ResourceDemand> Price;
        public List<AbilityType> ToAbilities;
    }
}