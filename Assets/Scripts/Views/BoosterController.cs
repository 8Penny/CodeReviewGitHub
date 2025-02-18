using System;
using Services;
using Services.Sounds;
using UI;
using UnityEngine;
using Zenject;

namespace Views
{
    public class BoosterController : MonoBehaviour
    {
        [SerializeField]
        private ColliderButton _colliderButton;

        private UIService _uiService;
        private SoundService _soundService;
        
        [Inject]
        public void Init(UIService uiService, SoundService soundService)
        {
            _uiService = uiService;
            _soundService = soundService;
        }

        private void OnEnable()
        {
            _colliderButton.OnClick += OpenBoostsWindow;
        }
        private void OnDisable()
        {
            _colliderButton.OnClick -= OpenBoostsWindow;
        }

        public void OpenBoostsWindow()
        {
            _soundService.PlayClick();
            _uiService.OnPanelButtonClicked(PanelType.Boosters);
        }
    }
}