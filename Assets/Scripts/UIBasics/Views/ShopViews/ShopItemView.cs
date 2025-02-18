using System;
using Services;
using Services.ShopService;
using Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using Zenject;

namespace UIBasics.Views.ShopViews
{
    public class ShopItemView : MonoBehaviour
    {
        [SerializeField]
        private string _productId;
        [SerializeField]
        private Text _price;
        [SerializeField]
        private TextMeshProUGUI _currencyReward;
        // [SerializeField]
        // private Image _bg;

        [SerializeField]
        private GameObject[] _textIconSpaces;
        [SerializeField]
        private Image[] _icons;
        [SerializeField]
        private TextMeshProUGUI[] _texts;
        
        private SettingsService _settingsService;
        private ShopService _shopService;

        [Inject]
        public void Init(ShopService shopService, SettingsService settingsService)
        {
            _shopService = shopService;
            _settingsService = settingsService;

        }

        private void FillIn()
        {
            if (!_shopService.TryGetProduct(_productId, out var demands))
            {
                Debug.LogError($"No product with id {_productId}");
                return;
            }
            //
            // _bg.sprite = _settingsService.ShopSettings.Slots[_iapButton.productId].Icon;
            UiUtils.UpdateRewards(_settingsService, demands, _currencyReward, _textIconSpaces, _icons, _texts);
        }

        private void Awake()
        {
            _shopService.OnUpdated += Setup;
            FillIn();
            Setup();
        }

        private void OnDestroy()
        {
            _shopService.OnUpdated -= Setup;
        }

        private void Setup()
        {
            _price.text = _shopService.GetPrice(_productId);
        }

        public void TryBuy()
        {
            _shopService.Purchase(_productId);
        }

    }
}