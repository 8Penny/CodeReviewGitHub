using System.Collections;
using Static;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Services.Ads
{
    public class AdFlagsController : MonoBehaviour
    {
        private int HARD_REWARD = 5;
        private int SOFT_REWARD = 70;
        private int SECONDS_INTER = 4;
        
        [SerializeField]
        private FlagsContainer _flagsContainer;
        [FormerlySerializedAs("_rewardedAdPopup")]
        [SerializeField]
        private RewardedPopupAd rewardedPopupAd;
        [SerializeField]
        private RewardPopup _rewardPopup;

        private AdsShowSystem _adsShowSystem;
        private AdsService _adsService;


        private bool _isShown;
        private bool _isFailed;

        [Inject]
        public void Init(AdsShowSystem adsShowSystem, AdsService adsService)
        {
            _adsShowSystem = adsShowSystem;
            _adsService = adsService;

            _adsShowSystem.ActivatedFlag += ActivateFlagHandler;
            _rewardPopup.gameObject.SetActive(false);
            rewardedPopupAd.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _adsShowSystem.ActivatedFlag -= ActivateFlagHandler;
        }

        private void ActivateFlagHandler(AdFlagType adFlagType)
        {
             _flagsContainer.Show(adFlagType);
             if (adFlagType == AdFlagType.Interstitial)
             {
                 _flagsContainer.SetText(SECONDS_INTER);
                 StartCoroutine(ProcessInterstitial());
             }
        }

        public void ShowRewardedAdPopUp()
        {
            rewardedPopupAd.SetText(HARD_REWARD.ToString());
            rewardedPopupAd.gameObject.SetActive(true);
        }

        public void ClickWatchAd()
        {
            _adsService.ShowRewardedAd(RewardedSuccess, "flag");
            _adsShowSystem.OnRewardedAdShown();
            rewardedPopupAd.Close();
            _adsShowSystem.ForceHide();
        }

        private void RewardedSuccess()
        {
            _rewardPopup.SetReward(ResourceNames.Hard, HARD_REWARD);
            _rewardPopup.gameObject.SetActive(true);
        }
        
        private void InterstitialSuccess()
        {
            _rewardPopup.SetReward(ResourceNames.Soft, SOFT_REWARD);
            _rewardPopup.gameObject.SetActive(true);
        }

        private void OnInterShown(bool isFailed)
        {
            _isShown = true;
            _isFailed = isFailed;
        }

        private IEnumerator ProcessInterstitial()
        {
            for (int i = 0; i < SECONDS_INTER; i++)
            {
                _flagsContainer.SetText(SECONDS_INTER - i);
                yield return new WaitForSeconds(1f);
            }
            _adsService.ShowInterstitialAd(OnInterShown, "flag");
            _adsShowSystem.ForceHide();

            while (!_isShown)
            {
                yield return null;
            }
            
            _isShown = false;

            if (!_isFailed)
            {
                InterstitialSuccess();
            }
            
        }
    }
}