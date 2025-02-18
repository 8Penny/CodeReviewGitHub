using System;
using System.Collections.Generic;
using Services;
using Static;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class RecipeSettings
    {
        public int Id;
        public ResourceNames RecipeResourceId;
        public int BenchId;
        public int Weight;
        public int Time = 5;
        public List<ResourceDemand> Demands;
        public int RecipePrice = 5;

        private List<ResourceDemand> _demands = new List<ResourceDemand>();
        private float _multiplier;

        public List<ResourceDemand> GetBoostedDemands(float demandsMultiplier)
        {
            if (Mathf.Abs(demandsMultiplier - _multiplier) < 0.1f)
            {
                return _demands;
            }

            _multiplier = demandsMultiplier;
            _demands.Clear();
            foreach (var demand in Demands)
            {
                _demands.Add(new ResourceDemand(demand.ResourceId, (int)(demand.Value * demandsMultiplier)));
            }

            return _demands;
        }
    }
}