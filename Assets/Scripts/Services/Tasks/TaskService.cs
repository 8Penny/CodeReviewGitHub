using System;
using System.Collections.Generic;
using System.Linq;
using Services.Sounds;
using Services.Talents;
using Services.Tutorial;
using Settings;
using Static;
using UI.Presenters.Upgrades;

namespace Services.Tasks
{
    public class TaskService : IDisposable
    {
        private PlayerDataManager _playerData;
        private SettingsService _settingsService;
        private PlayerResourcesService _playerResourcesService;
        private TalentsService _talentsService;
        private SoundService _soundsService;
        private TutorialTaskService _tutorialTasks;
        private TutorialService _tutorialService;

        private List<ActiveTask> _currentTasks = new List<ActiveTask>();
        private List<ActiveTask> EMPTY_TASKS = new List<ActiveTask>();
        private bool _wasUpdated;
        public List<ActiveTask> CurrentTasks => _tutorialService.IsComplete ? _currentTasks : EMPTY_TASKS;

        public Action OnCurrentTasksStatusUpdated;
        public Action<ActiveTask> OnTaskCompleted;
        
        public TaskService(PlayerDataManager playerData, SettingsService settingsService,
            PlayerResourcesService playerResourcesService, TalentsService talentsService,
            SoundService soundService, TutorialTaskService tt, TutorialService tutorialService)
        {
            _playerData = playerData;
            _settingsService = settingsService;
            _playerResourcesService = playerResourcesService;
            _talentsService = talentsService;
            _soundsService = soundService;
            _tutorialTasks = tt;
            _tutorialService = tutorialService;

            _playerData.OnPreSaveStep += SaveCurrentTasks;

            _playerData.OnSaveLoaded += Setup;
            _talentsService.OnAddedAbility += OnAbilityResearch;
            _playerResourcesService.OnResourceAdded += OnResourceAdded;
        }

        public void Dispose()
        {
            _playerData.OnSaveLoaded -= Setup;
            _talentsService.OnAddedAbility -= OnAbilityResearch;
            _playerResourcesService.OnResourceAdded -= OnResourceAdded;
            _playerData.OnPreSaveStep -= SaveCurrentTasks;
        }

        private void SaveCurrentTasks()
        {
            WriteCurrenTaskToSave();
        }
        
        private void Setup()
        {
            foreach (var playerTasks in _playerData.Activated.Tasks)
            {
                var settings = _settingsService.TasksConfig.TaskBranchSettingsMap[playerTasks.BranchId]
                    .TasksMap[playerTasks.Id];
                var currentCount = playerTasks.Value;
                _currentTasks.Add(new ActiveTask(settings, currentCount));
            }
        }

        public void OnResourceAdded(ResourceNames resource, float count)
        {
            if (count <= 0)
            {
                return;
            }
            foreach (var current in CurrentTasks)
            {
                if (current.Settings.TaskType != TaskType.CollectResource)
                {
                    continue;
                }

                if (current.Settings.ResourceId != resource)
                {
                    continue;
                }

                current.FloatCount += count;
                _wasUpdated = true;
            }

            if (_wasUpdated)
            {
                _wasUpdated = false;
                OnCurrentTasksStatusUpdated?.Invoke();
            }
        }

        public void OnCastleUnlocked()
        {
            foreach (var current in CurrentTasks)
            {
                if (current.Settings.TaskType == TaskType.Unlock)
                {
                    int planetUnlocked = _playerData.Updatables.Castles.Count(t => t.Value.IsOpen);
                    current.IntCount = planetUnlocked;
                    _wasUpdated = true;
                }
            }
            if (_wasUpdated)
            {
                _wasUpdated = false;
                OnCurrentTasksStatusUpdated?.Invoke();
            }
        }

        public void OnSimpleTapDestroyed()
        {
            foreach (var current in CurrentTasks)
            {
                if (current.Settings.TaskType == TaskType.Destroy)
                {
                    current.IntCount += 1;
                    _wasUpdated = true;
                }
            }
            if (_wasUpdated)
            {
                _wasUpdated = false;
                OnCurrentTasksStatusUpdated?.Invoke();
            }
        }

