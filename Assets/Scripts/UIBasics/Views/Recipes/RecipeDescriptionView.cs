using Services;
using Services.Sounds;
using Services.Talents;
using Settings;
using Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.Recipes
{
    public class RecipeDescriptionView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _unlocked;
        [SerializeField]
        private GameObject _locked;
        [SerializeField]
        private Image _bg;
        [SerializeField]
        private Sprite _active;
        [SerializeField]
        private Sprite _inactive;

        [Space]
        [SerializeField]
        private TextMeshProUGUI _unlockedHeader;
        [SerializeField]
        private TextMeshProUGUI _unlockedTime;
        [SerializeField]
        private RecipeBlockView _unlockedBlock;
        
        [Space]
        [SerializeField]
        private TextMeshProUGUI _lockedHeader;
        [SerializeField]
        private TextMeshProUGUI _lockedPrice;
        [SerializeField]
        private TextMeshProUGUI _lockedText;
        [SerializeField]
        private Image _lockedResourceImage;

        private RecipeService _recipeService;
        private  SettingsService _settingsService;
        private  SoundService _soundService;
        private UIService _uiService;
        private TalentsService _talentsService;
        private PlayerResourcesService _resourcesService;
        
        private int _currentBenchId;
        private int _currentRecipeId;
        private bool _isLocked;
        private RecipeSettings _currentRecipeSettings;

        [Inject]
        public void Init(RecipeService recipeService, SettingsService settingsService,
            UIService uiService, TalentsService talentsService, SoundService soundService,
            PlayerResourcesService resourcesService)
        {
            _recipeService = recipeService;
            _settingsService = settingsService;
            _uiService = uiService;
            _talentsService = talentsService;
            _soundService = soundService;
            _resourcesService = resourcesService;
        }

        public void SetRecipe(int id, int benchId, bool isLocked = false)
        {
            _currentBenchId = benchId;
            _currentRecipeId = id;
            _isLocked = isLocked;

            UpdatePanel();
        }

        private void UpdatePanel()
        {
            _currentRecipeSettings = _recipeService.GetRecipe(_currentBenchId, _currentRecipeId);
            _unlocked.gameObject.SetActive(!_isLocked);
            _locked.gameObject.SetActive(_isLocked);

            if (_isLocked)
            {
                SetupLocked();
            }
            else
            {
                SetupUnlocked();
            }
        }

        private void SetupLocked()
        {
            bool isAvailable = Mathf.FloorToInt(_resourcesService.GetResource(ResourceNames.Soft)) >= _currentRecipeSettings.RecipePrice;
            _bg.sprite = isAvailable ? _active : _inactive;
            _lockedPrice.color = isAvailable ? Color.white : StaticValues.InactiveTextColor;
            _lockedText.color = isAvailable ? Color.white : StaticValues.InactiveTextColor;
            _lockedHeader.text = StaticNames.Get(_currentRecipeSettings.RecipeResourceId);
            _lockedPrice.text = UiUtils.GetCountableValue(_currentRecipeSettings.RecipePrice, 0);
            _lockedResourceImage.sprite =
                _settingsService.GameResources[_currentRecipeSettings.RecipeResourceId].Sprite;
        }
        
        private void SetupUnlocked()
        {
            _unlockedHeader.text = StaticNames.Get(_currentRecipeSettings.RecipeResourceId);
            float multiplier = _currentBenchId == 0
                ? _talentsService.WorkbenchMultiplier
                : _talentsService.CraftTableMultiplier;
            _unlockedTime.text = UiUtils.FormatedTime(Mathf.CeilToInt(_currentRecipeSettings.Time * multiplier));
            _unlockedBlock.SetRecipe( _currentBenchId, _currentRecipeId);
        }
        
        public void OnButtonClicked()
        {
            //_soundService.PlayClick();
            _recipeService.Interact(_currentBenchId, _currentRecipeSettings, _isLocked, out bool canClose);
            if (canClose)
            {
                _uiService.CloseWindow();
            }
        }
    }
}