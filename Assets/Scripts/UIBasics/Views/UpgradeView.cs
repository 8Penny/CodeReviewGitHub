using System;
using Static;
using TMPro;
using UI.Presenters.Upgrades;
using UnityEngine;
using UnityEngine.UI;

namespace UIBasics.Views
{
    public class UpgradeView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _levelLabel;
        [SerializeField]
        private TextMeshProUGUI _valueLabel;
        [SerializeField]
        private TextMeshProUGUI _priceLabel;
        [SerializeField]
        private TextMeshProUGUI _addValue;
        
        [SerializeField]
        private TutorialTaskUIFinger _tutorialTaskUIFinger;
        
        
        [SerializeField]
        private Sprite _activeButton;
        [SerializeField]
        private Sprite _inactiveButton;

        private UpgradePresenter _presenter;
        private Image _image;

        private void Awake()
        {
            _image = _priceLabel.transform.parent.GetComponent<Image>();
        }

        public void SetPresenter(UpgradePresenter presenter)
        {
            _presenter = presenter;
        }
        
        public void UpdateParameters(int level, int price, bool isButtonActive)
        {
            _levelLabel.text = level.ToString();
            _priceLabel.text = UiUtils.GetCountableValue(price, 0);
            _priceLabel.color = isButtonActive? Color.white : StaticValues.InactiveTextColor;
            _image.sprite = isButtonActive ? _activeButton : _inactiveButton;
        }

        public void SetRate(float value, float nextValue)
        {
            _valueLabel.text = $"{Math.Round(value, 2)}/s";
            SetNextValue(nextValue);
        }

        public void SetSpeed(float value, float nextValue)
        {
            _valueLabel.text = Math.Round(value, 2).ToString();
            SetNextValue(nextValue);
        }

        public void SetCapacity(int value, int nextValue)
        {
            _valueLabel.text = value.ToString();
            SetNextValue(nextValue, true);
        }

        public void SetNextValue(float value, bool isInteger = false)
        {
            var result = isInteger
                ? UiUtils.GetCountableValue(Mathf.FloorToInt(value))
                : Math.Round(value, 2).ToString();
            _addValue.text = result;
        }

        public void OnButtonClick()
        {
            _tutorialTaskUIFinger.OnClick();
            _presenter.OnButtonClickedHandler();
        }
    }
}