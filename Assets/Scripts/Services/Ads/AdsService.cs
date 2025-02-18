using System;
using CAS;
using Services.Analytics;
using Static;
using MobileAds = CAS.MobileAds;

namespace Services.Ads
{
    public class AdsService
    {

        private IMediationManager _manager;

        private AnalyticsService _analyticsService;
        private ShopService.ShopService _shopService;

        private Action _onRewardedAdShown;
        private Action<bool> _onInterstitialAdShown;
        
        private bool _inProcess;
        private string _source;

        
        public Action OnAdLoaded;
        
        
        public bool IsInProcess => _inProcess;
        public bool IsAdsEnabled => _shopService.IaAdsActive;
        public bool IsAdLoaded => IsAdsEnabled && _manager != null && _manager.IsReadyAd(AdType.Rewarded);

        public bool IsInterLoaded => IsAdsEnabled && _manager != null && _manager.IsReadyAd(AdType.Interstitial);

        public AdsService(AnalyticsService analyticsService, ShopService.ShopService shopService)
        {
            _shopService = shopService;
            _analyticsService = analyticsService;
        }
        
        public void Init()
        {
            if (StaticValues.IsDevelopADS)
            {
                CAS.MobileAds.settings.SetTestDeviceIds(new[]
                {
                    "12345678901234567890123456789012"
                });
            }

            _manager = GetAdManager();

            SubscribeInter();
            SubscribeRewarded();

        }
        
        public static IMediationManager GetAdManager()
        {
            // Configure MobileAds.settings before initialize
            return MobileAds.BuildManager()
                // Optional initialize listener
                .WithCompletionListener((config) => {
                    string initErrorOrNull = config.error;
                    string userCountryISO2OrNull = config.countryCode;
                    bool protectionApplied = config.isConsentRequired;
                    IMediationManager manager = config.manager;
                })
                .Build();
        }


        private void SubscribeInter()
        {
            _manager.OnInterstitialAdLoaded += () => OnAdLoaded?.Invoke();
            _manager.OnInterstitialAdFailedToLoad += (error) => _analyticsService.AdFailHandler("failed_load", $"INTER: {error.GetMessage()}");

            _manager.OnInterstitialAdOpening += (data) => _analyticsService.AdHandler("ad_impression", "inter", _source);

            _manager.OnInterstitialAdFailedToShow += (error) =>
            {
                _analyticsService.AdHandler("inter_ad_fail", $"{error}", _source);
                _inProcess = false;
                _onInterstitialAdShown?.Invoke(true);
            };

            _manager.OnInterstitialAdClicked += () => _analyticsService.AdHandler("ad_click", "inter", _source);

            _manager.OnInterstitialAdClosed += () =>
            {
                _inProcess = false;
                _analyticsService.AdHandler("ad_close", "inter", _source);
                _onInterstitialAdShown?.Invoke(false);
            };
        }
        private void SubscribeRewarded()
        {
            _manager.OnRewardedAdLoaded += () => OnAdLoaded?.Invoke();
            _manager.OnRewardedAdFailedToLoad += (error) => _analyticsService.AdFailHandler("failed_load", $"REWARDED: {error.GetMessage()}");
            _manager.OnRewardedAdOpening += (data) => _analyticsService.AdHandler("ad_open", "rewarded", _source);
            _manager.OnRewardedAdFailedToShow += (error) =>
            {
                _analyticsService.AdHandler("rewarded_ad_fail", $"{error}", _source);
                _inProcess = false;
            };
            _manager.OnRewardedAdClicked += () => _analyticsService.AdHandler("ad_click", "rewarded", _source);
            _manager.OnRewardedAdCompleted += () =>
            {
                RewardedAdShowHandler();
            };
            _manager.OnRewardedAdClosed += () =>
            {
                _analyticsService.AdHandler("ad_close", "rewarded", _source);
                _inProcess = false;
            };
        }
        
        
        
        public void ShowInterstitialAd(Action<bool> onInterstitialAdShown, string source)
        {
            if (!IsInterLoaded)
            {
                return;
            }
            _source = source;
            _onInterstitialAdShown = onInterstitialAdShown;
            _inProcess = true;
            _manager.ShowAd(AdType.Interstitial);
            _analyticsService.AdHandler("open","inter", _source);
            
        }

        public void ShowRewardedAd(Action onRewardedAdShown, string source)
        {
            if (!IsAdLoaded)
            {
                return;
            }

            _source = source;
            _onRewardedAdShown = onRewardedAdShown;
            _inProcess = true;
            _manager.ShowAd(AdType.Rewarded);
            _analyticsService.AdHandler("open","rewarded", _source);
            
        }

        private void RewardedAdShowHandler()
        {
            _inProcess = false;
            _analyticsService.AdHandler("success","","rewarded");
            _onRewardedAdShown?.Invoke();
        }
        
    }
}