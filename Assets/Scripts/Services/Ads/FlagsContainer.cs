using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Services.Ads
{
    public class FlagsContainer : MonoBehaviour
    {
        [SerializeField]
        private GameObject _rewardedFlag;
        [SerializeField]
        private GameObject _interstitialFlag;
        [SerializeField]
        private RectTransform _startPosition;
        [SerializeField]
        private TextMeshProUGUI _text;

        private RectTransform _rewardedRectTransform;
        private RectTransform _interstitialRectTransform;

        private Vector2 _rewardedPosition;
        private Vector2 _interstitialPosition;

        private void Awake()
        {
            _interstitialFlag.SetActive(false);
            _rewardedFlag.SetActive(false);

            _interstitialRectTransform = _interstitialFlag.GetComponent<RectTransform>();
            _rewardedRectTransform = _rewardedFlag.GetComponent<RectTransform>();

            _rewardedPosition = _rewardedRectTransform.anchoredPosition;
            _interstitialPosition = _interstitialRectTransform.anchoredPosition;
        }

        public void Show(AdFlagType type)
        {
            switch (type)
            {
                case AdFlagType.None:
                    SlowMove();
                    break;
                case AdFlagType.Rewarded:
                    _rewardedRectTransform.anchoredPosition = _startPosition.anchoredPosition;
                    _rewardedFlag.SetActive(true);
                    _rewardedRectTransform.DOAnchorPos(_rewardedPosition, 1f).SetEase(Ease.OutQuad);
                    _interstitialFlag.SetActive(false);
                    break;
                case AdFlagType.Interstitial:
                    _interstitialRectTransform.anchoredPosition = _startPosition.anchoredPosition;
                    _rewardedFlag.SetActive(false);
                    _interstitialRectTransform.DOAnchorPos(_interstitialPosition, 1f).SetEase(Ease.OutQuad);
                    _interstitialFlag.SetActive(true);
                    break;
            }
        }

        private void SlowMove()
        {
            if (_interstitialFlag.activeSelf)
            {
                _interstitialRectTransform.DOAnchorPos(_startPosition.anchoredPosition, 0.6f).SetEase(Ease.OutQuad).OnComplete(()=>_interstitialFlag.SetActive(false));
            }
            if (_rewardedFlag.activeSelf)
            {
                _rewardedRectTransform.DOAnchorPos(_startPosition.anchoredPosition, 0.6f).SetEase(Ease.OutQuad).OnComplete(()=>_rewardedFlag.SetActive(false));
            }
        }

        public void SetText(int secondsLeft)
        {
            _text.text = $":{secondsLeft}";
        }
    }
}