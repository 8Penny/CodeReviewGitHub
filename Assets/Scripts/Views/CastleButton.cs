using Services;
using Services.Sounds;
using Services.Tutorial;
using UnityEngine;
using Zenject;

namespace Views
{
    public class CastleButton : MonoBehaviour
    {
        [SerializeField]
        private ColliderButton _colliderButton;
        
        private TutorialService _tutorialService;
        private SoundService _soundService;
        private UIService _uiService;

        [Inject]
        public void Init(TutorialService tutorialService, SoundService soundService, UIService uiService)
        {
            _soundService = soundService;
            _tutorialService = tutorialService;
            _uiService = uiService;
        }
        public void OnEnable()
        {
            _colliderButton.OnClick += ShowResourcePanel;
        }
        public void OnDisable()
        {
            _colliderButton.OnClick -= ShowResourcePanel;
        }

        private void ShowResourcePanel()
        {
            if (_tutorialService.TutorialStep == 0)
            {
                return;
            }
            _soundService.PlayMainCastleClick();
            _uiService.Views.DataPanelView.OpenPanel(PanelType.Resources);
        }
    }
}