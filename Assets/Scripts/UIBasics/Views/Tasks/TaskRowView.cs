using Services;
using Services.Sounds;
using Services.Tasks;
using Services.UIResourceAnimator;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class TaskRowView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private TextMeshProUGUI _description;

    [SerializeField]
    private TextMeshProUGUI _rewardCount;
    [SerializeField]
    private TextMeshProUGUI _disablrRewardCount;

    [SerializeField]
    private TextMeshProUGUI _progressValue;

    [SerializeField]
    private Image _progressBar;
    [SerializeField]
    private GameObject _enabledButton;
    [SerializeField]
    private GameObject _disabledButton;
    [SerializeField]
    private RectTransform _buttonPosition;

    private TaskService _taskService;
    private SettingsService _settingsService;
    private SoundService _soundService;
    private UIResourceAnimatorService _uiResourceAnimatorService;

    private TaskSettings _settings;
    private ActiveTask _activeTask;

    [Inject]
    public void Init(TaskService taskService, SettingsService settingsService, SoundService soundService,
        UIResourceAnimatorService uiResourceAnimatorService)
    {
        _taskService = taskService;
        _settingsService = settingsService;
        _soundService = soundService;
        _uiResourceAnimatorService = uiResourceAnimatorService;
    }

    private void OnEnable()
    {
        _taskService.OnCurrentTasksStatusUpdated += UpdateDynamicParameters;
        UpdateDynamicParameters();
    }

    private void OnDisable()
    {
        _taskService.OnCurrentTasksStatusUpdated -= UpdateDynamicParameters;
    }

    public void SetSettings(TaskSettings settings, ActiveTask activeTask)
    {
        if (_settings == settings)
        {
            return;
        }
        _settings = settings;
        _activeTask = activeTask;

        UpdateMainParameters();
        UpdateDynamicParameters();
    }

    private void UpdateMainParameters()
    {
        _name.text = _settings.Name;
        _description.text = UiUtils.GetTaskDescription(_settings, _settingsService);
        int currencyIcon = _settings.IsHardReward ? 1 : 0;
        string result = UiUtils.GetCountableValue(_settings.RewardCount, currencyIcon);
        _disablrRewardCount.text = result;
        _rewardCount.text = result;
    }

    private void UpdateDynamicParameters()
    {
        if (_settings == null)
        {
            return;
        }
        var result = _settings.Count == 0 ? 1 : _settings.Count;
        if (_settings.TaskType == TaskType.ResearchAbility)
        {
            result = 1;
        }
        _progressValue.text = $"{UiUtils.GetCountableValue(_activeTask.Count)} / {UiUtils.GetCountableValue(result)}";
        float progress = Mathf.Clamp(_activeTask.Count / (float) result,0,1f);

        _progressBar.fillAmount = progress;
        bool completed = _activeTask.Count >= result;
        _disabledButton.SetActive(!completed);
        _enabledButton.SetActive(completed);
    }

    public void OnButtonClicked()
    {
        _soundService.PlayClick();
        _uiResourceAnimatorService.Play(_buttonPosition.position,true, _settings.IsHardReward);
        _taskService.GetReward(_settings.BranchId, _settings.Id);
        
    }
}
