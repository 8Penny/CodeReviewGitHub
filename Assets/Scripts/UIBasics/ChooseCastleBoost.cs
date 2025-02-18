using Services;
using Services.Ads;
using Services.Boosts;
using Services.Tasks;
using Services.Tutorial;
using Static;
using UnityEngine;
using Zenject;


    public class ChooseCastleBoost : MonoBehaviour
    {
        private BoostService _boostService;
        private UIService _uiService;
        private PlayerResourcesService _playerResourcesService;
        private SettingsService _settingsService;
        private AdsService _adsService;
        private TutorialService _tutorialService;

        private ResourceDemand _demand;
        private ResourceDemand _adDemand;

        private int _castleId;
        private bool _isAd;
        
        [Inject]
        public void Init(BoostService boostService, UIService uiService,
            PlayerResourcesService playerResourcesService, SettingsService settingsService, AdsService adsService,
                TutorialService tutorialService)
        {
            _tutorialService = tutorialService;
            _boostService = boostService;
            _uiService = uiService;
            _playerResourcesService = playerResourcesService;
            _settingsService = settingsService;
            _adsService = adsService;
        }

        public void Awake()
        {
            var settings = _settingsService.BoostsSettings.Boosts.Find(t => t.BoostType == BoostType.CastleBoost);
            _demand = new ResourceDemand(ResourceNames.Hard, settings.Price);
            _adDemand = new ResourceDemand(ResourceNames.Hard, settings.AdPrice);

            gameObject.SetActive(false);
        }

        public void OnCloseClicked()
        {
            gameObject.SetActive(false);
            _uiService.OnChooseCastleUpdated(false);
        }

        public void ShowChooseCastle(BoostTypeView boostTypeView)
        {
            ResourceDemand current = boostTypeView.IsAdPriceActive ? _adDemand : _demand;
            _isAd = boostTypeView.IsAdPriceActive;

            if (!_playerResourcesService.CanBuy(current))
            {
                _uiService.OpenGoToShopPopup();
                return;
            }
            
            _tutorialService.ActivateChoosingCastleStep();

            _uiService.CloseWindow();
            _uiService.CloseMainPanel();
            gameObject.SetActive(true);
            _uiService.OnChooseCastleUpdated(true);
        }

        public void ChooseCastle(int id)
        {
            _castleId = id;
            if (_isAd)
            {
                _adsService.ShowRewardedAd(ActivateBoost, "boost");
            }
            else
            {
                if (_playerResourcesService.TryBuy(_demand))
                {
                    
                    _tutorialService.FinishChooseCastleStep();
                    _boostService.ActivateBoost(BoostType.CastleBoost, _castleId);
                }

                OnCloseClicked();
            }
        }
        
        private void ActivateBoost()
        {
            if (_playerResourcesService.TryBuy(_adDemand))
            {
                _boostService.ActivateBoost(BoostType.CastleBoost, _castleId);
            }

            OnCloseClicked();
        }
    }
