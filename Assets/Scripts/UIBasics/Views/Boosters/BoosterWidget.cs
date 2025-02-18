using System;
using Services;
using Services.Boosts;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.Boosters
{
    public class BoosterWidget : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private GameObject _bg;
        [SerializeField]
        private GameObject _icon;
        [SerializeField]
        private GameObject _button;
        [SerializeField]
        private BoostType _boostType;
        [SerializeField]
        private TextMeshProUGUI _timeText;

        private BoostService _boostService;
        private SettingsService _settingsService;
        private TickService _tickService;

        private SingleBoost _boost;

        [Inject]
        public void Init(BoostService boostService, SettingsService settingsService, TickService tickService)
        {
            _boostService = boostService;
            _settingsService = settingsService;
            _tickService = tickService;
        }


        private void Awake()
        {
            _boost = _settingsService.BoostsSettings.Boosts.Find(t => t.BoostType == _boostType);
        }

        public void Update()
        {
            if (!_boostService.IsActive(_boostType))
            {
                _image.fillAmount = 0;
                _button.SetActive(false);
                _icon.SetActive(false);
                _bg.SetActive(false);
                _timeText.gameObject.SetActive(false);
                return;
            }

            _button.SetActive(true);
            _icon.SetActive(true);
            _bg.SetActive(true);
            _image.fillAmount = _boostService.GetBoostRatio(_boostType);
            
            _timeText.gameObject.SetActive(true);
            _timeText.text = UiUtils.FormatedTime((int)_boostService.GetTime(_boostType));
        }
    }
}