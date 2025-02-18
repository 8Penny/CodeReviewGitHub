using Services;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UIBasics.Views
{
    public abstract class BaseClickButton : MonoBehaviour, IPointerClickHandler
    {
        protected UIService _uiService;
        
        [Inject]
        public void Init(UIService uiService)
        {
            _uiService = uiService;
        }

        protected abstract void OnClick();

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }
    }
}