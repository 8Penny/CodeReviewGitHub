using System;
using System.Collections.Generic;
using System.Linq;
using Services;
using Services.Boosts;
using Services.Sounds;
using Services.Talents;
using Services.Tasks;
using Services.Tutorial;
using TMPro;
using UI.Presenters.Upgrades;
using UnityEngine;
using Zenject;

namespace UIBasics.Views
{
    public class CastlePanelView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _nameLabel;
        [SerializeField]
        private List<PlanetStatRowView> _stats;
        [SerializeField]
        private List<GameObject> _dividers;
        [SerializeField]
        private List<MultiplierView> _castleMultipliers;
        [SerializeField]
        private List<MultiplierView> _catMultipliers;
        [SerializeField]
        private List<MultiplierView> _backpackMultipliers;

        [SerializeField]
        private List<UpgradeView> _upgradeViews;

        private Dictionary<CastleParameterType, UpgradePresenter> _upgradePresenters;
        private Castle _castle;
        
        private PlayerResourcesService _playerResourcesService;
        private TalentsService _talentsService;
        private BoostService _boostService;
        private TaskService _taskService;

        public Action<int> OnPanelUpdated;

        [Inject]
        public void Init(PlayerResourcesService playerResourcesService,
            TalentsService talentsService, BoostService boostService, TutorialTaskService tutorialTaskService,
            TaskService taskService, TutorialService tutorialService, SoundService soundService, UIService uiService)
        {
            _playerResourcesService = playerResourcesService;
            _talentsService = talentsService;
            _boostService = boostService;
            
            _upgradePresenters = new Dictionary<CastleParameterType, UpgradePresenter>();
            _upgradePresenters.Add(CastleParameterType.MiningRate,
                new UpgradePresenter(_playerResourcesService, CastleParameterType.MiningRate, _upgradeViews[0],
                    _talentsService, boostService, taskService, tutorialService, soundService, uiService, tutorialTaskService));
            _upgradePresenters.Add(CastleParameterType.CatSpeed,
                new UpgradePresenter(_playerResourcesService, CastleParameterType.CatSpeed, _upgradeViews[1],
                    _talentsService, boostService, taskService, tutorialService, soundService, uiService, tutorialTaskService));
            _upgradePresenters.Add(CastleParameterType.Capacity,
                new UpgradePresenter(_playerResourcesService, CastleParameterType.Capacity, _upgradeViews[2],
                    _talentsService, boostService, taskService, tutorialService, soundService, uiService, tutorialTaskService));
            
            
            _upgradeViews[0].SetPresenter(_upgradePresenters[CastleParameterType.MiningRate]);
            _upgradeViews[1].SetPresenter(_upgradePresenters[CastleParameterType.CatSpeed]);
            _upgradeViews[2].SetPresenter(_upgradePresenters[CastleParameterType.Capacity]);
            
        }

        public void SetValue(Castle castle)
        {
            _castle = castle;
            foreach (var presenter in _upgradePresenters)
            {
                presenter.Value.SetCastle(castle);
            }
            UpdateConstantPlanetValues();
            UpdateDynamicPlanetValues();

            _nameLabel.text = $"{_castle.Settings.Name} Castle";
            OnPanelUpdated?.Invoke(_castle.Id);
        }

        private void UpdateConstantPlanetValues()
        {
            int percents = 100;
            float weightSum = _castle.Settings.Resources.Sum(t => t.Weight);
            for (int j = 0; j <_castle.Settings.Resources.Count; j++)
            {
                var resource = _castle.Settings.Resources[j];
                int currentPercent = (int)Math.Floor((resource.Weight / weightSum) * 100);
                if (j == _castle.Settings.Resources.Count - 1)
                {
                    currentPercent = percents;
                }

                percents -= currentPercent;
                _stats[j].SetConstantFields(resource.ResourceId,  currentPercent);
                _stats[j].gameObject.SetActive(true);

                if (j > 0)
                {
                    _dividers[j-1].SetActive(true);
                }
            }

            for (int i = _castle.Settings.Resources.Count; i < 3; i++)
            {
                _stats[i].gameObject.SetActive(false);
                
                if (i > 0)
                {
                    _dividers[i-1].SetActive(false);
                }
            }
        }
        
        private void UpdateDynamicPlanetValues()
        {
            for (int i = 0; i < 3; i++)
            {
                var currentStat = _stats[i];
                var countValue = _castle.ResourcesHolder.Get(currentStat.ResourceId);
                var rateValue = CastleUtils.GetMiningCount(1, _castle.MineSpeedLevel,
                    _talentsService.MiningRateMultiplier * _boostService.MiningBoost * _boostService.GetPlanetBoost(_castle.Id));
                currentStat.UpdateParameters(rateValue, countValue);
            }
        }

        public void UpdateMultipliers()
        {
            _castleMultipliers[MultiplierIds.CastleBoost].UpdateVisibility(_boostService.IsActive(BoostType.CastleBoost) && _boostService.BoostEntityId == _castle.Id);
            _castleMultipliers[MultiplierIds.CastleBoost].SetValue(_boostService.GetPlanetBoost(_castle.Id));
            _castleMultipliers[MultiplierIds.Ability].UpdateVisibility(_talentsService.MiningRateMultiplier > 1.1f);
            _castleMultipliers[MultiplierIds.Ability].SetValue(_talentsService.MiningRateMultiplier);
            _castleMultipliers[MultiplierIds.Boost].UpdateVisibility(_boostService.IsActive(BoostType.MiningSpeedBoost));
            _castleMultipliers[MultiplierIds.Boost].SetValue(_boostService.MiningBoost);
            
            _catMultipliers[MultiplierIds.CastleBoost].UpdateVisibility(_boostService.IsActive(BoostType.CastleBoost) && _boostService.BoostEntityId == _castle.Id);
            _catMultipliers[MultiplierIds.CastleBoost].SetValue(_boostService.GetPlanetBoost(_castle.Id));
            _catMultipliers[MultiplierIds.Ability].UpdateVisibility(_talentsService.CatSpeedMultiplier > 1.1f);
            _catMultipliers[MultiplierIds.Ability].SetValue(_talentsService.CatSpeedMultiplier);
            
            _backpackMultipliers[MultiplierIds.CastleBoost].UpdateVisibility(_boostService.IsActive(BoostType.CastleBoost) && _boostService.BoostEntityId == _castle.Id);
            _backpackMultipliers[MultiplierIds.CastleBoost].SetValue(_boostService.GetPlanetBoost(_castle.Id));
            _backpackMultipliers[MultiplierIds.Ability].UpdateVisibility(_talentsService.BackpackMultiplier > 1.1f);
            _backpackMultipliers[MultiplierIds.Ability].SetValue(_talentsService.BackpackMultiplier);
        }

        public void Update()
        {
            if (_castle == null)
            {
                return;
            }
            UpdateDynamicPlanetValues();
            UpdateMultipliers();
        }
    }
}