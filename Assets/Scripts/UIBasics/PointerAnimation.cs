using DG.Tweening;
using UnityEngine;

namespace UIBasics
{
    public class PointerAnimation : MonoBehaviour
    {
        
        [SerializeField]
        public int _sign = 1;

        private bool _isPlaying;
        private Sequence _sequence;
        private RectTransform _rect;
        
        private Vector2 _endPosition;
        private Vector2 _startPosition;

        private void Awake()
        {
            
            _rect = GetComponent<RectTransform>();
            _endPosition = _rect.anchoredPosition;
            _startPosition = _endPosition + new Vector2(0, _sign * 40f);
        }

        private void OnEnable()
        {
            Setup();
        }

        private void OnDisable()
        {
            if (_isPlaying)
            {
                _isPlaying = false;
                _sequence.Kill();
            }
        }
        private void Setup()
        {
            if (!gameObject.activeInHierarchy || _isPlaying)
            {
                return;
            }
            float seconds = 0.4f;
            _sequence = DOTween.Sequence();
            _sequence.SetAutoKill(false);

            _rect.anchoredPosition = _startPosition;
            _sequence.Append(_rect.DOAnchorPos(_endPosition, seconds/1.2f).SetEase(Ease.InOutQuad))
                .Append(_rect.DOAnchorPos(_startPosition, seconds).SetEase(Ease.InOutQuad))
                .OnComplete(()=>_sequence.Restart())
                .Play();
                
            _isPlaying = true;
        }
    }
}