using Services;
using Services.Sounds;
using Services.Talents;
using Services.Tutorial;
using Settings;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Sirenix.OdinInspector;

namespace UIBasics.Views
{
    public class MainPanelButton : MonoBehaviour
    {
        [SerializeField]
        private PanelType _panelType;
        [SerializeField]
        private Color _color;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private GameObject _iconOutline;
        [SerializeField]
        private Image _bgImage;
        [SerializeField]
        private Image _boopImage;
        [SerializeField]
        private GameObject _selectedImage;
        [SerializeField]
        private GameObject _lock;
        [SerializeField]
        private GameObject _lockIcon;

        private UIService _uiService;
        private TalentsService _talentsService;
        private SoundService _soundService;
        private TutorialService _tutorialService;


        [Inject]
        public void Init(UIService uiService, TalentsService talentsService, SoundService soundService, TutorialService tutorialService)
        {
            _uiService = uiService;
            _talentsService = talentsService;
            _soundService = soundService;
            _tutorialService = tutorialService;
        }

        public void Awake()
        {
            UpdateVisibility();
            OnPanelChanged(PanelType.None);
            _uiService.Views.DataPanelView.RegisterColor(_panelType, _color);
            UpdateColor();
        }

        [Button]
        public void UpdateColor()
        {
            _bgImage.color = Color.clear;
            _boopImage.color = _color;
        }

        public void OnEnable()
        {
            _talentsService.OnAbilitiesUpdated += UpdateVisibility;
            _uiService.OnPanelChanged += OnPanelChanged;
            _tutorialService.OnNextTutorialStep += UpdateLockVisibility;
            UpdateLockVisibility(0);
        }
        
        public void OnDisable()
        {
            _talentsService.OnAbilitiesUpdated -= UpdateVisibility;
            _uiService.OnPanelChanged -= OnPanelChanged;
            _tutorialService.OnNextTutorialStep -= UpdateLockVisibility;
        }

        private void UpdateLockVisibility(int _)
        {
            bool isVisible = !IsAvailable();
            if (_lockIcon == null)
            {
                return;
            }
            _lockIcon.SetActive(isVisible);
        }

        private bool IsAvailable()
        {
            if (_lockIcon == null)
            {
                return true;
            }

            if (_tutorialService.IsComplete)
            {
                return true;
            }
            bool isAvailable = false;
            switch (_panelType)
            {
                case PanelType.Resources:
                    isAvailable = _tutorialService.TutorialStep >= (int)TutorialStepNames.UpgradedAllCastleParametersTasks;
                    break;
                case PanelType.AbilityTree:
                    isAvailable = _tutorialService.TutorialStep >= (int)TutorialStepNames.SellAndEarnTasks;
                    break;
                case PanelType.Benches:
                    isAvailable = _talentsService.HasTalent(AbilityType.Workbench1);
                    break;
                case PanelType.Boosters:
                    isAvailable = _tutorialService.TutorialStep >= (int)TutorialStepNames.TakenEmeralds;
                    break;
            }

            return isAvailable;
        }

        private void UpdateVisibility()
        {
            if (_panelType == PanelType.Benches)
            {
                bool isLocked = !_talentsService.HasTalent(AbilityType.Workbench1);
                _lock.gameObject.SetActive(isLocked);
                _icon.gameObject.SetActive(!isLocked);
                _lockIcon.gameObject.SetActive(isLocked);
            }
        }

        public void OnButtonClick()
        {
            if (!IsAvailable())
            {
                return;
            }
            if (_panelType == PanelType.Benches && !_talentsService.HasTalent(AbilityType.Workbench1))
            {
                return;
            }
            _uiService.OnPanelButtonClicked(_panelType);
            _soundService.PlayClick();
        }

        private void OnPanelChanged(PanelType pType)
        {
            bool isVisible = pType == _panelType;
            
            _selectedImage.SetActive(isVisible);
            _bgImage.color = isVisible? _color : Color.clear;
            _iconOutline.SetActive(isVisible);
        }
    }
}