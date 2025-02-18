using System;
using Services.Tutorial;
using Services.Updater;
using UnityEngine;

namespace Services.Ads
{
    public class AdsShowSystem : IUpdatable, IDisposable
    {
        private string AD_STEP_ID = "AD_STEP_ID";
        private string TIME_TO_NEXT_AD = "TIME_TO_NEXT_AD";
        private string SESSION_COUNT = "SESSION_COUNT";
        private string SESSION_LAST_TIME = "SESSION_LAST_TIME";
        private int SESSION_TIME = 5 * 60;
        
        private int CHECK_INTERVAL = 2;
        private int DECISION_COOLDOWN = 90;
        private int APPEARING_SECONDS_COOLDOWN = 7;
        private int INTERSTITIAL_COOLDOWN = 7;
        
        private AdsService _ads;
        private PlayerDataManager _dataManager;
        private TutorialService _tutorialService;
        private UpdateService _updateService;

        private bool _isEnabled => _tutorialService.TutorialStep >= (int)TutorialStepNames.UpgradedAllCastleParametersTasks;//&& _sessionCount > 2;
        private int _timeToNextAd;
        private int _timeToHideFlag;
        private int _adStepId;
        private int _rewardsBannersShowCount;
        private int _sessionCount;
        private float _sessionLastTime;

        private float _nextCheckTime;
        private bool _isActive;

        public Action<AdFlagType> ActivatedFlag;
        public bool IsActive => _isActive;
        
        public AdsShowSystem(AdsService ads, PlayerDataManager dataManager, TutorialService tutorialService, UpdateService updateService)
        {
            _tutorialService = tutorialService;
            _ads = ads;
            _dataManager = dataManager;
            _updateService = updateService;
            
            _dataManager.OnPreSaveStep += SaveParameters;
            
            LoadParameters();
            _updateService.Register(this);
        }

        public void Dispose()
        {
            _updateService.Unregister(this);
            _dataManager.OnPreSaveStep -= SaveParameters;
        }

        private void LoadParameters()
        {
            _adStepId = PlayerPrefs.GetInt(AD_STEP_ID, 0);
            _timeToNextAd = PlayerPrefs.GetInt(TIME_TO_NEXT_AD, 10);
        }

        public void UpdateSessionParameters()
        {
            if (!_tutorialService.IsComplete)
            {
                return;
            }
            
            if (!PlayerPrefs.HasKey(SESSION_COUNT))
            {
                _sessionCount = 1;
                _sessionLastTime = CommonUtils.UnixTime();
                PlayerPrefs.SetInt(SESSION_COUNT, _sessionCount);
                PlayerPrefs.SetFloat(SESSION_LAST_TIME, _sessionLastTime);
                return;
            }
            
            _sessionCount = PlayerPrefs.GetInt(SESSION_COUNT);
            _sessionLastTime = PlayerPrefs.GetFloat(SESSION_LAST_TIME);
            float currentTime = CommonUtils.UnixTime();
            if (currentTime - _sessionLastTime > SESSION_TIME)
            {
                _sessionCount += 1;
                PlayerPrefs.SetInt(SESSION_COUNT, _sessionCount);
                PlayerPrefs.SetFloat(SESSION_LAST_TIME, currentTime);
            }
        }

        private void SaveParameters()
        {
            PlayerPrefs.SetInt(TIME_TO_NEXT_AD, _timeToNextAd);
            PlayerPrefs.SetInt(AD_STEP_ID, _adStepId);
        }

        public void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            if (_isActive && !_ads.IsAdsEnabled)
            {
                ForceHide();
            }

            if (Time.time < _nextCheckTime)
            {
                return;
            }
            _nextCheckTime = Time.time + CHECK_INTERVAL;
            
            if (_isActive)
            {
                ProcessActive();
                return;
            }

            if (_ads.IsInProcess)
            {
                return;
            }

            if (_timeToNextAd > 0)
            {
                _timeToNextAd -= CHECK_INTERVAL;
                return;
            }
            ChooseAdType();
        }

        private void ProcessActive()
        {
            if (_timeToHideFlag > 0)
            {
                _timeToHideFlag -= CHECK_INTERVAL;
                return;
            }

            _isActive = false;
            ActivatedFlag?.Invoke(AdFlagType.None);
        }

        public void ForceHide()
        {
            _isActive = false;
            ActivatedFlag?.Invoke(AdFlagType.None);
        }

        private void ChooseAdType()
        {
            if (_adStepId < 2 && _ads.IsAdLoaded)
            {
                _timeToHideFlag = APPEARING_SECONDS_COOLDOWN;
                ActivatedFlag?.Invoke(AdFlagType.Interstitial);//AdFlagType.Rewarded);
                _adStepId += 1;
            }
            else if (_adStepId >= 2 && _ads.IsInterLoaded)
            {
                _timeToHideFlag = INTERSTITIAL_COOLDOWN;
                ActivatedFlag?.Invoke(AdFlagType.Interstitial);
                _adStepId = 0;
            }
            else
            {
                return;
            }
            
            _isActive = true;
            _timeToNextAd = DECISION_COOLDOWN;
        }

        public void OnRewardedAdShown()
        {
            _adStepId -= 1;
        }
    }
}