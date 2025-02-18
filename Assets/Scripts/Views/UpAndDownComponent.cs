using System;
using DG.Tweening;
using UnityEngine;

namespace Views
{
    public class UpAndDownComponent : MonoBehaviour
    {
        [SerializeField]
        private Transform _transform;
        [SerializeField]
        private Vector3 _endPosition;
        [SerializeField]
        private Vector3 _startPosition;
        [SerializeField]
        private float _speed = 0.4f;

        private Sequence _sequence;
        private void Awake()
        {
            _sequence = DOTween.Sequence();
            _sequence.SetAutoKill(false);
            _transform.localPosition = _endPosition;
            _sequence.Append(_transform.DOLocalMove(_startPosition, _speed).SetEase(Ease.InOutQuad))
                .Append(_transform.DOLocalMove(_endPosition, _speed).SetEase(Ease.InOutQuad))
                .OnComplete(()=>_sequence.Restart())
                .Play();
        }
    }
}