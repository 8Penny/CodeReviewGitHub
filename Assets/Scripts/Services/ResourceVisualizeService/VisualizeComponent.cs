using DG.Tweening;
using Services.Updater;
using UnityEngine;
using Zenject;

namespace Services.ResourceVisualizeService
{
    public class VisualizeComponent : MonoBehaviour, IUpdatable
    {
        [SerializeField]
        private ResourceVisualizeView[] _views;
        [SerializeField]
        private RectTransform _holder;
        [SerializeField]
        private RectTransform _innerHolder;
        [SerializeField]
        private CanvasGroup _canvasGroup;

        private SettingsService _settingsService;
        private ResourceVisualizerService _visualizerService;
        private UpdateService _updateService;
        private UIService _uiService;

        private Sequence _sequence;
        private bool _isActive;
        private Transform _point;
        private Vector2 _startPosition;

        public bool IsActive => _isActive;

        [Inject]
        public void Init(SettingsService settingsService, ResourceVisualizerService visualizerService,
            UpdateService updateService, UIService uiService)
        {
            _settingsService = settingsService;
            _visualizerService = visualizerService;
            _updateService = updateService;
            _uiService = uiService;

            _visualizerService.Register(this);
            _startPosition = _innerHolder.anchoredPosition;
            Complete();
        }

        private void OnEnable()
        {
            _updateService.Register(this);
        }
        private void OnDisable()
        {
            _updateService.Unregister(this);
        }

        public void Show(ResourcesHolder resourcesHolder, Transform point)
        {
            _point = point;

            int index = 0;
            foreach (var item in resourcesHolder.Values)
            {
                if (item.Value < 1f)
                {
                    continue;
                }
                var settings = _settingsService.GameResources[item.Key];
                _views[index].Set(settings.Sprite, (int)item.Value);
                _views[index].gameObject.SetActive(true);
                index++;
            }

            for (int i = index; i < _views.Length; i++)
            {
                _views[i].gameObject.SetActive(false);
            }

            Play();
        }

        private void Play()
        {
            _sequence = DOTween.Sequence();
            _isActive = true;
            _innerHolder.anchoredPosition = _startPosition;

            float y = _startPosition.y + 70f;
            _sequence
                .Append(_innerHolder.DOAnchorPosY(y, 0.6f))
                .Insert(0, _canvasGroup.DOFade(1f, 0.1f))
                .Insert(0.5f, _canvasGroup.DOFade(0f, 0.1f))
                .OnComplete(Complete)
                .Play();
        }

        private void Complete()
        {
            _isActive = false;
            _canvasGroup.alpha = 0;
        }

        void IUpdatable.Update()
        {
            if (!_isActive)
            {
                return;
            }
            _holder.position = _uiService.Views.Camera.WorldToScreenPoint(_point.position);
        }
    }
}