using System.Collections.Generic;
using Services;
using Services.ShopService;
using UnityEngine;
using Zenject;

namespace UIBasics.Views.ShopWindow
{
    public class ShopWindowView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _main;
        [SerializeField]
        private GameObject _window;

        [SerializeField]
        private GameObject _failed;
        [SerializeField]
        private SuccessPanel _success;

        private ShopService _shopService;
        private SettingsService _settingsService;

        [Inject]
        public void Init(ShopService shopService, SettingsService settingsService)
        {
            _shopService = shopService;
            _settingsService = settingsService;

            _shopService.OnSuccess += ShowSuccessPanel;
            _shopService.OnFailed += ShowFailPanel;
        }
        
        public void Show()
        {
            _main.SetActive(true);
            _window.SetActive(true);
            ClosePanels();
        }
        
        public void Close()
        {
            _main.SetActive(false);
            _window.SetActive(false);
            ClosePanels();
        }

        private void ShowSuccessPanel(List<ResourceDemand> demands)
        {
            _success.transform.parent.gameObject.SetActive(true);
            _success.SetReward(demands, _settingsService);
        }

        private void ShowFailPanel()
        {
            _failed.gameObject.SetActive(false);
        }

        public void ClosePanels()
        {
            _failed.SetActive(false);
            _success.transform.parent.gameObject.SetActive(false);
        }
    }
}