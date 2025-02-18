using System;
using DG.Tweening;
using Services.Sounds;
using Services.Tasks;
using Services.Tutorial;
using Settings;
using UIBasics.Views.AbilityPanel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Sequence = DG.Tweening.Sequence;

namespace UIBasics.Views.Tutorial
{
    public class AbilityTutorialController : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect _scroll;
        
        private TutorialService _tutorialService;
        private TutorialTaskService _tutorialTaskService;
        private Sequence _sequence;
        private SoundService _soundService;

        private TutorialStepNames _lastTutorialStep;

        [Inject]
        public void Init(TutorialService tutorialService, SoundService soundService, TutorialTaskService tutorialTaskService)
        {
            _tutorialTaskService = tutorialTaskService;
            _tutorialService = tutorialService;
            _soundService = soundService;
        }

        public void OnEnable()
        {
            _scroll.enabled = _tutorialService.IsComplete;
        }

        public bool CanClick()
        {
            return (TutorialStepNames)_tutorialService.TutorialStep < TutorialStepNames.Ability4Shown;
        } 
        public bool CanClickViaTutorial(AbilityType abilityType)
        {
            if (_tutorialService.IsComplete)
            {
                return true;
            }

            var needType = _tutorialTaskService.CurrentTasks[0].TutorialTaskType;
            switch (needType)
            {
                case TutorialTaskType.OpenFlowerPicker:
                    return abilityType == AbilityType.Flower1;
                case TutorialTaskType.OpenWorkbench:
                    return abilityType == AbilityType.Workbench1;
                case TutorialTaskType.OpenCraftTable:
                    return abilityType == AbilityType.CraftTable1;
            }
            return false;
        } 

        public void Click(AbilityView view)
        {
            TutorialStepNames tutorialStep = (TutorialStepNames) _tutorialService.TutorialStep;

            TutorialStepNames current = TutorialStepNames.CameraAppearing;
            TutorialStepNames next = TutorialStepNames.CameraAppearing;
            switch (view.Id)
            {
                case AbilityType.Workbench1:
                    current = TutorialStepNames.StartedAbilityShowing;
                    next = TutorialStepNames.TapedOn1Ability;
                    _soundService.PlayClick();
                    break;
                case AbilityType.Flower1:
                    current = TutorialStepNames.Ability1Shown;
                    next = TutorialStepNames.TapedOn2Ability;
                    _soundService.PlayClick();
                    break;
                case AbilityType.Mining1:
                    current = TutorialStepNames.Ability2Shown;
                    next = TutorialStepNames.TapedOn3Ability;
                    _soundService.PlayClick();
                    break;
                case AbilityType.Spyglass1:
                    current = TutorialStepNames.Ability3Shown;
                    next = TutorialStepNames.TapedOn4Ability;
                    _soundService.PlayClick();
                    break;
            }

            
            if (tutorialStep == current)
            {
                SetTutorialStep(next);
            }
            else
            {
                OnClickFinished();
            }
        }

        private void SetTutorialStep(TutorialStepNames nextTutorialStep)
        {
            _tutorialService.NextAbilityTutorialStep(nextTutorialStep);

            _lastTutorialStep = TutorialStepNames.Ability4Shown;
            switch (nextTutorialStep)
            {
                case TutorialStepNames.TapedOn1Ability:
                    _lastTutorialStep = TutorialStepNames.Ability1Shown;
                    break;
                case TutorialStepNames.TapedOn2Ability:
                    _lastTutorialStep = TutorialStepNames.Ability2Shown;
                    break;
                case TutorialStepNames.TapedOn3Ability:
                    _lastTutorialStep = TutorialStepNames.Ability3Shown;
                    break;
            }

            
            _sequence = DOTween.Sequence();
            _sequence.AppendInterval(3f).OnComplete(OnClickFinished);
        }

        private void OnClickFinished()
        {
            _sequence?.Kill();
            if (_lastTutorialStep == TutorialStepNames.Ability4Shown)
            {
                _tutorialService.CompleteClickedAbility();
                return;
            }
            _tutorialService.NextAbilityTutorialStep(_lastTutorialStep);
        }

        public void TryStart()
        {
            TutorialStepNames tutorialStep = (TutorialStepNames) _tutorialService.TutorialStep;
            if (tutorialStep is TutorialStepNames.OpenedAbilityPanel)
            {
                _tutorialService.NextAbilityTutorialStep(TutorialStepNames.StartedAbilityShowing);
            }
        }
    }
}