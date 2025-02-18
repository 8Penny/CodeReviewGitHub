using Services;
using Services.Sounds;
using Services.Tasks;
using Services.UIResourceAnimator;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class TutorialRow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private TextMeshProUGUI _description;

    [SerializeField]
    private TextMeshProUGUI _progressValue;

    [SerializeField]
    private Image _progressBar;
    [SerializeField]
    private Image _bg;
    [SerializeField]
    private GameObject _completeButton;
    [SerializeField]
    private GameObject _infoButton;
    [SerializeField]
    private Button _infoButtonComponent;
    [SerializeField]
    private GameObject _blockObject;

    private TutorialTaskService _tutorialTaskService;
    private SettingsService _settingsService;
    private SoundService _soundService;
    private UIResourceAnimatorService _uiResourceAnimatorService;
    private UIService _uiService;


    private TutorialTaskSettings _currentSettings;
    private TutorialTaskData _currentTask;

    [Inject]
    public void Init(TutorialTaskService taskService, SettingsService settingsService, SoundService soundService,
        UIResourceAnimatorService uiResourceAnimatorService, UIService ui)
    {
        _tutorialTaskService = taskService;
        _settingsService = settingsService;
        _soundService = soundService;
        _uiResourceAnimatorService = uiResourceAnimatorService;
        _uiService = ui;
    }


    public void Setup(TutorialTaskSettings settings, TutorialTaskData task)
    {
        _currentSettings = settings;
        _currentTask = task;

        UpdateData();
    }

    public void UpdateData()
    {
        _bg.sprite = _currentSettings.BG;
        _name.text = _currentSettings.Name;
        int targetCount = _currentSettings.Count == 0 ? 1 : _currentSettings.Count;
        switch (_currentSettings.TaskType)
        {
            case TutorialTaskType.UpgradeCastle:
                targetCount += 2;
                break;
            case TutorialTaskType.UpgradeCat:
            case TutorialTaskType.UpgradeBackpack:
                targetCount += 1;
                break;
        }
        _description.text = string.Format(_currentSettings.Description, targetCount);

        int currentCount = _currentTask.Value;
        switch (_currentSettings.TaskType)
        {
            case TutorialTaskType.UpgradeCastle:
                currentCount += 2;
                break;
            case TutorialTaskType.UpgradeCat:
            case TutorialTaskType.UpgradeBackpack:
                currentCount += 1;
                break;
        }
        string progressText = $"{UiUtils.GetCountableValue(currentCount)} / {UiUtils.GetCountableValue(targetCount)}";

        _progressValue.text = progressText;
        
        float progress = Mathf.Clamp(currentCount/ (float) targetCount,0,1f);

        _progressBar.fillAmount = progress;
        bool completed = currentCount >= targetCount;
        _infoButton.SetActive(!completed);
        _completeButton.SetActive(completed);
    }

    public void UpdateInteractable(bool isInteractable = true)
    {
        _infoButtonComponent.interactable = isInteractable;
        _blockObject.SetActive(!isInteractable);
    }

    public void OnButtonClicked()
    {
        if (_tutorialTaskService.TryCompleteTask(_currentTask.TutorialTaskType))
        { 
            _soundService.PlayTutorialTaskComplete();
        }
    }
    public void OnInfoClicked()
    {
        _soundService.PlayClick();
        _uiService.Views.InfoView.Show(_currentSettings.TaskType);
    }
}
