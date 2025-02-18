using System;
using Services;
using Services.Sounds;
using Services.Tasks;
using Services.Tutorial;
using Settings;
using UnityEngine;
using Zenject;

namespace UIBasics.Views.Tutorial
{
    public class TutorialButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject _lightEffect;
        
        private UIService _uiService;
        private TutorialService _tutorialService;
        private TutorialTaskService _tutorialTaskService;

        private bool _isNewTaskShow;

        
        [Inject]
        public void Init(UIService uiService, TutorialService tutorialService, TutorialTaskService tutorialTaskService)
        {
            _tutorialTaskService = tutorialTaskService;
            _tutorialService = tutorialService;
            _uiService = uiService;
        }

        private void Awake()
        {
            if (!_tutorialService.IsComplete)
            {
                _tutorialService.OnNextTutorialStep += UpdateVisibility;
                _tutorialTaskService.TaskAdded += TaskAddedHandler;
                _tutorialTaskService.TaskCompleted += TaskCompletedHandler;
                _tutorialTaskService.TaskUpdated += TaskUpdatedHandler;
                UpdateVisibility(0);
                UpdateEffect();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        private void OnDestroy()
        {
            _tutorialService.OnNextTutorialStep -= UpdateVisibility;
            _tutorialTaskService.TaskAdded -= TaskAddedHandler;
            _tutorialTaskService.TaskCompleted -= TaskCompletedHandler;
            _tutorialTaskService.TaskUpdated -= TaskUpdatedHandler;
        }

        private void OnEnable()
        {
            _uiService.OnWindowChangedVisibility += WindowChangedVisibilityHandler;
        }

        private void OnDisable()
        {
            _lightEffect.SetActive(false);
            _uiService.OnWindowChangedVisibility -= WindowChangedVisibilityHandler;
        }

        private void WindowChangedVisibilityHandler(bool isVisible)
        {
            if (isVisible)
            {
                _lightEffect.SetActive(false);
                return;
            }

            UpdateEffect();
        }

        private void TaskAddedHandler()
        {
            if (!_tutorialTaskService.HasEarnTask())
            {
                _isNewTaskShow = true;
            }
            
            UpdateEffect();
            UpdateVisibility();
        }
        private void TaskCompletedHandler(TutorialTaskType type)
        {
            UpdateVisibility();
        }
        private void TaskUpdatedHandler(TutorialTaskType type, bool forced)
        {
            UpdateEffect();
        }

        private void UpdateVisibility(int _)
        {
            UpdateVisibility();
        }

        private void UpdateEffect()
        {//_isNewTaskShow ||
            _lightEffect.gameObject.SetActive( _tutorialTaskService.HasAnyCompleted());
        }

        private void UpdateVisibility()
        {
            bool isFits = (TutorialStepNames) _tutorialService.TutorialStep is TutorialStepNames.CastleUpgrade 
                or TutorialStepNames.CastleUpgrade
                or TutorialStepNames.ClosedTaskWindow
                or TutorialStepNames.TakenEmeralds
                or TutorialStepNames.ActivatedBoost 
                or TutorialStepNames.ChosenCastleForBoost
                or TutorialStepNames.ResourceSold 
                or TutorialStepNames.Ability4Shown;
            gameObject.SetActive(!_tutorialService.IsComplete && isFits && _tutorialTaskService.HasTutorialTasks);
        }
        
        public void OnButtonClicked()
        {
            _isNewTaskShow = false;
            _uiService.ShowTutorialTaskWindow();
        }
    }
}