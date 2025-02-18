using System.Collections.Generic;
using Static;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "MainGameConfig", menuName = "Configs/MainGameConfig")]
    public class MainGameSettings : ScriptableObject
    {
        public float FlowersCooldown;
        public float CartCooldown;
        public List<ResourceNames> CartRewards;
        public int MinFlowerRewardsCount;
        public int MaxFlowerRewardsCount;
        public int MinCartRewardsCount;
        public int MaxCartRewardsCount;
    }    
}
