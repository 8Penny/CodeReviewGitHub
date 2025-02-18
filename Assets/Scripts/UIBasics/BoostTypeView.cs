using System;
using Services;
using Services.Ads;
using Services.Boosts;
using Services.Tasks;
using Services.Tutorial;
using Settings;
using Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class BoostTypeView : MonoBehaviour
{
    private string BASE_PRICE_PREFIX = "base price ";

    [SerializeField]
    private BoostType _boostType;

    [SerializeField]
    private Image _image;

    [SerializeField]
    private Sprite _active;

    [SerializeField]
    private Sprite _inactive;

    [SerializeField]
    private Image _icon;

    [SerializeField]
    private TextMeshProUGUI _nameLabel;

    [SerializeField]
    private TextMeshProUGUI _descriptionLabel;

    [SerializeField]
    private TextMeshProUGUI _priceLabel;

    [SerializeField]
    private TextMeshProUGUI _basePriceLabel;

    [SerializeField]
    private GameObject _inactiveButton;

    [SerializeField]
    private GameObject _adPart;

    private AdsService _adsService;
    private SettingsService _settingsService;
    private TutorialService _tutorialService;
    private PlayerResourcesService _playerResourcesService;
    private UIService _uiService;
    
    private BoostService _boostService;
    private SingleBoost _boostSettings;
    private ResourceDemand _demand;
    private ResourceDemand _adDemand;
    private TutorialTaskService _tutorialTaskService;
    private bool _isAdPriceActive;
    private bool _wasBoostActive;

    public BoostType BoostType => _boostType;
    public bool IsAdPriceActive => _isAdPriceActive;

    [Inject]
    public void Init(SettingsService settingsService,
        PlayerResourcesService playerResourcesService,
        BoostService boostService,
        AdsService adsService,
        TutorialService tutorialService,
        TutorialTaskService tutorialTaskService,
        UIService uiService)
    {
        _uiService = uiService;
        _tutorialTaskService = tutorialTaskService;
        _settingsService = settingsService;
        _playerResourcesService = playerResourcesService;
        _boostService = boostService;
        _adsService = adsService;
        _tutorialService = tutorialService;
    }

    private void Awake()
    {
        _boostSettings = _settingsService.BoostsSettings.Boosts.Find(t => t.BoostType == _boostType);

        //_image.sprite = _boostSettings.Sprite;
        _descriptionLabel.text = _boostSettings.Description;
        _nameLabel.text = _boostSettings.Name;
        _priceLabel.text = UiUtils.GetCountableValue(_boostSettings.Price, 1);
        _basePriceLabel.text = BASE_PRICE_PREFIX + UiUtils.GetCountableValue(_boostSettings.Price, 1);
        _icon.sprite = _boostSettings.Sprite;
        _demand = new ResourceDemand(ResourceNames.Hard, _boostSettings.Price);
        _adDemand = new ResourceDemand(ResourceNames.Hard, _boostSettings.AdPrice);
    }

    private void OnEnable()
    {
        _playerResourcesService.OnResourcesUpdated += ResourcesChangedHandler;
        _tutorialService.OnNextTutorialStep += NextTutorialStepHandler;
        Setup();
    }

    private void OnDisable()
    {
        _playerResourcesService.OnResourcesUpdated -= ResourcesChangedHandler;
        _tutorialService.OnNextTutorialStep -= NextTutorialStepHandler;
    }

    private void ResourcesChangedHandler(ResourceType rt)
    {
        if (rt == ResourceType.None)
        {
            Setup();
        }
    }

    private void NextTutorialStepHandler(int _)
    {
        Setup();
    }

    private void Setup()
    {
        bool isBoostActive = _boostService.IsActive(_boostType);
        if (!isBoostActive)
        {
            bool isBaseDemand = _playerResourcesService.CanBuy(_demand);
            bool availableByTutorial = IsAvailable();
            bool canBuy = availableByTutorial &&
                          (isBaseDemand || _adsService.IsAdLoaded && _playerResourcesService.CanBuy(_adDemand));
            _image.sprite = canBuy ? _active : _inactive;
            _priceLabel.color = canBuy ? Color.white : StaticValues.InactiveTextColor;
            _isAdPriceActive = !isBaseDemand && canBuy && _adsService.IsAdLoaded;
            _priceLabel.text = _isAdPriceActive
                ? UiUtils.GetCountableValue(_boostSettings.AdPrice, 1)
                : UiUtils.GetCountableValue(_boostSettings.Price, 1);
            _adPart.SetActive(_isAdPriceActive);
        }
        else
        {
            _adPart.SetActive(false);
        }
    }

    public void OnButtonClicked()
    {
        if (_boostService.IsActive(_boostType))
        {
            return;
        }

        if (_isAdPriceActive)
        {
            _adsService.ShowRewardedAd(BuyBoostWithAdPrice, "boost");
            return;
        }


        if (!IsAvailable())
        {
            return;
        }

        if (_playerResourcesService.TryBuy(_demand))
        {
            _boostService.ActivateBoost(_boostType);
            if (_boostType == BoostType.CraftBoost)
            {
                _tutorialService.ReturnToAbilityShownAfter2Boost();
            }
        }
        else
        {
            _uiService.OpenGoToShopPopup(false);
        }
    }

    private bool IsAvailable()
    {
        bool tutorialStepAvailable =
            (TutorialStepNames) _tutorialService.TutorialStep is TutorialStepNames.TakenEmeralds &&
            _boostType == BoostType.CastleBoost ||
            (TutorialStepNames) _tutorialService.TutorialStep is TutorialStepNames.TakenEmeralds2 &&
            _boostType == BoostType.CraftBoost;
        bool availableByTutorial = _tutorialService.IsComplete || tutorialStepAvailable;
        return availableByTutorial;
    }

    private void BuyBoostWithAdPrice()
    {
        if (_playerResourcesService.TryBuy(_adDemand))
        {
            _boostService.ActivateBoost(_boostType);
        }
    }

    public void Update()
    {
        bool isBoostActive = _boostService.IsActive(_boostType);
        _inactiveButton.SetActive(isBoostActive);
        if (!isBoostActive && _wasBoostActive)
        {
            Setup();
        }

        _wasBoostActive = isBoostActive;
    }
}