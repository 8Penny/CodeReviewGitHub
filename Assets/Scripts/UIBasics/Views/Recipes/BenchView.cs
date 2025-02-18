using System;
using Services;
using Services.Craft;
using Settings;
using Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.Recipes
{
    public class BenchView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _headerLabel;
        [SerializeField]
        private RecipeBlockView _recipeBlockView;
        [SerializeField]
        private RecipeBlockView _doubleRecipeBlockView;
        [SerializeField]
        private RecipeBlockView _tripleRecipeBlockView;
        
        [SerializeField]
        private GameObject _noRecipe;
        [SerializeField]
        private GameObject _blockingRecipeGO;
        [SerializeField]
        private GameObject _closeRecipeGO;
        [SerializeField]
        private Image _headerImage;
        
        [SerializeField]
        private Image _buyBenchImage;
        [SerializeField]
        private Sprite _activeButtonImage;
        [SerializeField]
        private Sprite _inactiveButtonImage;
        
        [SerializeField]
        private TextMeshProUGUI _benchPrice;
        [SerializeField]
        private TextMeshProUGUI _benchUnlockText;
        
        private Color _currentBenchTypeColor;
        private Color _unavailableColor;
        
        private RecipeBlockView _currentRecipe;
        private RecipeService _recipeService;
        private SettingsService _settingsService;
        private UIService _uiService;
        private CraftService _craftService;
        
        private CraftEntityPresenter _presenter;
        private int _currentBenchId;
        private RecipeSettings _lockedRecipe;

        private bool _wasUpdated;
        private bool _previousWorkStatus;

        [Inject]
        public void Init(RecipeService recipeService, UIService uiService, SettingsService settingsService,
            CraftService craftService)
        {
            _recipeService = recipeService;
            _uiService = uiService;
            _settingsService = settingsService;
            _craftService = craftService;

            _currentRecipe = _recipeBlockView;
            DisableRecipeViews();
            _recipeService.OnRecipeUnlocked += UpdateView;
        }

        private void DisableRecipeViews()
        {
            _recipeBlockView.gameObject.SetActive(false);
            _doubleRecipeBlockView.gameObject.SetActive(false);
            _tripleRecipeBlockView.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {
            _recipeService.OnRecipeUnlocked += UpdateView;
        }

        public void OnRecipeButtonClicked()
        {
            _recipeService.SetCurrentPresenter(_presenter);
            _uiService.ShowRecipesWindow(_presenter.CraftTypeId);
        }

        public void SetPresenter(CraftEntityPresenter p)
        {
            _presenter = p;
            _presenter.WasUpdated -= UpdateRecipe;
            _presenter.WasUpdated += UpdateRecipe;
            
            UpdateRecipe();
        }
        public void SetColors(Color current, Color unavailable)
        {
            _unavailableColor = current;
            _currentBenchTypeColor = current;
        }

        public void ClearPresenter()
        {
            if (_presenter != null)
            {
                _presenter.WasUpdated -= UpdateRecipe;
                _presenter = null;
            }

            _wasUpdated = false;
        }

        private void UpdateView(int _, int __)
        {
            if (_presenter == null)
            {
                return;
            }
            UpdateRecipe();
        }

        private void UpdateRecipe()
        {
            _currentBenchId = _presenter.CraftTypeId;
            _recipeService.GetOpenedRecipes(_currentBenchId, out int nextRecipeIndex);

            ChooseRecipeBlock(_presenter.CraftTypeId, _presenter.RecipeId);
            _currentRecipe.SetRecipe(_presenter.CraftTypeId, _presenter.RecipeId, true);
            
            SetHeader();
            _closeRecipeGO.SetActive(true);
            _blockingRecipeGO.SetActive(false);
        }

        public void ShowAsSlot(int price, bool isActive = true)
        {
            _buyBenchImage.sprite = isActive? _activeButtonImage : _inactiveButtonImage;
            _benchPrice.color = isActive ? Color.white : StaticValues.InactiveTextColor;
            _benchUnlockText.color = isActive ? Color.white : StaticValues.InactiveTextColor;
            _blockingRecipeGO.SetActive(true);
            _closeRecipeGO.SetActive(false);
            _noRecipe.SetActive(false);
            DisableRecipeViews();
            FillInBuyBlock(price);
        }

        private void SetHeader()
        {
            if (_presenter == null)
            {
                return;
            }

            _headerImage.color = _presenter.RecipeId == -1 ? _unavailableColor : _currentBenchTypeColor;
            _headerLabel.text = _presenter.RecipeId == -1? "Empty" : StaticNames.Get(_currentRecipe.Recipe.RecipeResourceId);
        }

        private void ChooseRecipeBlock(int bench, int recipe)
        {
            if (recipe == -1)
            {
                return;
            }
            _currentRecipe.gameObject.SetActive(false);
            int count = _settingsService.Recipes.GetRecipe(bench, recipe).Demands.Count;
            switch (count)
            {
                case 1:
                    _currentRecipe = _recipeBlockView;
                    break;
                case 2:
                    _currentRecipe = _doubleRecipeBlockView;
                    break;
                case 3:
                    _currentRecipe = _tripleRecipeBlockView;
                    break;
            }
        }

        private void FillInBuyBlock(int price)
        {
            _benchPrice.text = UiUtils.GetCountableValue(price, 0);
        }

        public void Update()
        {
            if (_presenter == null)
            {
                return;
            }
            _currentRecipe.gameObject.SetActive(_presenter.HasRecipe);
            _noRecipe.SetActive(!_presenter.HasRecipe);
            _closeRecipeGO.SetActive(_presenter.HasRecipe);

            if (_presenter.HasRecipe)
            {
                UpdateDynamicParameters();
                _wasUpdated = false;
                return;
            }
            if ( !_wasUpdated )
            {
                _wasUpdated = true;
                _currentRecipe.SetProgress(null,0, false);
                _headerLabel.text = "Empty";
            }
        }

        private void UpdateDynamicParameters()
        {
            bool isWorking = _presenter.CanContinueWork();
            if (_wasUpdated && !isWorking && !_previousWorkStatus)
            {
                return;
            }
            _currentRecipe.SetProgress($"{Math.Round(_presenter.Progress, 2) * 100}%", _presenter.Progress, isWorking);
            _previousWorkStatus = isWorking;
            _wasUpdated = true;
        }

        public void FreeRecipe()
        {
            _presenter.SetRecipe(-1);
        }
        
        public void OnBenchBuyClicked()
        {
            _craftService.TryBuyNewSlot(_currentBenchId);
        }

        public void SetBenchId(int currentBenchId)
        {
            _currentBenchId = currentBenchId;
        }

        public void OnDisable()
        {
            if (_presenter == null)
            {
                return;
            }
            _presenter.WasUpdated -= UpdateRecipe;
        }
    }
}