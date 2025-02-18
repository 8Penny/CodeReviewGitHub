using UnityEngine;
using UnityEngine.UI;

namespace UIBasics.Views.Tutorial
{
    public class ResourceAnimator : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _destination;
        [SerializeField]
        private RectTransform[] _lerpHelpPoints;

        [SerializeField]
        private AnimationCurve _scaleCurve;
        [SerializeField]
        private AnimationCurve _fadeCurve;
        [SerializeField]
        private Image[] _images;
        [SerializeField]
        private RectTransform _imagesHolder;
        [SerializeField]
        private float _maxScale = 1f;
        [SerializeField]
        private float[] _times;
        
        private float _startTime;
        private float _maxTime;

        private bool _isPlaying;

        private Vector2[] _startPositions;
        public Image[] Images => _images;
        public bool IsPlaying => _isPlaying;
        
        public void Awake()
        {
            _startPositions = new Vector2[_images.Length];
            for (int i = 0; i < _images.Length; i++)
            {
                _images[i].color = Color.clear;
            }
        }
        
        public void SetImagesHolderPosition(Vector2 pos)
        {
            _imagesHolder.position = pos;
        }

        private void SetPositions()
        {
            for (int i = 0; i < _images.Length; i++)
            {
                _startPositions[i] = _images[i].transform.position;
                _images[i].color = Color.clear;
                if (_maxTime < _times[i])
                {
                    _maxTime = _times[i];
                }
            }
        }

        public void Play()
        {
            SetPositions();
            _startTime = Time.time;
            _isPlaying = true;
        }

        public void Update()
        {
            if (!_isPlaying)
            {
                return;
            }

            for (int i = 0; i < _images.Length; i++)
            {
                float currentTimePassed = (Time.time - _startTime);

                if (currentTimePassed > _maxTime)
                {
                    _isPlaying = false;
                    ReturnImages();
                    break;
                }
                float ratio = currentTimePassed/ _times[i];
                if (ratio > 1f)
                {
                    _images[i].color = Color.clear;
                    continue;
                }
                
                float scale = _scaleCurve.Evaluate(ratio) * _maxScale;
                Vector2 localScale = new Vector2(scale, scale);
                float alpha = _fadeCurve.Evaluate(ratio);
                _images[i].color = new Color(1, 1, 1, alpha);
                
                Vector2 l1 = Vector2.Lerp(_startPositions[i], _lerpHelpPoints[i].transform.position, ratio);
                Vector2 l2 = Vector2.Lerp(_startPositions[i], _destination.transform.position, ratio);
                _images[i].transform.position = Vector2.Lerp(l1, l2, ratio);
                _images[i].transform.localScale = localScale;
            }
        }

        private void ReturnImages()
        {
            for (int i = 0; i < _images.Length; i++)
            {
                _images[i].color = Color.clear;
                _images[i].transform.position = _startPositions[i];
            }
        }
    }
}