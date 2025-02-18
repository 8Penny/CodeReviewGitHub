using Services;
using Services.ResourceService;
using Settings;
using Static;
using TMPro;
using UIBasics.Views.ResourcesPanel;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.AbilityPanel
{
    public class ResourceDemandView : MonoBehaviour
    {
        private static Color HAS_COLOR = Color.green;
        private static Color HASNT_COLOR = Color.red;
        
        [SerializeField]
        private Image _bg;
        [SerializeField]
        private TextMeshProUGUI _itemName;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TextMeshProUGUI _count;

        private PlayerResourcesService _resourcesService;
        private SettingsService _settingsService;
        private ResourceService _resourceService;

        private Resource _currentResource;
        private ResourceNames _currentId;
        private int _demandCount;

        [Inject]
        public void Init(PlayerResourcesService resourcesService, SettingsService settingsService, ResourceService resourceService)
        {
            _resourcesService = resourcesService;
            _settingsService = settingsService;
            _resourceService = resourceService;
        }

        private void OnEnable()
        {
            _resourcesService.OnResourcesUpdated += ResourceUpdatedHandler;
        }
        private void OnDisable()
        {
            _resourcesService.OnResourcesUpdated -= ResourceUpdatedHandler;
        }

        private void ResourceUpdatedHandler(ResourceType resourceType)
        {
            if (_currentResource != null && resourceType == _currentResource.ResourceType)
            {
                UpdateResource();
            }
        }

        public void SetResource(ResourceNames id, int demandCount)
        {
            _currentId = id;
            _demandCount = demandCount;
            _currentResource = _settingsService.GameResources[id];
            Sprite sprite = _resourceService.GetSprite(_currentResource.ResourceType);
            if (sprite == null)
            {
                _bg.gameObject.SetActive(false);
            }
            else
            {
                _bg.gameObject.SetActive(true);
                _bg.sprite = sprite;
            }
            _itemName.text = StaticNames.Get(_currentResource.ResourceId);
            _image.sprite = _currentResource.Sprite;

            UpdateResource();
        }

        private void UpdateResource()
        {
            var inPocketValue = _resourcesService.GetResource(_currentId);
            SetCount(inPocketValue);
        }

        private void SetCount(float playerHas)
        {
            _count.text = $"{UiUtils.GetCountableValue(playerHas)}/{UiUtils.GetCountableValue(_demandCount)}";
            _count.color = playerHas >= _demandCount ? HAS_COLOR : HASNT_COLOR;
        }
    }
}