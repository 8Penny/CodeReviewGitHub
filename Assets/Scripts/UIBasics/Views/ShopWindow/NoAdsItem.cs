using System;
using Services;
using Services.ShopService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using Zenject;

namespace UIBasics.Views.ShopWindow
{
    public class NoAdsItem : MonoBehaviour
    {
        [SerializeField]
        private Button _mainButton;
        [SerializeField]
        private GameObject _content;
        [SerializeField]
        private string _productId;
        [SerializeField]
        private Text _price;
        
        
        private SettingsService _settingsService;
        private ShopService _shopService;
        

        [Inject]
        public void Init(ShopService shopService, SettingsService settingsService)
        {
            _shopService = shopService;
            _settingsService = settingsService;
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
            _mainButton.interactable = _shopService.IaAdsActive;
        }

        public void TryBuy()
        {
            _shopService.Purchase(_productId);
        }
    }
}