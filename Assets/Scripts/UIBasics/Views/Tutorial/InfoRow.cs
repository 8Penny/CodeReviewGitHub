using Services;
using Services.Sounds;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class InfoRow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private TextMeshProUGUI _description;
    [SerializeField]
    private Image _bg;

    private SoundService _soundService;
    private UIService _uiService;
    
    private TutorialTaskSettings _currentSettings;
    private TutorialTaskData _currentTask;

    [Inject]
    public void Init(SoundService soundService, UIService ui)
    {
        _soundService = soundService;
        _uiService = ui;
    }

    public void Setup(TutorialTaskSettings settings)
    {
        _currentSettings = settings;
        UpdateData();
    }

    private void UpdateData()
    {
        _bg.sprite = _currentSettings.BG;
        _name.text = _currentSettings.InfoName;
        _description.text = string.Format(_currentSettings.InfoDescription, _currentSettings.Count);
    }
    
    public void OnInfoClicked()
    {
        _soundService.PlayClick();
        _uiService.Views.InfoView.Show(_currentSettings.TaskType, false);
    }
}
