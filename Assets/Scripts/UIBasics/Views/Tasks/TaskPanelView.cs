using System;
using Services.Tasks;
using UnityEngine;
using Zenject;

namespace UIBasics.Views.Tasks
{
    public class TaskPanelView : MonoBehaviour
    {
        [SerializeField]
        private Transform _tasksHolder;

        private TaskRowView[] _views;
        private TaskService _taskService;
        
        [Inject]
        public void Init(TaskService taskService)
        {
            _taskService = taskService;
        }

        public void Awake()
        {
            _views = _tasksHolder.GetComponentsInChildren<TaskRowView>();
            _taskService.OnCurrentTasksStatusUpdated += RefreshTasks;
            RefreshTasks();
        }

        public void OnDestroy()
        {
            _taskService.OnCurrentTasksStatusUpdated -= RefreshTasks;
        }

        private void RefreshTasks()
        {
            for (int i = 0; i < _taskService.CurrentTasks.Count; i++)
            {
                var task = _taskService.CurrentTasks[i];
                _views[i].SetSettings(task.Settings, task);
                _views[i].gameObject.SetActive(true);
            }

            for (int i = _taskService.CurrentTasks.Count; i < _views.Length; i++)
            {
                _views[i].gameObject.SetActive(false);
            }
        }
    }
}