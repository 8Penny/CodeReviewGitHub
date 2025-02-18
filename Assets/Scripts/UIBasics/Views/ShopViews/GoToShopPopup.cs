using Services;
using UnityEngine;
using Zenject;

namespace UIBasics.Views.ShopViews
{
    public class GoToShopPopup : MonoBehaviour
    {
        private UIService _uiService;

        [SerializeField]
        private GameObject _softText;
        [SerializeField]
        private GameObject _hardText;
        
        [Inject]
        public void Init(UIService uiService)
        {
            _uiService = uiService;
            Close();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
        public void Show(bool isSoft)
        {
            _softText.gameObject.SetActive(isSoft);
            _hardText.gameObject.SetActive(!isSoft);
            gameObject.SetActive(true);
        }
        public void GoToShop()
        {
            Close();
            _uiService.ShowShopWindow();
        }
    }
}