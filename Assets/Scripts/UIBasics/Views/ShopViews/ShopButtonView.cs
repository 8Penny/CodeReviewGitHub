using Services;
using Services.Tutorial;
using UnityEngine;
using Zenject;

namespace UIBasics.Views.ShopViews
{
    public class ShopButtonView : MonoBehaviour
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
        }
        private void OnDestroy()
        {
            _tutorialService.OnNextTutorialStep -= UpdateVisibility;
        }

        private void UpdateVisibility(int _)
        {
            gameObject.SetActive(_tutorialService.IsComplete);
        }

        public void OnButtonClicked()
        {
            _uiService.ShowShopWindow();
        }
    }
}