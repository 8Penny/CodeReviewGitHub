using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Services.Sounds
{
    public class SoundService : MonoBehaviour
    {
        private string MUSIC_ENABLED = "MusicEnabled";
        private string UI_SOUNDS_ENABLED = "UISoundsEnabled";
        
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private AudioSource _musicAudioSource;
        [SerializeField]
        private AudioMixerGroup _uiMixerGroup;
        [SerializeField]
        private AudioMixerGroup _musicMixerGroup;

        [Space]
        [SerializeField]
        private AudioClip _uiClick;
        [SerializeField]
        private AudioClip _uiUnableClick;
        [SerializeField]
        private AudioClip _uiMainCastleClick;
        [SerializeField]
        private AudioClip _uiCastleClick;
        [SerializeField]
        private AudioClip _castleSound;
        [SerializeField]
        private AudioClip _currencySound;
        [SerializeField]
        private AudioClip _tapOnFlower;
        [SerializeField]
        private AudioClip _abilityResearch;
        [SerializeField]
        private AudioClip _completeTutorialTask;
        [SerializeField]
        private AudioClip _chestAppearing;
        [SerializeField]
        private AudioClip _chestOpen;

#if UNITY_EDITOR
        private bool _isMusicEnabled = false;
#else 
        private bool _isMusicEnabled = true;
#endif
        private bool _isUIEnabled = true;

        public bool IsMusicEnabled => _isMusicEnabled;
        public bool IsUIEnabled => _isUIEnabled;
        
        private void Awake()
        {
            if (PlayerPrefs.HasKey(MUSIC_ENABLED))
            {
                _isMusicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED) == 1;
            }
            if (PlayerPrefs.HasKey(UI_SOUNDS_ENABLED))
            {
                _isUIEnabled = PlayerPrefs.GetInt(UI_SOUNDS_ENABLED) == 1;
            }

            UpdateMusic(_isMusicEnabled);
            UpdateUISounds(_isUIEnabled);
        }

        public void PlayClick()
        {
            _audioSource.PlayOneShot(_uiClick);
        }

        public void PlayOpenChest()
        {
            _audioSource.PlayOneShot(_chestOpen);
        }
        public void PlayChestAppearing()
        {
            _audioSource.PlayOneShot(_chestAppearing);
        }
        public void PlayTutorialTaskComplete()
        {
            _audioSource.PlayOneShot(_completeTutorialTask);
        }
        public void PlayMainCastleClick()
        {
            _audioSource.PlayOneShot(_uiMainCastleClick);
        }
        public void PlayClickUnable()
        {
            _audioSource.PlayOneShot(_uiUnableClick);
        }

        public void PlayOpenCastleSound()
        {
            _audioSource.PlayOneShot(_castleSound);
        }
        public void PlayTapOnFlower()
        {
            _audioSource.PlayOneShot(_tapOnFlower);
        }
        public void PlayCastleClick()
        {
            _audioSource.PlayOneShot(_uiCastleClick);
        }
        
        public void PlayCurrencySound()
        {
            _audioSource.PlayOneShot(_currencySound);
        }
        
        public void PlayAbilityResearch()
        {
            _audioSource.PlayOneShot(_abilityResearch);
        }

        public void ChangeMusicValue(bool isEnabled)
        {
            if (isEnabled == _isMusicEnabled)
            {
                return;
            }

            PlayerPrefs.SetInt(MUSIC_ENABLED, isEnabled ? 1 : 0);
            _isMusicEnabled = isEnabled;
            UpdateMusic(isEnabled);
        }
        
        public void ChangeUIValue(bool isEnabled)
        {
            if (isEnabled == _isUIEnabled)
            {
                return;
            }

            PlayerPrefs.SetInt(UI_SOUNDS_ENABLED, isEnabled ? 1 : 0);
            _isUIEnabled = isEnabled;
            UpdateUISounds(isEnabled);
        }

        private void UpdateMusic(bool isEnabled)
        {
            _musicAudioSource.volume = isEnabled ? 0.16f : 0f;
            UpdateAudio(_musicMixerGroup, isEnabled);
        }
        private void UpdateUISounds(bool isEnabled)
        {
            _audioSource.volume = isEnabled ? 0.16f : 0f;
            UpdateAudio(_uiMixerGroup, isEnabled);
        }

        private void UpdateAudio(AudioMixerGroup mixer, bool isEnabled)
        {
            int value = isEnabled ? 0 : -80;
            mixer.audioMixer.SetFloat("Volume", value);
        }
    }
}