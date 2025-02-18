using System;
using Services;
using Services.ShopService;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.ShopWindow
{
    public class ShopCurrencyItem : MonoBehaviour
    {
        [SerializeField]
        private string _productId;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TextMeshProUGUI _count;
        [SerializeField]
        private Text _price;

        private SettingsService _settingsService;
        private ShopService _shopService;

        [Inject]
        public void Init(SettingsService settingsService, ShopService shopService)
        {
            _shopService = shopService;
            _settingsService = settingsService;
        }
        
        private void Awake()
        {
            _image.sprite = _settingsService.ShopSettings.Slots[_productId].Icon;

            if (_shopService.TryGetProduct(_productId, out var demands))
            {
                _count.text = UiUtils.GetCountableValue(demands[0].Value);
            }
        }

        private void OnEnable()
        {
            _shopService.OnUpdated += ShopDataUpdatedHandler;
            ShopDataUpdatedHandler();
        }

        private void OnDisable()
        {
            _shopService.OnUpdated -= ShopDataUpdatedHandler;
        }

        private void ShopDataUpdatedHandler()
        {
            _price.text = _shopService.GetPrice(_productId);
        }

        public void TryBuy()
        {
            _shopService.Purchase(_productId);
        }
    }
}