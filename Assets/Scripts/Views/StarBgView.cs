using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Views
{
    public class StarBgView : MonoBehaviour
    {
        private Transform _transform;
        private SpriteRenderer _spriteRenderer;
        private Sequence _tween;

        private Color _fadeColor;
        private Vector3 _startScale;
        private Vector3 _fadeScale;

        private float _startTweenTime;
        private float _startTweenDelta;
        private bool _started;
        
        private void Awake()
        {
            _transform = transform;
            _startScale = _transform.localScale;
            _fadeScale = _startScale * 0.2f;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _fadeColor = new Color(1f, 1f, 1f, Random.Range(0, 0.3f));

            bool isHidden = Random.Range(0, 2) == 0;
            if (!isHidden)
            {
                StartTween();
                _started = true;
                return;
            }
            
            _spriteRenderer.color = Color.clear;

            _startTweenDelta = Random.Range(1f, 3f);
            _startTweenTime = Time.time + _startTweenDelta;
        }

        private void Update()
        {
            if (_started)
            {
                return;
            }

            float ration = (_startTweenTime - Time.time) / _startTweenDelta;

            if (ration > 1f)
            {
                _spriteRenderer.color = Color.white;
                _started = true;
                StartTween();
                return;
            }

            _spriteRenderer.color = new Color(1f, 1f, 1f, ration);
        }

        private void StartTween()
        {
            float time = Random.Range(0.5f, 1f);
            float time2 = Random.Range(1f, 2f);

            _tween = DOTween.Sequence();
            _tween.SetAutoKill(false);
            _tween.AppendInterval(time)
                .Append(_spriteRenderer.DOColor(_fadeColor, time2))
                .Insert(time, _transform.DOScale(_fadeScale,time2))
                .AppendInterval(time)
                .Append(_spriteRenderer.DOColor(Color.white, time2))
                .Insert(2 * time + time2,_transform.DOScale(_startScale,time2))
                .OnComplete(()=>
            {
                _tween.Restart();
            });
            _tween.Play();
        }
    }
}