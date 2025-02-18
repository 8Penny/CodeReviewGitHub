
using Services;
using Services.Boosts;
using Services.Sounds;
using Services.Talents;
using Services.Tasks;
using Services.Tutorial;
using Settings;
using Static;
using UIBasics.Views;
using UnityEngine;

namespace UI.Presenters.Upgrades
{
    public class UpgradePresenter
    {
        private readonly PlayerResourcesService _playerResourcesService;
        private readonly TalentsService _talentsService;
        private readonly BoostService _boostService;
        private readonly TaskService _taskService;
        private readonly TutorialService _tutorialService;
        private readonly SoundService _soundService;
        private readonly UIService _uiService;
        private TutorialTaskService _tutorialTaskService;
        
        private CastleParameterType _currentType;
        private Castle _castle;
        private UpgradeView _view;
        private bool _isObservingResourcesChange;

        
        public CastleParameterType CurrentType => _currentType;

        public UpgradePresenter(
            PlayerResourcesService playerResourcesService,
            CastleParameterType currentType,
            UpgradeView view,
            TalentsService talentsService,
            BoostService boostService,
            TaskService taskService,
            TutorialService tutorialService,
            SoundService soundService, UIService uiService,
            TutorialTaskService tutorialTaskService)
        {
            _talentsService = talentsService;
            _playerResourcesService = playerResourcesService;
            _boostService = boostService;
            _taskService = taskService;
            _tutorialService = tutorialService;
            _soundService = soundService;
            _uiService = uiService;
            _tutorialTaskService = tutorialTaskService;
            
            _currentType = currentType;
            _view = view;
        }

        public void SetCastle(Castle castle)
        {
            _castle = castle;
            UpdateUpgradeView();
            if (_isObservingResourcesChange)
            {
                return;
            }

            _isObservingResourcesChange = true;
            _playerResourcesService.OnResourcesUpdated += ResourcesUpdatedHandler;

            _tutorialTaskService.TaskCompleted += TaskCompletedHandler;
            _tutorialTaskService.TaskAdded += UpdateUpgradeView;
        }

        private void TaskCompletedHandler(TutorialTaskType t)
        {
            UpdateUpgradeView();
        }

        private void ResourcesUpdatedHandler(ResourceType rt)
        {
            if (rt is ResourceType.None)
            {
                UpdateUpgradeView();
            }
        }

        public void OnButtonClickedHandler()
        {
            if (!_tutorialService.CanUpgrade(_currentType))
            {
                return;
            }
            
            var nextLevel = GetCurrentLevel() + 1;
            var demand = new ResourceDemand(ResourceNames.Soft,
                CommonUtils.GetUpgradePrice(nextLevel, _castle.Settings.PriceMultiplier));
            if (_playerResourcesService.TryBuy(demand))
            {
                _soundService.PlayClick();
                _castle.SetLevel(_currentType, nextLevel);
                _taskService.OnCastleUpgrade(_currentType);

                _tutorialService.UpgradeCastle();
                UpdateUpgradeView();
                return;
            }
            _uiService.OpenGoToShopPopup();
            _soundService.PlayClick();
        }
        
        private void UpdateUpgradeView()
        {
            var currentLevel = GetCurrentLevel();
            var nextLevel = currentLevel + 1;
            int price = CommonUtils.GetUpgradePrice(nextLevel, _castle.Settings.PriceMultiplier);
            int currentPlayerCoinsAmount = Mathf.FloorToInt(_playerResourcesService.GetResource(ResourceNames.Soft));
            
            bool canBeUpgraded = _tutorialService.CanUpgrade(_currentType);
            _view.UpdateParameters(currentLevel, price, currentPlayerCoinsAmount >= price && canBeUpgraded);
            UpdateSpecialParameter(nextLevel, currentLevel);
        }

        private void UpdateSpecialParameter(int nextLevel, int currentLevel)
        {
            switch (_currentType)
            {
                case CastleParameterType.MiningRate:
                    float miningMultiplier = _talentsService.MiningRateMultiplier * _boostService.MiningBoost * _boostService.GetPlanetBoost(_castle.Id);
                    var nextValue = CastleUtils.GetMiningCount(1, nextLevel, miningMultiplier);
                    var value = CastleUtils.GetMiningCount(1, currentLevel, miningMultiplier);
                    _view.SetRate(value, nextValue);
                    break;
                case CastleParameterType.Capacity:
                    float capacityMultiplier = _talentsService.BackpackMultiplier* _boostService.GetPlanetBoost(_castle.Id);
                    var currentCapacity = Mathf.FloorToInt(CastleUtils.GetCapacity(currentLevel, capacityMultiplier));
                    var capacity = Mathf.FloorToInt(CastleUtils.GetCapacity(nextLevel, capacityMultiplier));
                    _view.SetCapacity(currentCapacity, capacity);
                    break;
                case CastleParameterType.CatSpeed:
                    float speedMultiplier = _talentsService.CatSpeedMultiplier* _boostService.GetPlanetBoost(_castle.Id);
                    var speed = CastleUtils.GetSpeed(currentLevel, speedMultiplier);
                    var nextSpeed = CastleUtils.GetSpeed(nextLevel, speedMultiplier);
                    _view.SetSpeed(speed, nextSpeed);
                    break;
            }
        }

        private int GetCurrentLevel()
        {
            switch (_currentType)
            {
                case CastleParameterType.MiningRate:
                    return _castle.MineSpeedLevel;
                case CastleParameterType.Capacity:
                    return _castle.CapacityLevel;
                case CastleParameterType.CatSpeed:
                    return _castle.CatSpeedLevel;
            }

            return 0;
        }
        
    }
}