using System;
using Services.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

public class TaskNotifyIcon : MonoBehaviour
{
    [NonSerialized][Inject]
    public TaskService TaskService;

    [SerializeField]
    private GameObject _notifyGameObject;
    [SerializeField]
    private TextMeshProUGUI _notifyLabel;
    
    
    public void OnEnable()
    {
        TaskService.OnCurrentTasksStatusUpdated += UpdateIcon;
        UpdateIcon();
    }
    public void OnDisable()
    {
        TaskService.OnCurrentTasksStatusUpdated -= UpdateIcon;
    }

    private void UpdateIcon()
    {
        int result = 0;
        foreach (var task in TaskService.CurrentTasks)
        {
            result += task.IsCompleted()? 1 : 0;
        }

        if (result == 0)
        {
            _notifyGameObject.SetActive(false);
            return;
        }
        _notifyGameObject.SetActive(true);
        _notifyLabel.text = result.ToString();
    }
}
