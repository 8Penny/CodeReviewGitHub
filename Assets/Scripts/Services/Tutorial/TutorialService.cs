using System;
using System.Collections.Generic;
using DG.Tweening;
using Services.Talents;
using Services.Tasks;
using Settings;
using Static;
using UI.Presenters.Upgrades;
using UnityEngine;
using UnityEngine.UI;

namespace Services.Tutorial
{
    public class TutorialService : IDisposable
    {
        private PlayerDataManager _dataManager;
        private PlayerResourcesService _playerResourcesService;
        private SettingsService _settingsService;
        private UIService _uiService;
        private TalentsService _talentsService;
        private TutorialTaskService _tutorialTaskService;

        private Dictionary<int, int> _softReward;
        private TutorialSettings _config;
        public int TutorialStep => _dataManager?.Parameters?.LastTutorialStep ?? 0;
        //public bool IsFirstPartTutorialComplete => TutorialStep >= 4; //WAITING STEPS: 4, 7
        public bool IsComplete => TutorialStep >= (int) TutorialStepNames.ResearchAll;
        public TutorialSettings Config => _config;

        public Action OnTutorialStepUpdated;
        public Action OnTutorialHalfStep;
        public Action<int> OnNextTutorialStep;

        public TutorialService(PlayerDataManager dataManager,
            PlayerResourcesService playerResourcesService,
            SettingsService settingsService,
            UIService uiService,
            TalentsService talentsService,
            TutorialTaskService tutorialTaskService)
        {
            _uiService = uiService;
            _dataManager = dataManager;
            _playerResourcesService = playerResourcesService;
            _settingsService = settingsService;
            _talentsService = talentsService;
            _tutorialTaskService = tutorialTaskService;

            _softReward = new Dictionary<int, int>();
            _softReward[1] = 100;
            _softReward[2] = 100;
            _softReward[3] = 100;

            OnNextTutorialStep += NextTutorialStepHandler;
            _dataManager.OnSaveLoaded += UpdateParameters;
            _talentsService.OnAddedAbility += AbilityAddedHandler;
            _talentsService.OnCurrenChanged += CurrenChangedHandler;
            _tutorialTaskService.TaskCompleted += MoveNext;
            _uiService.OnPanelChanged += PanelChangedHandler;
            _uiService.OnViewsRegistered += ViewRegisterHandler;
        }

        public void Dispose()
        {
            _dataManager.OnSaveLoaded -= UpdateParameters;
            OnNextTutorialStep -= NextTutorialStepHandler;
            _talentsService.OnAddedAbility -= AbilityAddedHandler;
            _talentsService.OnCurrenChanged -= CurrenChangedHandler;
            _tutorialTaskService.TaskCompleted -= MoveNext;
            _uiService.OnPanelChanged -= PanelChangedHandler;
            _uiService.OnViewsRegistered -= ViewRegisterHandler;
        }

        private void ViewRegisterHandler()
        {
            if ((TutorialStepNames) TutorialStep is TutorialStepNames.CastleUpgrade
                or TutorialStepNames.ChosenCastleForBoost
                or TutorialStepNames.Ability4Shown)
            {
                MoveNext(TutorialTaskType.OpenWorkbench);
            }
            _uiService.Register(this);

            InitTutorial();
        }

        private void UpdateParameters()
        {
            _config = _settingsService.TutorialConfig;
            OnTutorialStepUpdated?.Invoke();
        }

        private void CurrenChangedHandler()
        {
            OnNextTutorialStep.Invoke(0);
        }

        private void PanelChangedHandler(PanelType type)
        {
            if ((TutorialStepNames) TutorialStep is TutorialStepNames.SellAndEarnTasks && type == PanelType.AbilityTree)
            {
                SetTutorialStep((int) TutorialStepNames.OpenedAbilityPanel);
            }
        }

        public void InitTutorial()
        {
            if (IsComplete)
            {
                return;
            }

            float interval = TutorialStep == (int) TutorialStepNames.CameraAppearing ? 2f : 0.5f;
            _uiService.Views.ScrollWorldComponent.FocusFirstCastle(interval, CompleteInitTutorial);
        }

        private void CompleteInitTutorial()
        {
            if (TutorialStep == (int) TutorialStepNames.CameraAppearing)
            {
                SetTutorialStep((int) TutorialStepNames.CameraToCastle);
            }
        }

        private void MoveNext(TutorialTaskType taskType)
        {
            if (_tutorialTaskService.HasTutorialTasks)
            {
                return;
            }

            _uiService.CloseWindow();


            _uiService.Views.GiftChestWindow.Show((TutorialStepNames) TutorialStep);
        }

        public void ContinueTutorial()
        {
            if ((TutorialStepNames) TutorialStep == TutorialStepNames.CastleUpgrade)
            {
                SetTutorialStep((int) TutorialStepNames.UpgradedAllCastleParametersTasks);
            }

            if ((TutorialStepNames) TutorialStep == TutorialStepNames.ChosenCastleForBoost)
            {
                SetTutorialStep((int) TutorialStepNames.SellAndEarnTasks);
            }

            if ((TutorialStepNames) TutorialStep == TutorialStepNames.Ability4Shown)
            {
                SetTutorialStep((int) TutorialStepNames.ResearchAll);
            }
        }

