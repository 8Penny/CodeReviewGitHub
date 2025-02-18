using System;
using System.Collections.Generic;
using System.Linq;
using Services.Tasks;
using Settings;
using UnityEngine;
using Zenject;

namespace Services.Tutorial
{
    public class TutorialTasksWindow : MonoBehaviour
    {
        [SerializeField]
        private GameObject _currentContainer;
        [SerializeField]
        private GameObject _parentWindow;
        [SerializeField]
        private TutorialRow _tutorialRow;

        private TutorialTaskService _tutorialTaskService;
        private TutorialService _tutorialService;

        private List<TutorialRow> _rows = new List<TutorialRow>();
        private Dictionary<TutorialTaskType, TutorialRow> _rowByType = new Dictionary<TutorialTaskType, TutorialRow>();

        [Inject]
        public void Init(TutorialService t, TutorialTaskService tutorialTaskService, DiContainer container)
        {
            _tutorialService = t;
            _tutorialTaskService = tutorialTaskService;
            InitRows(container);
        }

        private void InitRows(DiContainer container)
        {
            Transform parent = _tutorialRow.transform.parent;
            _rows.Add(_tutorialRow);
            _tutorialRow.gameObject.SetActive(false);
            for (int i = 0; i < 4; i++)
            {
                GameObject go = container.InstantiatePrefab(_tutorialRow.gameObject, parent);
                var tutorialRow = go.GetComponent<TutorialRow>();
                _rows.Add(tutorialRow);
            }
        }

        private void Activate()
        {
            _tutorialTaskService.TaskAdded += TaskAddedHandler;
            _tutorialTaskService.TaskCompleted += TaskCompletedHandler;
            _tutorialTaskService.TaskUpdated += TaskUpdateHandler;
            TaskAddedHandler();
        }

        private void Deactivate()
        {
            _tutorialTaskService.TaskAdded -= TaskAddedHandler;
            _tutorialTaskService.TaskCompleted -= TaskCompletedHandler;
            _tutorialTaskService.TaskUpdated -= TaskUpdateHandler;
        }

        private void TaskAddedHandler()
        {
            if (!_tutorialTaskService.HasTutorialTasks)
            {
                foreach (var row in _rowByType)
                {
                    _rows.Add(row.Value);
                    row.Value.gameObject.SetActive(false);
                }
                _rowByType.Clear();
                return;
            }

            foreach (var task in _tutorialTaskService.CurrentTasks)
            {
                if (!_rowByType.TryGetValue(task.TutorialTaskType, out var oldRow))
                {
                    var row = _rows.First();
                    _rowByType.Add(task.TutorialTaskType, row);
                    row.Setup(_tutorialTaskService.GetSettings(task.TutorialTaskType), task);
                    _rows.Remove(row);
                    row.gameObject.SetActive(true);
                    continue;
                }

                oldRow.UpdateData();
            }

            UpdateInteractableTask();
        }

        private void UpdateInteractableTask()
        {
            UpdateForUpgradeTasks();
            UpdateForAbilityTasks();
        }

        private void UpdateForUpgradeTasks()
        {
            bool hasUpgradeCastle = _rowByType.ContainsKey(TutorialTaskType.UpgradeCastle);
            bool hasUpgradeCat = _rowByType.TryGetValue(TutorialTaskType.UpgradeCat, out var cat);
            bool hasUpgradeBackpack = _rowByType.TryGetValue(TutorialTaskType.UpgradeBackpack, out var backpack);

            if (!hasUpgradeBackpack)
            {
                return;
            }

            if (hasUpgradeCat)
            {
                cat.UpdateInteractable(!hasUpgradeCastle);
            }
            backpack.UpdateInteractable(!hasUpgradeCat);
        }

        private void UpdateForAbilityTasks()
        {
            bool hasFlower = _rowByType.ContainsKey(TutorialTaskType.OpenFlowerPicker);
            bool hasWorkbench = _rowByType.TryGetValue(TutorialTaskType.OpenWorkbench, out var work);
            bool hasCraftTable = _rowByType.TryGetValue(TutorialTaskType.OpenCraftTable, out var craft);

            if (!hasCraftTable)
            {
                return;
            }

            if (hasWorkbench)
            {
                work.UpdateInteractable(!hasFlower);
            }
            craft.UpdateInteractable(!hasWorkbench);
        }
        
        private void TaskCompletedHandler(TutorialTaskType type)
        {
            if (_rowByType.TryGetValue(type, out var oldRow))
            {
                oldRow.gameObject.SetActive(false);
                _rows.Add(oldRow);
                _rowByType.Remove(type);
            }

            UpdateInteractableTask();
        }

        private void TaskUpdateHandler(TutorialTaskType type, bool forced)
        {
            if (_rowByType.TryGetValue(type, out var oldRow))
            {
                oldRow.UpdateData();
            }

            UpdateInteractableTask();
        }
        
        public void Show()
        {
            _currentContainer.SetActive(true);
            _parentWindow.SetActive(true);
            Activate();
        }
        
        public void Close()
        {
            _currentContainer.SetActive(false);
            _parentWindow.SetActive(false);
            Deactivate();

            if (_tutorialTaskService.HasEarnTask())
            {
                if ((TutorialStepNames) _tutorialService.TutorialStep == TutorialStepNames.ResourceSold)
                {
                    _tutorialService.ActivateEmeraldsStep();
                }
            }
        }
    }
}