using System;
using System.Collections.Generic;
using Static;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Services.ShopService
{
    public class ShopService
    {
        private PurchaseService _purchaseService;
        private PlayerResourcesService _playerResourcesService;
        
        private Dictionary<string, ResourceDemand> _shopProducts = new Dictionary<string, ResourceDemand>()
        {
            {"catsking.idle.hard.slot1", new ResourceDemand(ResourceNames.Hard, 100)},
            {"com.catsking.idle.soft.slot1", new ResourceDemand(ResourceNames.Soft, 100000)},
            
            {"cats.king.hard.slot.1", new ResourceDemand(ResourceNames.Hard, 25)},
            {"cats.king.hard.slot.2", new ResourceDemand(ResourceNames.Hard, 70)},
            {"cats.king.hard.slot.3", new ResourceDemand(ResourceNames.Hard, 150)},
            
            {"cats.king.soft.slot.1", new ResourceDemand(ResourceNames.Soft, 50000)},
            {"cats.king.soft.slot.2", new ResourceDemand(ResourceNames.Soft, 300000)},
            {"cats.king.soft.slot.3", new ResourceDemand(ResourceNames.Soft, 1500000)},
        };
        private Dictionary<string, List<ResourceDemand>> _shopProductsList = new Dictionary<string, List<ResourceDemand>>()
        {
            {"cats.king.starter.slot.1", new List<ResourceDemand>()
            {
                new ResourceDemand(ResourceNames.Soft, 100000),
                new ResourceDemand(ResourceNames.Hard, 25),
            }},
            {"cats.king.starter.slot.2", new List<ResourceDemand>()
            {
                new ResourceDemand(ResourceNames.Soft, 140000),
                new ResourceDemand(ResourceNames.Hard, 40),
                new ResourceDemand(ResourceNames.Fireflies, 100000),
            }},
            {"cats.king.starter.slot.3", new List<ResourceDemand>()
            {
                new ResourceDemand(ResourceNames.Soft, 200000),
                new ResourceDemand(ResourceNames.Hard, 50),
                new ResourceDemand(ResourceNames.DuckTape, 25),
                new ResourceDemand(ResourceNames.ShamansStick, 5),
            }},
        };
        
        public Dictionary<string, ResourceDemand> ShopProducts => _shopProducts;
        public Dictionary<string, List<ResourceDemand>> ShopProductsList => _shopProductsList;
        public Action OnUpdated;
        public Action<List<ResourceDemand>> OnSuccess;
        public Action OnFailed;

        private bool _isAdsActive = true;
        public bool IaAdsActive => _isAdsActive;
        public bool IsInitialized => (_purchaseService != null) && _purchaseService.IsInitialized();

        public ShopService(PlayerResourcesService playerResourcesService)
        {
            _playerResourcesService = playerResourcesService;

            _isAdsActive = !PlayerPrefs.HasKey(StaticValues.NO_ADS);
        }

        public void Register(PurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
            _purchaseService.Initialized += PurchaseServiceInitializedHandler;
        }

        private void PurchaseServiceInitializedHandler()
        {
            bool hasRecipe = _purchaseService.HasRecipe(PurchaseService.NO_ADS, out bool isInit);
            _isAdsActive = isInit ? !hasRecipe : _isAdsActive;
            OnUpdated?.Invoke();
        }

        public bool TryGetProduct(string key, out List<ResourceDemand> demands)
        {
            bool isSuccess = _shopProductsList.TryGetValue(key, out demands);
            if (!isSuccess)
            {
                isSuccess = _shopProducts.TryGetValue(key, out var demand);
                demands = new List<ResourceDemand>() {demand};
            }

            return isSuccess;
        }

        public void PurchaseSuccess(string id)
        {
            if (!TryGetProduct(id, out var demands))
            {
                return;
            }

            foreach (var d in demands)
            {
                _playerResourcesService.AddResource(d.ResourceId, d.Value);
            }
            OnSuccess?.Invoke(demands);
            OnUpdated?.Invoke();
        }

        public void PurchaseADSComplete()
        {
            PlayerPrefs.SetInt(StaticValues.NO_ADS, 1);
            _isAdsActive = false;
            OnUpdated?.Invoke();
        }
        
        public void PurchaseFailed()
        {
            OnFailed?.Invoke();
        }

        public void Purchase(string id)
        {
            if (_purchaseService == null)
            {
                Debug.LogError("Theres no purchase service");
                return;
            }
            _purchaseService.BuyProductID(id);
        }
        public string GetPrice(string id)
        {
            if (_purchaseService == null)
            {
                Debug.LogError("Theres no purchase service");
                return "";
            }
            return _purchaseService.GetProductPrice(id);
        }
    }
}