        private void AbilityAddedHandler(AbilityType abilityType)
        {
            if (abilityType == AbilityType.Workbench1)
            {
                _uiService.CloseWindow();
                SetTutorialStep((int) TutorialStepNames.OpenedWorkbench);
            }

            if (abilityType == AbilityType.CraftTable1)
            {
                _uiService.CloseWindow();
                SetTutorialStep((int) TutorialStepNames.OpenedCraftTable);
                
                _playerResourcesService.AddResource(ResourceNames.Soft, 2000);
            }
        }

        public void TryUpdateRecipeSetStep()
        {
            if (TutorialStep == (int) TutorialStepNames.OpenedWorkbench)
            {
                _dataManager.Parameters.LastTutorialStep = (int) TutorialStepNames.SetRecipe;
                OnNextTutorialStep(0);
            }
        }

        public bool CanUpgrade(CastleParameterType currentType)
        {
            if ((TutorialStepNames) TutorialStep is TutorialStepNames.CameraMan)
            {
                return currentType == CastleParameterType.MiningRate;
            }
            if ((TutorialStepNames) TutorialStep is TutorialStepNames.CastleUpgrade)
            {
                return _tutorialTaskService.CanUpgrade(currentType);
            }

            return true;
        }

        public void ClickCraftTable()
        {
            if (TutorialStep == (int) TutorialStepNames.OpenedCraftTable)
            {
                _dataManager.Parameters.LastTutorialStep = (int) TutorialStepNames.Ability4Shown;
                OnNextTutorialStep(0);
            }
        }

        public void UnlockFirstCastle()
        {
            SetTutorialStep((int) TutorialStepNames.FirstCastleUnlocked);
            //_uiService.Views.Camera.transform.parent.DOMove(new Vector3(-13.58f,3165.22f,28.94f), 1f);
        }

        public void UpgradeCastle()
        {
            if (TutorialStep == (int) TutorialStepNames.CameraMan)
            {
                _playerResourcesService.AddResource(ResourceNames.Fireflies, 5);
                SetTutorialStep((int) TutorialStepNames.CastleUpgrade);
                _tutorialTaskService.AddUpgradeTasks();
                
                _tutorialTaskService.Upgrade(CastleParameterType.MiningRate, true);
                //_playerResourcesService.AddResource(ResourceNames.Fireflies, 5);
            }
        }

        public void SellResources()
        {
            if (TutorialStep == (int) TutorialStepNames.UpgradedAllCastleParametersTasks)
            {
                SetTutorialStep((int) TutorialStepNames.ResourceSold);
                _tutorialTaskService.AddFirefliesTask();
                _playerResourcesService.AddResource(ResourceNames.Fireflies, 20);
                
                _tutorialTaskService.Sell(5, 0,true);
            }
        }

        public void CompleteClickedAbility()
        {
            if (TutorialStep == (int) TutorialStepNames.TapedOn4Ability)
            {
                SetTutorialStep((int) TutorialStepNames.Ability4Shown);
                _tutorialTaskService.AddResearchTasks();
            }
        }

        public void HalfStep()
        {
            if (Config.TutorialSteps.TryGetValue((TutorialStepNames) TutorialStep, out var value) &&
                value.HasSecondText)
            {
                OnTutorialHalfStep?.Invoke();
            }
        }

        private void NextTutorialStepHandler(int softCount)
        {
            _playerResourcesService.AddResource(ResourceNames.Soft, softCount);
        }

        public void ActivateEmeraldsStep()
        {
            SetTutorialStep((int) TutorialStepNames.ClosedTaskWindow);
        }

        public void ActivateBoostStep()
        {
            if ((TutorialStepNames) TutorialStep is TutorialStepNames.ClosedTaskWindow)
            {
                SetTutorialStep((int) TutorialStepNames.TakenEmeralds);
                return;
            }

            if ((TutorialStepNames) TutorialStep is TutorialStepNames.SetRecipe)
            {
                SetTutorialStep((int) TutorialStepNames.TakenEmeralds2);
            }
        }

        public void ReturnToAbilityShownAfter2Boost()
        {
            if ((TutorialStepNames) TutorialStep is TutorialStepNames.TakenEmeralds2)
            {
                SetTutorialStep((int) TutorialStepNames.Ability4Shown);
                _playerResourcesService.AddResource(ResourceNames.Bulb, 2);
            }
        }

        public void ActivateChoosingCastleStep()
        {
            if ((TutorialStepNames) TutorialStep is TutorialStepNames.TakenEmeralds)
            {
                SetTutorialStep((int) TutorialStepNames.ActivatedBoost);
            }
        }

        public void FinishChooseCastleStep()
        {
            if ((TutorialStepNames) TutorialStep is TutorialStepNames.ActivatedBoost)
            {
                SetTutorialStep((int) TutorialStepNames.ChosenCastleForBoost);
            }
        }

        public void NextAbilityTutorialStep(TutorialStepNames stepName)
        {
            if (stepName > TutorialStepNames.Ability4Shown || stepName < TutorialStepNames.OpenedAbilityPanel)
            {
                return;
            }

            SetTutorialStep((int) stepName);
        }

        public void OnCameramanDeath()
        {
            SetTutorialStep((int) TutorialStepNames.CameraMan);
        }

        private void SetTutorialStep(int stepValue)
        {
            // if (stepValue <= TutorialStep)
            // {
            //     return;
            // }

            //_softReward.TryGetValue(TutorialStep, out int rewardCount);

            _dataManager.Parameters.LastTutorialStep = stepValue;
            OnNextTutorialStep(0);
        }
    }
}