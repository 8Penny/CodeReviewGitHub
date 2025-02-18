using System;
using Sirenix.OdinInspector;
using Static;
using UnityEngine;
using Zenject;

namespace Settings
{
    [Serializable]
    public class TaskSettings
    {
        public int Id;
        public TaskType TaskType; 
        public string Name;
        public string Description;
        [Space]
        [ShowIf("@SettingsHelper.ShowAbility(TaskType)")]
        public AbilityType Ability;
        [HideIf("@SettingsHelper.ShowAbility(TaskType)")]
        public ResourceNames ResourceId;
        [HideIf("@SettingsHelper.ShowAbility(TaskType)")]
        public int Count;
        
        [Space]
        public int RewardCount;
        public bool IsHardReward;
        [NonSerialized]
        public int BranchId;
    }
}