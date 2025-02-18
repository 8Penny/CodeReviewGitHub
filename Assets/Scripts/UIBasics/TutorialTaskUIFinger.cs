using System;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using Services.Tasks;
using Services.Tutorial;
using Settings;
using UnityEngine;
using Zenject;
using Sirenix.OdinInspector;
using TMPro;

namespace UIBasics
{
    public class TutorialTaskUIFinger : MonoBehaviour
    {
        [SerializeField]
        private TutorialTaskType _activeOnTask;
        [SerializeField]
        private PanelType _panelType = PanelType.None;
        [SerializeField]
        private GameObject _container;
        [SerializeField]
        private GameObject _circleTemplate;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Color _completeColor;
        [SerializeField]
        private bool _workOnAnyAbility;

        private List<UITutorialCircle> _images = new List<UITutorialCircle>();
        private Sequence _sequence;

        private int _currentValue;
        private int _targetValue;

        private bool _wasClicked;
        
        [Inject]
        public TutorialTaskService _TutorialTaskService;
        [Inject]
        public TutorialService _TutorialService; 
        [Inject]
        public UIService _uiService;

        public void Awake()
        {
            if (_TutorialService.IsComplete)
            {
                _canvasGroup.gameObject.SetActive(false);
                return;
            }

    
            for (int i = 0; i < 4 ; i++)
            {
                GameObject n = Instantiate(_circleTemplate.gameObject, _circleTemplate.transform.parent);
                _images.Add(n.GetComponent<UITutorialCircle>());
            }

            _TutorialTaskService.TaskUpdated += UpdateHandler;
            _TutorialTaskService.TaskAdded += UpdateHandler;
            _TutorialTaskService.TaskCompleted += UpdateHandler;
            _TutorialService.OnNextTutorialStep += UpdateHandler;
            if (_uiService.Views != null)
            {
                Register();
                return;
            }
            _uiService.OnViewsRegistered += Register;
        }

        private void Register()
        {
            _uiService.OnPanelChanged += UpdateVisibilityHandler;
            _uiService.OnViewsRegistered -= Register;
        }

        public void OnDestroy()
        {
            _TutorialService.OnNextTutorialStep -= UpdateHandler;
            _TutorialTaskService.TaskUpdated -= UpdateHandler;
            _TutorialTaskService.TaskAdded -= UpdateHandler;
            _TutorialTaskService.TaskCompleted -= UpdateHandler;
            _uiService.OnPanelChanged += UpdateVisibilityHandler;
            
            _uiService.OnViewsRegistered -= Register;
            
            _uiService.OnPanelChanged -= UpdateVisibilityHandler;
        }

        public void OnEnable()
        {
            UpdateCurrentFinger();
        }

        private void UpdateHandler(TutorialTaskType t)
        {
            UpdateCurrentFinger();
        }
        private void UpdateHandler()
        {
            UpdateCurrentFinger();
        }
        private void UpdateHandler(TutorialTaskType t, bool forced)
        {
            UpdateCurrentFinger(forced);
        }
        private void UpdateHandler(int _)
        {
            UpdateCurrentFinger();
        }

        private void UpdateVisibilityHandler(PanelType panelType)
        {
            UpdateCurrentFinger();
        }

        private void UpdateCurrentFinger(bool forced = false)
        {
            if (!_TutorialTaskService.HasTutorialTasks)
            {
                _container.SetActive(false);
                return;
            }

            bool isCorrectTutorialStep =
                (TutorialStepNames) _TutorialService.TutorialStep is TutorialStepNames.CastleUpgrade
                or TutorialStepNames.ResourceSold
                or TutorialStepNames.ChosenCastleForBoost
                or TutorialStepNames.Ability4Shown;
            if (!isCorrectTutorialStep)
            {
                _container.SetActive(false);
                return;
            }

            var ttype = _TutorialTaskService.CurrentTasks[0].TutorialTaskType;
            if (_workOnAnyAbility && _activeOnTask == TutorialTaskType.OpenFlowerPicker)
            {
                switch (ttype)
                {
                    case TutorialTaskType.OpenWorkbench:
                    case TutorialTaskType.OpenFlowerPicker:
                    case TutorialTaskType.OpenCraftTable:
                        break;
                    default:
                        _container.SetActive(false);
                        return;
                }
            }
            else if (_activeOnTask != ttype)
            {
                _container.SetActive(false);
                return;
            }
            
            var newCurrent = _TutorialTaskService.CurrentTasks[0].Value;
            bool needTip = newCurrent != 0 && newCurrent > _currentValue;
            _currentValue = newCurrent;
            _targetValue = _TutorialTaskService.GetTaskTargetValue(ttype);

            bool isVisible = !_TutorialTaskService.IsTaskCompleted(ttype);
            if (isVisible && _panelType != PanelType.None)
            {
                isVisible = !_uiService.IsPanelActive;
            }
            _container.SetActive(isVisible);

            if ((forced ||_wasClicked) && needTip)
            {
                PlayTip();
            }
        }

        [Button]
        public void OnClick()
        {
            //_sequence = DOTween.Sequence();
            if (_container.activeSelf)
            {
                _wasClicked = true;
                CreateNewSequence();
            }
        }

        private void PlayTip()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
            }
            else
            {
                _canvasGroup.alpha = 0;
            }
            _sequence = DOTween.Sequence();
            bool isCompleted = _currentValue >= _targetValue;
            _text.text = isCompleted? "Task completed!" : $"progress: {_currentValue}/{_targetValue}";
            _text.color = isCompleted ? _completeColor : Color.white;
            _sequence.Append(_canvasGroup.DOFade(1f, 0.3f)).AppendInterval(0.7f).Append(_canvasGroup.DOFade(0f, 0.3f));
            _sequence.Play();
            
        }

        private void CreateNewSequence()
        {
            UITutorialCircle free = null;
            foreach (var i in _images)
            {
                if (!i.gameObject.activeSelf)
                {
                    free = i;
                    break;
                }
            }

            if (free == null)
            {
                return;
            }
            free.Play();
        }
    }
}