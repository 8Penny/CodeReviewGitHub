using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace Services.Tutorial
{
    public class TutorialHoleController : IDisposable
    {
        private UIService _uiService;
        private TutorialService _tutorialService;
        private PlayerDataManager _playerDataManager;
        
        private HashSet<TutorialUIArrow> _uiElements = new HashSet<TutorialUIArrow>();

        private Sequence _colorSequence = DOTween.Sequence();

        public TutorialHoleController(UIService uiService, TutorialService tutorialService, PlayerDataManager playerDataManager)
        {
            _uiService = uiService;
            _playerDataManager = playerDataManager;
            _tutorialService = tutorialService;

            _uiService.OnViewsRegistered += UpdateHoleStatus;
            _uiService.OnWindowChangedVisibility += WindowChangedVisibilityHandler;
            _uiService.OnPanelChanged += PanelChangedHandler;
        }

        public void Dispose()
        {
            _uiService.OnViewsRegistered -= UpdateHoleStatus;
            _uiService.OnWindowChangedVisibility -= WindowChangedVisibilityHandler;
            _uiService.OnPanelChanged -= PanelChangedHandler;
        }
        
        public void Activated(TutorialUIArrow element)
        {
            int count = _uiElements.Count;
            _uiElements.Add(element);
            if (_uiService.Views != null && count != _uiElements.Count)
            {
                UpdateHoleStatus();
            }
        }
        
        public void Deactivated(TutorialUIArrow element)
        {
            if (_uiElements.Contains(element))
            {
                _uiElements.Remove(element);            
                if (_uiService.Views != null)
                {
                    UpdateHoleStatus();
                }
            }
        }

        private void UpdateHoleStatus()
        {
            bool isVisible = false;
            Vector3 position = Vector3.zero;
            
            foreach (var element in _uiElements)
            {
                position = element.transform.position;
                if (element.IsOnWindow && _uiService.IsWindowActive)
                {
                    isVisible = true;
                    break;
                }
                if (element.IsOnPanel && _uiService.IsPanelActive)
                {
                    isVisible = true;
                    break;
                }
                if (element.IsOnNavigation && !_uiService.IsWindowActive)
                {
                    isVisible = true;
                    break;
                }
            }

            isVisible &= IsVisibleOnStep((TutorialStepNames) _tutorialService.TutorialStep);
            if (isVisible)
            {
                SetHolePosition(position);
            }

            UpdateHoleVisibility(isVisible);
        }

        private bool IsVisibleOnStep(TutorialStepNames name)
        {
            switch (name)
            {
                //case TutorialStepNames.OpenedAbilityPanel:
                case TutorialStepNames.TapedOn1Ability:
                case TutorialStepNames.TapedOn2Ability:
                case TutorialStepNames.TapedOn3Ability:
                case TutorialStepNames.TapedOn4Ability:
                    return true;
                default:
                    return false;
            }
        }
        private void UpdateHoleVisibility(bool isVisible)
        {
            if (_colorSequence.IsActive())
            {
                _colorSequence.Kill();
            }
            bool isHoleVisible = (TutorialStepNames) _tutorialService.TutorialStep >= TutorialStepNames.FirstCastleUnlocked && isVisible;
            float target = isHoleVisible ? 0.8f : 0f;
            if (Mathf.Abs(_uiService.Views.TutorialHole.color.a - target) < 0.02f)
            {
                return;
            }

            _colorSequence = DOTween.Sequence();
            _colorSequence.Append(_uiService.Views.TutorialHole.DOFade(target, 0.5f)).Play();
        }
        private void SetHolePosition(Vector3 v)
        {
            _uiService.Views.TutorialHole.transform.position = v;
        }

        private void WindowChangedVisibilityHandler(bool isVisible)
        {
            UpdateHoleStatus();
        }
        private void PanelChangedHandler(PanelType _)
        {
            UpdateHoleStatus();
        }
    }
}