using System;
using TMPro;
using UnityEngine;

namespace UIBasics.Views
{
    public class MultiplierView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _label;

        public void UpdateVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
        public void SetValue(float value)
        {
            if (gameObject.activeSelf)
            {
                _label.text = $"x{Math.Round(value, 2)}";
            }
        }
    }
}