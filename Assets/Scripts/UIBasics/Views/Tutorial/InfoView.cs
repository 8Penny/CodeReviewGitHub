using Services;
using Services.View;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.Tutorial
{
    public class InfoView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _main;
        [SerializeField]
        private TextMeshProUGUI _header;
        [SerializeField]
        private TextMeshProUGUI _description;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Image _abilityImage;
        [SerializeField]
        private Text _textButton;
        [SerializeField]
        private GameObject _goToButton;

        private UIService _uiService;
        private SettingsService _settingsService;
        private ViewCastlesService _viewCastlesService;

        private InfoSingleSettings _settings;
        
        [Inject]
        public void Init(SettingsService settingsService, UIService uiService, ViewCastlesService viewCastlesService)
        {
            _viewCastlesService = viewCastlesService;
            _uiService = uiService;
            _settingsService = settingsService;
            _main.SetActive(false);
        }
        
        public void Show(TutorialTaskType taskType, bool isTutorial = true)
        {
            _settings = _settingsService.InfoSettings.InfoDict[taskType];
            _header.text = _settings.Header;
            _description.text = _settings.Description;
            _image.sprite = _settings.Image;

            if (_settings.AbilityImage != null)
            {
                _abilityImage.sprite = _settings.AbilityImage;
                _abilityImage.gameObject.SetActive(true);
            } else
            {
                _abilityImage.gameObject.SetActive(false);
            }
            
            _goToButton.SetActive(isTutorial);
            if (isTutorial)
            {
                _textButton.text = _settings.GoToText;
            }
            
            _main.gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            _main.gameObject.SetActive(false);
        }

        public void OnGoToButtonClick()
        {
            Hide();
            _uiService.CloseWindow();
            switch (_settings.Type)
            {
                case TutorialTaskType.UpgradeCat:
                case TutorialTaskType.UpgradeBackpack:
                case TutorialTaskType.UpgradeCastle:
                    _viewCastlesService.InteractCastle(0);
                    break;
                case TutorialTaskType.OpenCraftTable:
                case TutorialTaskType.OpenFlowerPicker:
                case TutorialTaskType.OpenWorkbench:
                    _uiService.OnPanelButtonClicked(PanelType.AbilityTree);
                    break;
                case TutorialTaskType.EarnCatCoins:
                case TutorialTaskType.SellFlies:
                    _uiService.OnPanelButtonClicked(PanelType.Resources);
                    break;
            }
        }
    }
}