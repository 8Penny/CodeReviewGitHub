using DG.Tweening;
using UnityEngine;
using Views;
using Zenject;

namespace Services.Tutorial
{
    public class TutorialKitty : MonoBehaviour
    {
        [SerializeField]
        private Transform _cameraPosition;

        [SerializeField]
        private CastleView _castleView;

        private UIService _uiService;
        private TutorialService _tutorialService;

        private bool _isStarted;
        private float _time;
        private Vector3 _startPosition;
        private Quaternion _startRotation;

        [Inject]
        public void Init(UIService uiService, TutorialService tutorialService)
        {
            _uiService = uiService;
            _tutorialService = tutorialService;
            if (_tutorialService.TutorialStep > 2)
            {
                Destroy(gameObject);
                return;
            }

            _tutorialService.OnNextTutorialStep += NextStepHandler;
        }

        public void OnDestroy()
        {
            _tutorialService.OnNextTutorialStep -= NextStepHandler;
        }

        private void NextStepHandler(int _)
        {
            if (_tutorialService.TutorialStep != 2)
            {
                return;
            }

            _isStarted = true;
            _time = Time.time;
            var tr = _uiService.Views.Camera.transform;
            _startPosition = tr.position;
            _startRotation = tr.rotation;
            _uiService.Views.ScrollWorldComponent.Lock();
            //Time.timeScale = 0.5f;
        }

        public void Update()
        {
            if (!_isStarted)
            {
                return;
            }

            if (_castleView.Presenter.Progress > 0.5f)
            {
                _uiService.Views.Camera.transform.DOMove(_startPosition, 1f).OnComplete(()=>{{
                    _uiService.Views.ScrollWorldComponent.Unlock();}});
                _uiService.Views.Camera.transform.DORotateQuaternion(_startRotation, 1f);
                _isStarted = false;
                return;
            }

            _uiService.Views.Camera.transform.position = Vector3.Lerp(_startPosition,
                _cameraPosition.transform.position, 2f * (Time.time - _time));
            _uiService.Views.Camera.transform.LookAt(transform.parent.GetChild(0));
        }
    }
}