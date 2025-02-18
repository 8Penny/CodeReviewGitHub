using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UIBasics
{
    public class UITutorialCircle : MonoBehaviour
    {
        [SerializeField]
        private Image _circle;
        [SerializeField]
        private RectTransform _circleRT;
        [SerializeField]
        private Color _color;
        
        private Color _toColor;
        
        private Sequence _sequence;

        private void Awake()
        {
            _toColor = new Color(_color.r, _color.g, _color.b, 0);
        }

        public void Play()
        {
            _sequence = DOTween.Sequence();
            _circleRT.localScale = Vector3.zero;
            _circle.color = _color;
            _circle.gameObject.SetActive(true);
            _sequence
                .Append(_circle.DOColor(_toColor, 0.5f))
                .Insert(0, _circleRT.DOScale(2, 0.5f).OnComplete(() =>
                {
                    _circle.gameObject.SetActive(false);
                }));
            _sequence.Play();
        }
    }
}