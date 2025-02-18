
using Services;
using Services.ResourceService;
using Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.Recipes
{
    public class RecipeItemView : MonoBehaviour
    {
        [SerializeField]
        private bool _needChangeBG;
        [SerializeField]
        private Image _bg;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TextMeshProUGUI _countLabel;
        [SerializeField]
        private CanvasGroup _canvasGroup;

        private SettingsService _settingsService;
        private ResourceService _resourceService;

        [Inject]
        public void Init(SettingsService settingsService, ResourceService resourceService)
        {
            _settingsService = settingsService;
            _resourceService = resourceService;
        }
        
        public void SetItem(bool isDemand, int count, ResourceNames resourceId)
        {
            _image.sprite = _settingsService.GameResources[resourceId].Sprite;
            if (_needChangeBG)
            {
                _bg.sprite = _resourceService.GetSprite(_settingsService.GameResources[resourceId].ResourceType);
            }
            _countLabel.text = isDemand ? UiUtils.GetCountableValue(count) : "";
        }

        public void UpdateVisibility(bool isVisible)
        {
            _canvasGroup.alpha = isVisible ? 1 : 0;
        }

        public void UpdateCount(bool isDemand, float inPlayerPocketCount, float needCount)
        {
            _countLabel.text = isDemand ? $"{UiUtils.GetCountableValue(inPlayerPocketCount)}/{UiUtils.GetCountableValue(needCount)}" : UiUtils.GetCountableValue(inPlayerPocketCount);
        }
    }
}