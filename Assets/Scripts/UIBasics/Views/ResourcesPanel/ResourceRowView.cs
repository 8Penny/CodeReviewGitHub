using Services;
using Static;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.ResourcesPanel
{
    public class ResourceRowView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private GameObject _simple;
        [SerializeField]
        private GameObject _selected;
        [SerializeField]
        private TextMeshProUGUI _nameLabel;
        [SerializeField]
        private TextMeshProUGUI _countLabel;
        [SerializeField]
        private TextMeshProUGUI _priceLabel;
        [SerializeField]
        private Image _headerImage;
        
        private SettingsService _settingsService;
        private UIService _uiService;
        private ResourceNames _id;
        public ResourceNames Id => _id;

        [Inject]
        public void Init(SettingsService settingsService, UIService uiService)
        {
            _settingsService = settingsService;
            _uiService = uiService;
        }

        public void SetConstantParameters(ResourceNames id)
        {
            _id = id;

            var current = _settingsService.GameResources[_id];
            _nameLabel.text =StaticNames.Get( _id);
            _icon.sprite = current.Sprite;
            _priceLabel.text = UiUtils.GetCountableValue(current.Price, 0);
            UpdatedSelected(false);
        }

        public void SetCount(int count)
        {
            _countLabel.text = UiUtils.GetCountableValue(count);
        }

        public void UpdatedSelected(bool isSelected)
        {
            _selected.SetActive(isSelected);
            _simple.SetActive(!isSelected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _uiService.Views.ResourcePanelView.OnResourceChosen(this);
        }

        public void SetHeaderColor(Color color)
        {
            _headerImage.color = color;
        }
    }
}