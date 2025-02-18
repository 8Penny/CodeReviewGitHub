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
    public class InfoButton : MonoBehaviour
    {
        private UIService _uiService;
        private TutorialService _tutorialService;
        
        
        [Inject]
        public void Init(UIService uiService, TutorialService tutorialService)
        {
            _tutorialService = tutorialService;
            _uiService = uiService;
        }

        private void Awake()
        {
            if (!_tutorialService.IsComplete)
            {
                _tutorialService.OnNextTutorialStep += UpdateVisibility;
                UpdateVisibility(0);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
        private void OnDestroy()
        {
            _tutorialService.OnNextTutorialStep -= UpdateVisibility;
        }

        private void UpdateVisibility(int _)
        {
            UpdateVisibility();
        }


        private void UpdateVisibility()
        {
            gameObject.SetActive(_tutorialService.IsComplete);
        }
        
        public void OnButtonClicked()
        {
            _uiService.ShowInfoWindow();
        }
    }
}