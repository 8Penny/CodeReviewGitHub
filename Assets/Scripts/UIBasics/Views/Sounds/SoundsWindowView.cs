using System;
using Services.Sounds;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.Sounds
{
    public class SoundsWindowView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _main;
        [SerializeField]
        private GameObject _content;
        [SerializeField]
        private Toggle _musicToggle;
        [SerializeField]
        private Toggle _uiToggle;

        private SoundService _soundService;

        [Inject]
        public void Init(SoundService soundService)
        {
            _soundService = soundService;
            _musicToggle.onValueChanged.AddListener(delegate {ValueChangeCheck(true);});
            _uiToggle.onValueChanged.AddListener(delegate {ValueChangeCheck();});
        }

        private void Awake()
        {
            _content.gameObject.SetActive(false);
        }

        private void UpdateParameters()
        {
            _musicToggle.isOn = _soundService.IsMusicEnabled;
            _uiToggle.isOn = _soundService.IsUIEnabled;
        }

        public void Show()
        {
            UpdateParameters();
            
            _content.SetActive(true);
            _main.SetActive(true);
        }
        
        public void Close()
        {
            _content.SetActive(false);
            _main.SetActive(false);
        }

        private void ValueChangeCheck(bool isMusic = false)
        {
            if (isMusic)
            {
                _soundService.ChangeMusicValue(_musicToggle.isOn);
            }else
            {
                _soundService.ChangeUIValue(_uiToggle.isOn);
            }
        }
    }
}