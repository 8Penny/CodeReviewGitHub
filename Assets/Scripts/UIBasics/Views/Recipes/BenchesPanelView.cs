using Services;
using Services.Craft;
using Services.Sounds;
using Services.Talents;
using Services.Tutorial;
using Settings;
using Static;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.Recipes
{
    public class BenchesPanelView : MonoBehaviour
    {
        [SerializeField]
        private BenchView[] _views;
        [SerializeField]
        private RectTransform _ScrollRect;
        [SerializeField]
        private RectTransform _rect;
        [SerializeField]
        private Color _colorBench1;
        [SerializeField]
        private Color _colorBench2;
        [SerializeField]
        private Color _unavailableColor;
        [SerializeField]
        private Image _bench1Button;
        [SerializeField]
        private Image _bench2Button;
        [SerializeField]
        private Image _bottomLine;
        [SerializeField]
        private GameObject _lock;

        private PlayerDataManager _dataManager;
        private RecipeService _recipeService;
        private SoundService _soundService;
        private CraftService _craftService;
        private TalentsService _talentsService;
        private TutorialService _tutorialService;
        private PlayerResourcesService _playerResourcesService;
        
        private BenchType _currentType;

        [Inject]
        public void Init(PlayerDataManager dataManager,
            RecipeService recipeService, SoundService soundService,
            CraftService craftService, TalentsService talentsService,
            TutorialService tutorialService, PlayerResourcesService playerResourcesService)
        {
            _playerResourcesService = playerResourcesService;
            _dataManager = dataManager;
            _recipeService = recipeService;
            _soundService = soundService;
            _craftService = craftService;
            _talentsService = talentsService;
            _tutorialService = tutorialService;
            
            _recipeService.OnRecipeUnlocked += UpdateElements;
            _craftService.OnBenchAdded += UpdateElements;

            UpdateColors();
        }

        private void OnDestroy()
        {
            _recipeService.OnRecipeUnlocked -= UpdateElements;
            _craftService.OnBenchAdded -= UpdateElements;
        }

        private void UpdateLockIcon()
        {
            _lock.SetActive(!_talentsService.HasTalent(AbilityType.CraftTable1));
        }

        private void OnEnable()
        {
            _currentType = BenchType.Workshop;
            UpdateElements();
            UpdateLockIcon();
        }

        public void OnTabSelected(BenchTypeView view)
        {
            if (_currentType == view.BenchType)
            {
                return;
            }
            
            if (!_talentsService.HasTalent(AbilityType.CraftTable1) && view.BenchType == BenchType.Recycling)
            {
                return;
            }

            _tutorialService.ClickCraftTable();
            _soundService.PlayClick();
            _currentType = view.BenchType;
            UpdateElements();
        }

        private void UpdateElements(int __, int _)
        {
            UpdateElements();
        }

        private void UpdateElements()
        {
            var craftEntities = _currentType == BenchType.Workshop
                ? _craftService.Workbenches
                : _craftService.CraftTables;

            int count = 0;
            Color currentColor = _currentType == BenchType.Workshop? _colorBench1 : _colorBench2;
            foreach (var craft in craftEntities)
            {
                _views[craft.Key.Id].SetBenchId(_craftService.GetBenchId(_currentType));
                _views[craft.Key.Id].SetColors(currentColor, _unavailableColor);
                _views[craft.Key.Id].SetPresenter(craft.Value);
                _views[craft.Key.Id].gameObject.SetActive(true);
                count += 1;
            }

            bool withBench = craftEntities.Count < CraftService.BENCH_MAX_COUNT;
            if (withBench)
            {
                int id = craftEntities.Count;
                _views[id].gameObject.SetActive(true);
                int price = _craftService.GetBenchPrice(id, _currentType == BenchType.Workshop);
                int currentAmount = Mathf.FloorToInt(_playerResourcesService.GetResource(ResourceNames.Soft));
                _views[id].ShowAsSlot(price, currentAmount >= price);
                _views[id].SetBenchId(_craftService.GetBenchId(_currentType));
                _views[id].ClearPresenter();
            }
            
            int min = 1019;
            int current = 600 * Mathf.CeilToInt((count + (withBench? 1 : 0)) / 2f);
            int height = Mathf.Clamp(current, min, current);
            _rect.sizeDelta = new Vector2(1080,  height);
            float newHeight = _ScrollRect.rect.height > _rect.rect.height?  _ScrollRect.rect.height : _rect.rect.height;
            _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, -newHeight/ 2f);
            for (int i = craftEntities.Count + (withBench? 1 : 0); i < _views.Length; i++)
            {
                _views[i].SetBenchId(_craftService.GetBenchId(_currentType));
                _views[i].gameObject.SetActive(false);
                _views[i].ClearPresenter();
            }
            
            UpdateLineColor(_currentType);
        }

        
        private void UpdateColors()
        {
            _bench1Button.color = _colorBench1;
            _bench2Button.color = _colorBench2;
            _bottomLine.color = _colorBench1;
        }

        private void UpdateLineColor(BenchType bType)
        {
            _bench1Button.gameObject.SetActive(bType == BenchType.Workshop);
            _bench2Button.gameObject.SetActive(bType == BenchType.Recycling);
            _bottomLine.color = bType == BenchType.Workshop? _colorBench1 : _colorBench2;
        }
    }
}