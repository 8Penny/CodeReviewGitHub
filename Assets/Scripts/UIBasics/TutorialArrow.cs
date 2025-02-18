using DG.Tweening;
using Services;
using Services.Tasks;
using Services.Tutorial;
using Settings;
using UnityEngine;
using Zenject;

namespace UIBasics
{
    public class TutorialArrow: MonoBehaviour
    {
        [SerializeField]
        private bool _byScript;
        private SpriteRenderer _sprite;
        private UIService _ui;

        private TutorialService _tutorialService;
        private TutorialTaskService _tutorialTaskService;
        private float _startZ;
        private float _endZ;
        private Transform _transform;
        private bool _isPlaying;
        private Sequence _sequence;

        private bool _isVisible = true;
        private bool IsArrowVisible =>_byScript? _isVisible : (TutorialStepNames)_tutorialService.TutorialStep is TutorialStepNames.CameraToCastle
            or TutorialStepNames.CameraMan
            or TutorialStepNames.ActivatedBoost
            or TutorialStepNames.CastleUpgrade && !_ui.IsPanelActive && !_tutorialTaskService.HasAnyCompleted();
        
        [Inject]
        public void Init(TutorialService tutorialService, UIService ui, TutorialTaskService tutorialTaskService)
        {
            _tutorialTaskService = tutorialTaskService;
            _ui = ui;
            _tutorialService = tutorialService;
            
            _tutorialService.OnTutorialStepUpdated += Setup;
            _tutorialService.OnNextTutorialStep += NextTutorialStepHandler;
            _ui.OnViewsRegistered += ViewRegisteredHandler;
            _tutorialTaskService.TaskCompleted += TutorialTaskCompletedHandler;
        }


        private void ViewRegisteredHandler()
        {
            _ui.OnPanelChanged += PanelChangedVisibilityHandler;
        }

        private void OnDestroy()
        {
            _tutorialService.OnTutorialStepUpdated -= Setup;
            _tutorialService.OnNextTutorialStep -= NextTutorialStepHandler;
            _ui.OnPanelChanged -= PanelChangedVisibilityHandler;
            _tutorialTaskService.TaskCompleted -= TutorialTaskCompletedHandler;

            DOTween.KillAll();
        }
        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _transform = transform;
            _startZ = _transform.position.z;
            _endZ = _startZ - 1.5f;
            
            Setup();
        }
        private void PanelChangedVisibilityHandler(PanelType _)
        {
            Setup();
        }

        private void TutorialTaskCompletedHandler(TutorialTaskType _)
        {
            Setup();
        }
        private void Setup()
        {
            if (!IsArrowVisible)
            {
                _sprite.enabled = false;
                if (_isPlaying)
                {
                    _sequence.Kill();
                    _isPlaying = false;
                }
                return;
            }

            if (_isPlaying)
            {
                return;
            }
            _sprite.enabled = true;
            
            var pos = _transform.position;
            _transform.position = new Vector3(pos.x, pos.y, _startZ);
            float seconds = 0.4f;
            _sequence = DOTween.Sequence();
            _sequence.SetAutoKill(false);
            _sequence.Append(_transform.DOMoveZ(_endZ, seconds / 1.2f).SetEase(Ease.InOutQuad))
                .Append(_transform.DOMoveZ(_startZ, seconds).SetEase(Ease.InOutQuad))
                .OnComplete(()=>_sequence.Restart())
                .Play();

            _isPlaying = true;
        }

        private void NextTutorialStepHandler(int reward)
        {
            Setup();
        }

        public void UpdateVisibility(bool isVisible)
        {
            _isVisible = isVisible;
            Setup();
        }
    }
}