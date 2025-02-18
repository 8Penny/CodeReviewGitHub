using System;
using System.Collections.Generic;
using System.Linq;
using Settings;
using UI.Presenters.Upgrades;

namespace Services.Tasks
{
    public class TutorialTaskService : IDisposable
    {
        private TutorialBranchSettings _settings;
        private PlayerDataManager _playerData;

        private List<TutorialTaskData> _currentTasks;
        
        public bool HasTutorialTasks => _currentTasks?.Count > 0;
        public List<TutorialTaskData> CurrentTasks => _currentTasks;

        public Action<TutorialTaskType, bool> TaskUpdated;
        public Action<TutorialTaskType> TaskCompleted;
        public Action TaskAdded;
        
        public TutorialTaskService(SettingsService settingsService, PlayerDataManager playerDataManager)
        {
            _settings = settingsService.TutorialTaskSettings;
            _playerData = playerDataManager;
            _playerData.OnSaveLoaded += Setup;
        }

        public void Dispose()
        {
            _playerData.OnSaveLoaded -= Setup;
        }

        private void Setup()
        {
            _currentTasks = _playerData.Activated.TutorialTasks;
        }

        public void AddUpgradeTasks()
        {
            AddTask(TutorialTaskType.UpgradeCastle);
            AddTask(TutorialTaskType.UpgradeCat);
            AddTask(TutorialTaskType.UpgradeBackpack);
            TaskAdded?.Invoke();
        }

        public void AddFirefliesTask()
        {
            AddTask(TutorialTaskType.SellFlies);
            TaskAdded?.Invoke();
        }
        private void AddSellTask()
        {
            AddTask(TutorialTaskType.EarnCatCoins);
            TaskAdded?.Invoke();
        }

        public void AddResearchTasks()
        {
            AddTask(TutorialTaskType.OpenFlowerPicker);
            AddTask(TutorialTaskType.OpenWorkbench);
            AddTask(TutorialTaskType.OpenCraftTable);
            TaskAdded?.Invoke();
        }
        
        private void AddTask(TutorialTaskType taskType)
        {
            var t = new TutorialTaskData
            {
                TutorialTaskType = taskType
            };
            _currentTasks.Add(t);
        }

        public void Upgrade(CastleParameterType castleParameterType, bool force = false)
        {
            switch (castleParameterType)
            {
                case CastleParameterType.Capacity:
                    TryIncrementTask(TutorialTaskType.UpgradeBackpack);
                    break;
                case CastleParameterType.MiningRate:
                    TryIncrementTask(TutorialTaskType.UpgradeCastle, 1, force);
                    break;
                case CastleParameterType.CatSpeed:
                    TryIncrementTask(TutorialTaskType.UpgradeCat);
                    break;
            }
        }

        public void Sell(int count, int amountCoins, bool forced = false)
        {
            TryIncrementTask(TutorialTaskType.SellFlies, count, forced);
            TryIncrementTask(TutorialTaskType.EarnCatCoins, amountCoins, forced);
        }

        public bool CanUpgrade(CastleParameterType currentType)
        {
            if (!HasTutorialTasks)
            {
                return true;
            }

            switch (currentType)
            {
                case CastleParameterType.MiningRate:
                    return !IsTaskCompleted(TutorialTaskType.UpgradeCastle);
                case CastleParameterType.CatSpeed:
                    bool hasCastleTask = _currentTasks.Any(t => t.TutorialTaskType == TutorialTaskType.UpgradeCastle);
                    return !hasCastleTask && !IsTaskCompleted(TutorialTaskType.UpgradeCat);
                case CastleParameterType.Capacity:
                    bool hasCatTask = _currentTasks.Any(t => t.TutorialTaskType == TutorialTaskType.UpgradeCat);
                    return !hasCatTask && !IsTaskCompleted(TutorialTaskType.UpgradeBackpack);
            }

            return true;
        }

        private bool IsTaskCompleted(TutorialTaskType taskType)
        {
            var data = _currentTasks.FirstOrDefault(s => s.TutorialTaskType == taskType);
            if (data == null)
            {
                return true;
            }

            return IsTaskCompleted(taskType, data);
        }

        public void Research(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.Workbench1:
                    TryIncrementTask(TutorialTaskType.OpenWorkbench);
                    break;
                case AbilityType.Flower1:
                    TryIncrementTask(TutorialTaskType.OpenFlowerPicker);
                    break;
                case AbilityType.CraftTable1:
                    TryIncrementTask(TutorialTaskType.OpenCraftTable);
                    break;
            }
        }

        private void TryIncrementTask(TutorialTaskType taskType, int value = 1, bool forced = false)
        {
            TutorialTaskData taskData =
                _currentTasks.FirstOrDefault(t => t.TutorialTaskType == taskType);

            if (taskData != null)
            {
                taskData.Value += value;
                TaskUpdated?.Invoke(taskType, forced);
            }
        }

        public bool HasAnyCompleted()
        {
            if (!HasTutorialTasks)
            {
                return false;
            }

            foreach (var current in _currentTasks)
            {
                if (IsTaskCompleted(current.TutorialTaskType))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsTaskCompleted(TutorialTaskType type, TutorialTaskData t = null)
        {
            if (t == null)
            {
                t = _currentTasks.FirstOrDefault(s => s.TutorialTaskType == type);
            }
            
            if (t == null)
            {
                return false;
            }
            if (_settings.TasksDic[type].Count > t.Value)
            {
                return false;
            }

            return true;
        }
        
        public bool TryCompleteTask(TutorialTaskType type)
        {
            var t = _currentTasks.FirstOrDefault(s => s.TutorialTaskType == type);
            if (IsTaskCompleted(type, t))
            {
                _currentTasks.Remove(t);
                if (type == TutorialTaskType.SellFlies)
                {
                    AddSellTask();
                }
                TaskCompleted.Invoke(type);
                return true;
            }

            return false;
        }


        public TutorialTaskSettings GetSettings(TutorialTaskType taskType)
        {
            return _settings.TasksDic[taskType];
        }
        public int GetTaskTargetValue(TutorialTaskType taskType)
        {
            return _settings.TasksDic[taskType].Count;
        }

        public bool HasEarnTask()
        {
            if (!HasTutorialTasks)
            {
                return false;
            }

            return _currentTasks[0].TutorialTaskType is TutorialTaskType.EarnCatCoins;
        }
    }
}