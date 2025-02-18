using System.Collections.Generic;
using Services;
using Static;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "ResourcesList", menuName = "Configs/AbilitiesTree")]
    public class AbilitiesTree : ScriptableObject
    {
        private Dictionary<AbilityType, Ability> _abilities;
        
        public List<Ability> WorkbenchBranch;
        public List<Ability> CraftTableBranch;
        public List<Ability> FlowerBranch;
        public List<Ability> CartBranch;
        public List<Ability> MiningBranch;
        public List<Ability> SpeedBranch;
        public List<Ability> BackpackBranch;
        public List<Ability> SpyglassBranch;
        public List<Ability> TimeBranch;
        public Dictionary<AbilityType, Ability> AbilitiesDict => _abilities;
        public void Init()
        {
            _abilities = new Dictionary<AbilityType, Ability>();
            
            Process(WorkbenchBranch);
            Process(CraftTableBranch);
            Process(FlowerBranch);
            Process(CartBranch);
            Process(MiningBranch);
            Process(SpeedBranch);
            Process(BackpackBranch);
            Process(SpyglassBranch);
            Process(TimeBranch);
        }

        private void Process(List<Ability> abilities)
        {
            
            foreach (var ability in abilities)
            {
                // if (StaticValues.IsDevelop)
                // {
                //     ability.Price = new List<ResourceDemand>() {new ResourceDemand(ResourceNames.Fireflies, 1)};
                // }
                
                _abilities.Add(ability.Id, ability);
            }
        }
    }
}