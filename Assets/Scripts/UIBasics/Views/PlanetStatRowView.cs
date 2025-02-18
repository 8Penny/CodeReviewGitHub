using System;
using Services;
using Static;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views
{
    public class PlanetStatRowView : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private TextMeshProUGUI _resourceLabel;
        [SerializeField]
        private TextMeshProUGUI _percentLabel;
        //[SerializeField]
        //private TextMeshProUGUI _rateLabel;
        [SerializeField]
        private TextMeshProUGUI _countLabel;

        private ResourceNames _resourceId;
        private float _percent;
        private SettingsService _settingsService;
        public ResourceNames ResourceId => _resourceId;

        [Inject]
        public void Init(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public void UpdateParameters(float rate, float count)
        {
            //_rateLabel.text = $"{Math.Round(rate * _percent, 2)}/sec";
            _countLabel.text = UiUtils.GetCountableValue(count) ;
        }

        public void SetConstantFields(ResourceNames resourceId, int percent)
        {
            _resourceId = resourceId;
            _percent = percent / 100f;
            var current = _settingsService.GameResources[_resourceId];
            _image.sprite = current.Sprite;
            _resourceLabel.text = StaticNames.Get(resourceId);
            _percentLabel.text = $"{percent}%";
        }
    }
}