using System.Collections.Generic;
using Services;
using Services.Ads;
using Services.ResourceService;
using TMPro;
using UnityEngine;
using Zenject;

namespace UIBasics.Views.WelcomeBack
{
    public class WelcomeBackWindowView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _inactiveTime;
        [SerializeField]
        private ItemView _template;
        [SerializeField]
        private GameObject _main;
        [SerializeField]
        private GameObject _window;
        [SerializeField]
        private GameObject _adButton;
        [SerializeField]
        private GameObject _loading;
        [SerializeField]
        private RectTransform _itemsRect;

        private UIService _uiService;
        private AdsService _adsService;
        
        private SettingsService _settingsService;
        private  PlayerResourcesService _playerResourcesService;

        private ResourceService _resourceService;
        
        private List<ItemView> _items = new List<ItemView>();

        [Inject]
        public void Init(SettingsService settingsService, PlayerResourcesService playerResourcesService, UIService uiService,
            AdsService adsService, ResourceService resourceService)
        {
            _settingsService = settingsService;
            _playerResourcesService = playerResourcesService;
            _uiService = uiService;
            _adsService = adsService;
            _resourceService = resourceService;
            
            _template.gameObject.SetActive(false);
            
            _loading.gameObject.SetActive(adsService.IsAdsEnabled);
        }
        
        public void Show()
        {
            _main.SetActive(true);
            _window.SetActive(true);
            UpdateAdButton();
            
            _adsService.OnAdLoaded += UpdateAdButton;
        }

        private void UpdateAdButton()
        {
            _adButton.SetActive(_adsService.IsAdLoaded);
        }

        public void SetData(ResourcesHolder p, int seconds)
        {
            int num = 0;
            foreach (var resource in p.Values)
            {
                if (resource.Value < 1f)
                {
                    continue;
                }

                if (num + 1 > _items.Count)
                {
                    _items.Add(Instantiate(_template, _template.transform.parent));
                }
                Sprite bg = _resourceService.GetSprite(_settingsService.GameResources[resource.Key].ResourceType);
                _items[num].Init(_settingsService.GameResources[resource.Key], resource.Value,bg);
                _items[num].gameObject.SetActive(true);
                num += 1;
            }

            _itemsRect.sizeDelta = new Vector2(_itemsRect.sizeDelta.x, Mathf.Max(653, num*150));

            for (int i = num; i < _items.Count; i++)
            {
                _items[i].gameObject.SetActive(false);
            }
            
            _inactiveTime.text = $"Inactive time: {UiUtils.FormatedTime(seconds)}";
        }

        public void OnSimpleButtonClicked()
        {
            _playerResourcesService.TransferFreezeResources();
            _uiService.CloseWindow();
        }

        public void OnDoubleButtonClicked()
        {
            _adsService.ShowRewardedAd(RewardPlayer, "double_resources");
            _playerResourcesService.TransferFreezeResources(2);
            _uiService.CloseWindow();
        }

        private void RewardPlayer()
        {
            _playerResourcesService.TransferFreezeResources(2f);
        }
        
        public void Close()
        {
            if (_main.activeSelf)
            {
                _playerResourcesService.TransferFreezeResources();
            }
            _main.SetActive(false);
            _main.SetActive(false);

            _adsService.OnAdLoaded -= UpdateAdButton;
        }
    }
}