using System;
using System.Collections.Generic;
using System.Linq;
using Services.Sounds;
using Settings;

namespace Services.Talents
{
    public partial class TalentsService : IDisposable
    {
        private PlayerDataManager _dataManager;
        private PlayerResourcesService _resourcesService;
        private SettingsService _settingsService;
        private SoundService _soundService;
        
        public Action OnAbilitiesUpdated;
        public Action OnCurrenChanged;
        public Action<AbilityType> OnAddedAbility;
        
        public List<AbilityType> UnlockedAbilities => _dataManager.Activated.UnlockedAbilities;
        public Dictionary<AbilityType, Ability> Settings => _settingsService.AbilitiesTree.AbilitiesDict;
        public AbilityType CurrentTalent { get; private set; }

        public TalentsService(PlayerDataManager dataManager,
            PlayerResourcesService resourcesService,
            SettingsService settingsService,
            SoundService soundsService)
        {
            _soundService = soundsService;
            _dataManager = dataManager;
            _resourcesService = resourcesService;
            _settingsService = settingsService;
            
            _dataManager.OnSaveLoaded += UpdateParameters;
        }
        public void Dispose()
        {
            _dataManager.OnSaveLoaded -= UpdateParameters;
        }

        public bool HasTalent(AbilityType id)
        {
            return _dataManager.Activated?.UnlockedAbilities.Any(t => t == id) ?? false;
        }
        
        public bool CanBuy(AbilityType id)
        {
            var currentSettings = _settingsService.AbilitiesTree.AbilitiesDict[id];
            return _resourcesService.CanBuy(currentSettings.Price);
        }

        public bool TryBuy(AbilityType id)
        {
            var currentSettings = _settingsService.AbilitiesTree.AbilitiesDict[id];
            if (HasTalent(id))
            {
                return true;
            }
            bool isSuccess = _resourcesService.TryBuy(currentSettings.Price);
            if (isSuccess)
            {
                Add(id);
                OnAbilitiesUpdated?.Invoke();
                OnAddedAbility?.Invoke(id);
                _soundService.PlayAbilityResearch();
                UpdateParameters();
            }

            return isSuccess;
        }

        public void SetCurrentTalent(AbilityType abilityType)
        {
            CurrentTalent = abilityType;
            OnCurrenChanged.Invoke();
        }

        private void Add(AbilityType id)
        {
            _dataManager.Activated.UnlockedAbilities.Add(id);
        }
    }
}