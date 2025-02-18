using System;
using System.Collections.Generic;
using System.Linq;
using Services;
using Static;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    [Serializable]
    public class PlanetSettings
    {
        public int Id;
        public string Name;
        public List<PlanetResource> Resources;
        public float PriceMultiplier;
        public int UnlockPrice;
        public float Distance;
        public bool AvailableOnStart;
        public AbilityType AbilityId = AbilityType.None;
        [NonSerialized]
        public List<PlanetResource> SortedResources;

        public void Init(SettingsService settingsService)
        {
            SortedResources = Resources.OrderByDescending(t => settingsService.GameResources[t.ResourceId].Weight).ToList();
        }
    }

    [Serializable]
    public class PlanetResource
    {
        public ResourceNames ResourceId;
        public int Weight;
    }
}