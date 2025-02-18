using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "BoostsSettings", menuName = "Configs/BoostsSettings")]
    public class BoostsSettings : ScriptableObject
    {
        public List<SingleBoost> Boosts;
    }
}