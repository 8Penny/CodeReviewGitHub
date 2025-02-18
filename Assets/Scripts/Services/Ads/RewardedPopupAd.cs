using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Services.Ads
{
    public class RewardedPopupAd : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text;


        public void Close()
        {
            gameObject.SetActive(false);
        }
        
        public void SetText(string text)
        {
            _text.text = text;
        }
        
    }
}