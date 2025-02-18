using Services;
using Services.Sounds;
using UnityEngine;
using Views;
using Zenject;

public class CatPurrComponent : MonoBehaviour
{
    private UIService _uiService;
    private SoundService _soundService;
    
    private ColliderButton _button;
    private AudioSource _audioSource;
    private AudioClip _clip;

    [Inject]
    public void Init(SoundService soundService, UIService uiService)
    {
        _uiService = uiService;
        _soundService = soundService;
    }
    
    private void Awake()
    {
        _button = transform.GetComponent<ColliderButton>();
        _audioSource = transform.GetComponent<AudioSource>();
        _clip = _audioSource.clip;
    }

    private void OnEnable()
    {
        _button.OnClick += ClickHandler;
    }
    private void OnDisable()
    {
        _button.OnClick -= ClickHandler;
    }

    private void ClickHandler()
    {
        if (!_soundService.IsUIEnabled)
        {
            return;
        }

        PlaySound();
    }

    private void PlaySound()
    {
        if (_audioSource.isPlaying)
        {
            return;
        }
        float distance = Vector3.Distance(_uiService.Views.Camera.transform.position, transform.position);
        float volume = 0;
        if (distance < 110f)
        {
            volume = 0.5f - (distance - 30) / 90f;
        }

        if (volume > 0)
        {
            _audioSource.PlayOneShot(_clip, 2f*Mathf.Clamp(volume, 0, 0.2f));
        }
    }
}
