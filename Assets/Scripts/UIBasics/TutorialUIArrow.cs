using System;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using Services.Talents;
using Services.Tutorial;
using Settings;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

using Sirenix.OdinInspector;


    public class TutorialUIArrow : MonoBehaviour
    {
        [SerializeField]
        public TutorialStepNames _showOnTutorialStepName;
        [SerializeField]
        private bool _isAnimated;
        [SerializeField]
        private bool _ignoreHole;
        [SerializeField]
        private List<Sprite> _sprites;
        [SerializeField]
        private float _spriteTime = 0.3f;
        [SerializeField]
        public int _sign = 1;
        [SerializeField]
        public bool _onlyScale = false;
        [SerializeField]
        public bool _isOnNavigation = false;
        [SerializeField]
        public bool _showInWindow = false;
        [SerializeField]
        public PanelType _panelType = PanelType.None;

        [SerializeField]
        public bool _hideIfWindow;

        private TalentsService _talentsService;
        private TutorialService _tutorialService;
        private TutorialHoleController _holeController;
        private UIService _uiService;
        private RectTransform _rect;
        private bool _isPlaying;
        private Sequence _sequence;
        private Vector3 _startScale;

        private int _index;
        private float _timeToNext;
        private Image _image;

        private Vector2 _endPosition;
        private Vector2 _startPosition;

        private bool _windowIsDisabled = true;


        public bool IsOnNavigation => _isOnNavigation;
        public bool IsOnWindow => _showInWindow;
        public bool IsOnPanel => !_showInWindow && !_isOnNavigation;

        [Inject]
        public void Init(TutorialHoleController hole, TutorialService tutorialService, UIService uiService, TalentsService talentsService)
        {
            _holeController = hole;
            _tutorialService = tutorialService;
            _uiService = uiService;
            _talentsService = talentsService;

            _image = GetComponent<Image>();
        }

        [Button]
        public void Create()
        {
            TutorialUIArrow prefab = Resources.Load<TutorialUIArrow>("Circle");
            TutorialUIArrow go = Instantiate(prefab, transform.parent);
            go.transform.localPosition = transform.localPosition;
            go._showOnTutorialStepName = _showOnTutorialStepName;
            go._sign = _sign; 
            go._onlyScale = _onlyScale;
            go._isOnNavigation = _isOnNavigation;
            go._showInWindow = _showInWindow; 
            go._panelType = _panelType;
            go._hideIfWindow = _hideIfWindow;
            
            DestroyImmediate(gameObject);
        }
        
        
        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _startScale = _rect.localScale;
            _endPosition = _rect.anchoredPosition;
            _startPosition = _endPosition + new Vector2(0, _sign * 40f);

            _tutorialService.OnNextTutorialStep += NextTutorialStepHandler;
            _uiService.OnPanelChanged += UpdateVisibility;

            UpdateVisibility(_uiService.Views.DataPanelView.CurrentPanelType);
            Setup();
        }

        private void OnDestroy()
        {
            _tutorialService.OnNextTutorialStep -= NextTutorialStepHandler;
            _uiService.OnPanelChanged -= UpdateVisibility;
        }

        private void UpdateVisibility(PanelType current)
        {
            _windowIsDisabled = true;
            Setup();
        }

        private void OnEnable()
        {
            _windowIsDisabled = true;
            _uiService.OnWindowChangedVisibility += UpdatedWindowHandler;
            _uiService.OnPanelChanged += UpdatedPanelHandler;
            Setup();
        }

        private void OnDisable()
        {
            _uiService.OnWindowChangedVisibility -= UpdatedWindowHandler;
            _uiService.OnPanelChanged -= UpdatedPanelHandler;
            if (_isPlaying)
            {
                _isPlaying = false;
                _sequence?.Kill();
            }
        }

        private void UpdatedWindowHandler(bool isVisible)
        {
            _windowIsDisabled = !isVisible;
            Setup();
        }

        private void UpdatedPanelHandler(PanelType _)
        {
            Setup();
        }

        private void NextTutorialStepHandler(int _)
        {
            Setup();
        }

        public void Update()
        {
            if (!_isAnimated)
            {
                return;
            }
            _timeToNext -= Time.deltaTime;
            if (_timeToNext < 0)
            {
                _index++;
                if (_index >= _sprites.Capacity)
                {
                    _index = 0;
                }
                _timeToNext = _spriteTime;
                _image.sprite = _sprites[_index];
            }
        }

        private void Setup()
        {
            bool isVisible = _tutorialService.TutorialStep == (int)_showOnTutorialStepName;
            if (!isVisible)
            {
                gameObject.SetActive(false);
                _holeController.Deactivated(this);
                return;
            }


            if (_onlyScale && !_isOnNavigation)
            {
                isVisible = isVisible && _uiService.Views.DataPanelView.CurrentPanelType != PanelType.Resources;
            }
            if (_isOnNavigation && _panelType != PanelType.None)
            {
                isVisible &= _uiService.Views.DataPanelView.CurrentPanelType != _panelType;
            }
            if (IsOnPanel)
            {
                isVisible &= _uiService.Views.DataPanelView.CurrentPanelType == _panelType;
            }

            if (_hideIfWindow)
            {
                isVisible &= _windowIsDisabled;
            }
            
            gameObject.SetActive(isVisible);
            if (!isVisible && _isPlaying)
            {
                _isPlaying = false;
                _sequence.Kill();
            }

            if (!_ignoreHole)
            {
                if (isVisible)
                {
                    _holeController.Activated(this);
                }
                else
                {
                    _holeController.Deactivated(this);
                }
            }

            if (isVisible && gameObject.activeInHierarchy && !_isPlaying)
            {
                // float seconds = 0.4f;
                // _sequence = DOTween.Sequence();
                // _sequence.SetAutoKill(false);
                // if (!_onlyScale)
                // {
                //     _rect.anchoredPosition = _startPosition;
                //     _sequence.Append(_rect.DOAnchorPos(_endPosition, seconds/1.2f).SetEase(Ease.InOutQuad))
                //         .Append(_rect.DOAnchorPos(_startPosition, seconds).SetEase(Ease.InOutQuad))
                //         .OnComplete(()=>_sequence.Restart())
                //         .Play();
                // } else
                // {
                //     _rect.localScale = _startScale;
                //     Vector3 endScale = _startScale / 1.3f;
                //     _sequence.Append(_rect.DOScale(endScale, seconds/1.2f).SetEase(Ease.InOutQuad))
                //         .Append(_rect.DOScale(_startScale, seconds).SetEase(Ease.InOutQuad))
                //         .OnComplete(()=>_sequence.Restart())
                //         .Play();
                // }


                _isPlaying = true;
            }
        }
    }
