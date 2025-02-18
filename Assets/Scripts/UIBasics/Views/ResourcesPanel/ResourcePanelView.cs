using System.Collections.Generic;
using System.Linq;
using Services;
using Services.ResourceVisualizeService;
using Services.Sounds;
using Services.Tasks;
using Services.Tutorial;
using Services.UIResourceAnimator;
using Static;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.ResourcesPanel
{
    public class ResourcePanelView : MonoBehaviour
    {
        [SerializeField]
        private ResourceTabView _oresTab;
        [SerializeField]
        private ResourceTabView _alloysTab;
        [SerializeField]
        private ResourceTabView _itemsTab;
        [SerializeField]
        private ResourceSellPanelView _resourceSellPanel;
        [SerializeField]
        private RectTransform _rowsHolder;
        [SerializeField]
        private Image _imageLine;
        [SerializeField]
        private RectTransform _buyButtonPosition;
        [SerializeField]
        private TutorialTaskUIFinger _tutorialTaskUIFinger;
        [SerializeField]
        private TutorialTaskUIFinger _secondFinger;


        private List<ResourceRowView> _rows;
        private ResourceRowView _selectedResource;
        private int _selectedResourcePrice;
        
        private PlayerResourcesService _playerResources;
        private TutorialService _tutorialService;
        private SettingsService _settingsService;
        private TaskService _taskService;
        private SoundService _soundsService;
        private UIResourceAnimatorService _uiResourceAnimatorService;
        
        private ResourceType _currentType;
        private Color _currentHeaderColor;
        
        [Inject]
        public void Init(TaskService taskService, PlayerResourcesService playerResources, SettingsService settingsService,
            SoundService soundService, TutorialService tutorialService, UIResourceAnimatorService uiResource)
        {
            _playerResources = playerResources;
            _settingsService = settingsService;
            _soundsService = soundService;
            _tutorialService = tutorialService;
            _uiResourceAnimatorService = uiResource;
            _taskService = taskService;
            
            
            _rows = transform.GetComponentsInChildren<ResourceRowView>().ToList();
            _playerResources.OnResourcesUpdated += ResourcesUpdatedHandler;
        }

        public void OnDestroy()
        {
            _playerResources.OnResourcesUpdated -= ResourcesUpdatedHandler;
        }

        public void Awake()
        {
            DeselectTabs();
            _currentType = ResourceType.Mined;
        }

        public void OnEnable()
        {
            SelectTab(_oresTab, false);
        }

        private void DeselectTabs()
        {
            _oresTab.UpdatedSelected(false);
            _alloysTab.UpdatedSelected(false);
            _itemsTab.UpdatedSelected(false);
        }

        public void SelectTab(ResourceTabView view, bool isClick = true)
        {
            _tutorialService.HalfStep();
            if (view.ResourceType != ResourceType.Mined && (TutorialStepNames)_tutorialService.TutorialStep < TutorialStepNames.Ability4Shown)
            {
                return;
            }
            if (_selectedResource != null)
            {
                _selectedResource.UpdatedSelected(false);
                _selectedResource = null;
            }
            
            if (isClick)
            {
                _soundsService.PlayClick();
            }
            _resourceSellPanel.UpdatePanel(false);
            DeselectTabs();
            view.UpdatedSelected(true);

            _currentHeaderColor = view.CurrentColor;
            _imageLine.color = _currentHeaderColor;
            _currentType = view.ResourceType;
            ResourcesUpdatedHandler(view.ResourceType);
            if (_rows[0].gameObject.activeSelf)
            {
                OnResourceChosen(_rows[0]);
            }
        }

        private void OnTabSelected(ResourceTabView view)
        {
            SelectTab(view, true);
        }

        private void ResourcesUpdatedHandler(ResourceType updatedType)
        {
            if (updatedType == _currentType)
            {
                UpdateResources();
            }
        }
        private void UpdateResources()
        {
            int startEmptyRowIndex = 0;
            if (_playerResources.ResourcesWithCount.TryGetValue(_currentType, out var resources))
            {
                for(int i = 0; i < resources.Count; i++)
                {
                    var current = resources[i];
                    ResourceRowView row = _rows[i];
                    row.SetHeaderColor(_currentHeaderColor);
                    if (row.Id != current.ResourceId || !row.gameObject.activeSelf)
                    {
                        row.SetConstantParameters(current.ResourceId);
                        
                        row.gameObject.SetActive(true);
                        
                        row.UpdatedSelected(false);
                    }
                
                    row.SetCount(current.Value);
                }

                startEmptyRowIndex = resources.Count;
            }
            _rowsHolder.sizeDelta = startEmptyRowIndex < _rows.Count ?new Vector2 (0, 807) : new Vector2 (0, 974);
            for (int i = startEmptyRowIndex; i < _rows.Count; i++)
            {
                _rows[i].gameObject.SetActive(false);
            }
        }
        
        public void OnResourceChosen(ResourceRowView resourceRowView)
        {
            _resourceSellPanel.UpdatePanel(true);
            if (_selectedResource != null)
            {
                _selectedResource.UpdatedSelected(false);
            }
            _selectedResource = resourceRowView;
            _selectedResource.UpdatedSelected(true);
            _selectedResourcePrice = _settingsService.GameResources[_selectedResource.Id].Price;
        }

        private void Update()
        {
            if (_selectedResource == null)
            {
                return;
            }

            var count = Mathf.Floor(_playerResources.GetResource(_selectedResource.Id) * _resourceSellPanel.Multiplier);
            var price = _selectedResourcePrice * count;
            _resourceSellPanel.UpdateResourceValue(count, price);
        }

        public void OnSellButtonClicked()
        {
            var count = (int)Mathf.Floor(_playerResources.GetResource(_selectedResource.Id) * _resourceSellPanel.Multiplier);
            if (count <= 0)
            {
                return;
            }
            _tutorialTaskUIFinger.OnClick();
            _secondFinger.OnClick();
            var price =_selectedResourcePrice * count;

            bool bought = _playerResources.TryBuy(new ResourceDemand(_selectedResource.Id, count));

            if (bought)
            {
                _playerResources.AddResource(ResourceNames.Soft, price);
                
                _uiResourceAnimatorService.Play(_buyButtonPosition.position, true);
                _soundsService.PlayCurrencySound();
                if (_selectedResource.Id == ResourceNames.Fireflies)
                {
                    _taskService.OnResourceSell(count, price);
                }
                _tutorialService.SellResources();
            }
        }
    }
}