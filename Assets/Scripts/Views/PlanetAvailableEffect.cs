using System;
using DG.Tweening;
using UnityEngine;

namespace Views
{
    public class PlanetAvailableEffect : MonoBehaviour
    {
        private float FADE_TIME = 1.7f;
        private Vector3 START_ROTATION = new Vector3(-90, 0, 0);
        private Vector3 END_ROTATION = new Vector3(0, 0, 180);

        private SpriteRenderer _spriteRenderer;
        private Sequence _sequence;

        public bool IsPlaying { get; private set; }

        private void Awake()
        {
            transform.localScale = Vector3.zero;
            FADE_TIME += UnityEngine.Random.Range(0.1f, 0.45f);
        }

        public void Play()
        {
            IsPlaying = true;

            transform.DOScale(0.857f, 1f).OnComplete(() =>
                {
                    transform.localRotation = Quaternion.Euler(START_ROTATION);
                    _sequence = DOTween.Sequence();
                    _sequence.SetAutoKill(false);
                    _sequence
                        .AppendInterval(FADE_TIME)
                        .AppendInterval(FADE_TIME)
                        .Insert(0,
                            transform.DOLocalRotate(END_ROTATION, FADE_TIME * 2, RotateMode.LocalAxisAdd)
                                .SetEase(Ease.Linear))
                        .Insert(FADE_TIME * 2,
                            transform.DOLocalRotate(END_ROTATION, FADE_TIME * 2, RotateMode.LocalAxisAdd)
                                .SetEase(Ease.Linear))
                        .Insert(0, transform.DOScale(0.87f, FADE_TIME / 5).SetEase(Ease.InOutSine))
                        .Insert(FADE_TIME / 5, transform.DOScale(0.857f, FADE_TIME / 5).SetEase(Ease.InOutSine))
                        .Insert(2 * FADE_TIME / 5, transform.DOScale(0.87f, FADE_TIME / 5).SetEase(Ease.InOutSine))
                        .Insert(3 * FADE_TIME / 5, transform.DOScale(0.857f, FADE_TIME / 5).SetEase(Ease.InOutSine))
                        .OnComplete(() => { _sequence.Restart(); });
                    _sequence.Play();
                }
            );
        }

        public void Stop()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
            }

            transform.DOScale(0.3f, 1f);
            IsPlaying = false;
        }
    }
}