        public void OnAbilityResearch(AbilityType ability)
        {
            _tutorialTasks.Research(ability);
            
            foreach (var current in CurrentTasks)
            {
                if (current.Settings.TaskType == TaskType.ResearchAbility && current.Settings.Ability == ability)
                {
                    current.IntCount += 1;
                    _wasUpdated = true;
                }
            }
            if (_wasUpdated)
            {
                _wasUpdated = false;
                OnCurrentTasksStatusUpdated?.Invoke();
            }
        }

        public void OnResourceSell(int count, int price)
        {
            _tutorialTasks.Sell(count, price);
        }
        
        public void OnCastleUpgrade(CastleParameterType castleParameterType)
        {
            _tutorialTasks.Upgrade(castleParameterType);
            
            foreach (var current in CurrentTasks)
            {
                if (current.Settings.TaskType == TaskType.Upgrade)
                {
                    current.IntCount += 1;
                    _wasUpdated = true;
                }
            }
            if (_wasUpdated)
            {
                _wasUpdated = false;
                OnCurrentTasksStatusUpdated?.Invoke();
            }
        }

        public void GetReward(int branchId, int taskId)
        {
            if (CheckIfCanClose(branchId, taskId, out var settings))
            {
                CloseTask(settings);
            }
        }
        private bool CheckIfCanClose(int branchId, int taskId, out ActiveTask active)
        {
            active = null;
            foreach (var current in CurrentTasks)
            {
                if (current.Settings.BranchId != branchId || current.Settings.Id != taskId)
                {
                    continue;
                }

                active = current;
                var settings = current.Settings;
                if (settings.TaskType == TaskType.ResearchAbility)
                {
                    return _talentsService.HasTalent(settings.Ability);
                }

                return current.Count >= settings.Count;
            }

            return false;
        }

        private void CloseTask(ActiveTask active)
        {
            _playerResourcesService.AddResource(active.Settings.IsHardReward?ResourceNames.Hard :ResourceNames.Soft, active.Settings.RewardCount);
            _soundsService.PlayCurrencySound();
            _currentTasks.Remove(active);
            OnTaskCompleted?.Invoke(active);

            TryAddTask(active.Settings.BranchId, active.Settings.Id + 1);
            OnCurrentTasksStatusUpdated?.Invoke();
        }

        public void Init()
        {
            if (!_playerData.IsFirstInit)
            {
                return;
            }
            
            TryAddTask(0, 0);
            OnCurrentTasksStatusUpdated?.Invoke();
        }
        
        private bool TryAddTask(int branchId, int taskId, bool canOpenNewBranch = true)
        {
            if (_settingsService.TasksConfig.TaskBranchSettingsMap[branchId].TasksMap
                .TryGetValue(taskId, out var newTaskSettings))
            {
                ActiveTask activeTask = new ActiveTask(newTaskSettings, 0);
                _currentTasks.Add(activeTask);
                if (newTaskSettings.TaskType == TaskType.Unlock)
                {
                    OnCastleUnlocked();
                }

                if (newTaskSettings.TaskType == TaskType.ResearchAbility && _talentsService.HasTalent(newTaskSettings.Ability))
                {
                    activeTask.IntCount = 1;
                }

                if (canOpenNewBranch)
                {
                    TryOpenNewBranch(branchId, taskId);
                }
                
                return true;
            }

            return false;
        }

        private bool TryOpenNewBranch(int branchCondition, int taskCondition)
        {
            foreach (var branch in _settingsService.TasksConfig.TaskBranchSettingsList)
            {
                if (branch.IsOpen)
                {
                    continue;
                }
                if (branch.BranchIdOpenCondition == branchCondition && branch.TaskIdOpenCondition == taskCondition)
                {
                    return TryAddTask(branch.Id, 0, false);
                }
            }

            return false;
        }

        private void WriteCurrenTaskToSave()
        {
            _playerData.Activated.Tasks.Clear();
            foreach (var current in _currentTasks)
            {
                var gameTask = new TaskData();
                gameTask.Id = current.Settings.Id;
                gameTask.BranchId = current.Settings.BranchId;
                gameTask.Value = current.Count;
                _playerData.Activated.Tasks.Add(gameTask);
            }
        }
    }
}