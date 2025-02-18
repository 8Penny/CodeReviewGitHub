using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace UIBasics.Views.ResourcesPanel
{
    public class ResourceSellPanelView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _noResourceSelected;
        [SerializeField]
        private GameObject _resourceContent;
        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private TextMeshProUGUI _count;
        [SerializeField]
        private TextMeshProUGUI _price;

        private float _multiplier = 1f;
        private int _value;

        public float Multiplier => _multiplier;

        private void OnEnable()
        {
            _slider.value = _multiplier;
        }
        
        public void OnContentChanged()
        {
            _multiplier = _slider.value;
        }

        public void UpdateResourceValue(float count, float price)
        {
            _count.text = UiUtils.GetCountableValue(count);
            _price.text = UiUtils.GetCountableValue(price, 0);
        }

        public void UpdatePanel(bool isResourceSelected )
        {
            _noResourceSelected.SetActive(!isResourceSelected);
            _resourceContent.SetActive(isResourceSelected);
        }
    }
}