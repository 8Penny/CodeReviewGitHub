using Services;
using Settings;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.AbilityPanel
{
    public class AbilityView : MonoBehaviour
    {
        [SerializeField]
        private AbilityType _id;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Image _bgImage;
        [SerializeField]
        private GameObject _closedImage;

        [SerializeField]
        private Color _available;
        [SerializeField]
        private Color _unavailable;
        [SerializeField]
        private Color _opened;
        [SerializeField]
        private Color _closed;
        [SerializeField]
        private Color _canBuy;
        
        private SettingsService _settingsService;

        private Ability _currentSettings;
        private bool _isClickable;
        public AbilityType Id => _id;
        public Ability CurrentSettings => _currentSettings;
        public bool CanBuy { get; private set; }
        public bool IsClickable => _isClickable;

        [Inject]
        public void Init(SettingsService settingsService)
        {
            _settingsService = settingsService;
            _currentSettings = _settingsService.AbilitiesTree.AbilitiesDict[_id];
        }

        public void InitAbility(bool isAvailable, bool isOpened, bool canBuy = false)
        {
            _image.sprite = _currentSettings.Sprite;
            _isClickable = isOpened || isAvailable;

            //_isClickable = true;
            
            _image.gameObject.SetActive(_isClickable);
            _closedImage.SetActive(!_isClickable);
            

            CanBuy = canBuy;
            Color bgColor = Color.white;
            if (isOpened)
            {
                bgColor = _opened;
            } else if (isAvailable)
            {
                bgColor = canBuy ? _canBuy : _available;
            }
            else
            {
                bgColor = _unavailable;
            }
            _bgImage.color = bgColor;
        }
    }
}