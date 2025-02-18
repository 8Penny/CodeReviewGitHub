using System;
using System.Collections;
using Services.Tutorial;
using UIBasics;
using UnityEngine;
using Zenject;

namespace Views
{
    public class CameraManLogic : MonoBehaviour
    {

        private int ATTACK = Animator.StringToHash("Attack");
        private int DEATH = Animator.StringToHash("Death");
        [Inject]
        public TutorialService _TutorialService;
        [Inject]
        public PlayerDataManager _DataManager;
        
        [SerializeField]
        private Animator _cameraManAnimator;
        [SerializeField]
        private Transform _finish;
        [SerializeField]
        private Transform _start;
        [SerializeField]
        private GameObject _cameraMan;
        [SerializeField]
        private ColliderButton _manButton;
        [SerializeField]
        private TutorialArrow _arrow;

        [SerializeField]
        private float _speed = 8f;

        private Vector3 _dir;
        private Transform _cmTransform;

        private bool _started;
        private float _timer;
        private bool _isDead;
        public void OnEnable()
        {
            _arrow.UpdateVisibility(false);
            _cmTransform = _cameraMan.transform;
            _dir =(_finish.position - _start.position).normalized;
            _cameraMan.SetActive(false);

            _TutorialService.OnNextTutorialStep += TutorialStepUpdatedHandler;
            _manButton.OnClick += ButtonClick;

            if (_DataManager.IsLoaded)
            {
                TutorialStepUpdatedHandler(0);
            }
        }

        public void OnDisable()
        {
            _TutorialService.OnNextTutorialStep -= TutorialStepUpdatedHandler;
            _manButton.OnClick -= ButtonClick;
        }

        private void TutorialStepUpdatedHandler(int _)
        {
            if (_TutorialService.IsComplete)
            {
                gameObject.SetActive(false);
                OnDisable();
                return;
            }

            if (_TutorialService.TutorialStep != (int) TutorialStepNames.FirstCastleUnlocked)
            {
                return;
            } 
            
            _cameraMan.transform.position = _start.position;
            _cameraMan.gameObject.SetActive(true);
            _started = true;
            _timer = 3f;
        }

        private void ButtonClick()
        {
            if (_isDead)
            {
                return;
            }
            _cameraManAnimator.SetTrigger(DEATH);
            _isDead = true;
            StartCoroutine(DeathCo());
        }

        private void Update()
        {
            Update1(); 
        }

        private void Update1()
        {
            if (!_started)
            {
                return;
            }

            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
                return;
            }

            float t = Vector3.Distance(_start.position, _cmTransform.position);
            float tt =Vector3.Distance(_start.position, _finish.position) + 0.2f;
            if (t < tt)
            {
                _cameraMan.transform.position += _dir * Time.deltaTime * _speed;
                return;
            }

            _started = false;
            _cameraManAnimator.SetTrigger(ATTACK);

            _arrow.UpdateVisibility(true);
        }
        private IEnumerator DeathCo()
        {
            _arrow.UpdateVisibility(false);
            yield return new WaitForSeconds(2.5f);
            _TutorialService.OnCameramanDeath();
            
            yield return new WaitForSeconds(2.5f);
            _cameraMan.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}