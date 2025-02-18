using Services;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UIBasics.Views
{
    public class CloseWindowsButton : MonoBehaviour, IPointerClickHandler
    {
        private UIService _uiService;
        [Inject]
        public void Init(UIService uiService)
        {
            _uiService = uiService;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            _uiService.CloseWindow();
        }
    }
}