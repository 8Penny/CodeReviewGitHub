using DG.Tweening;
using Services.Sounds;
using Services.UIResourceAnimator;
using Static;
using TMPro;
using UnityEngine;
using Zenject;

namespace Services.Tutorial
{
    public class GiftChestWindow : MonoBehaviour
    {
        private int SOFT_REWARD = 35;
        private int OPEN_CHEST = Animator.StringToHash("Open");
        private int CLOSE_CHEST = Animator.StringToHash("Close");
        
        private TutorialService _tutorialService;
        private PlayerResourcesService _resourcesService;
        private UIResourceAnimatorService _uiResourceAnimatorService;
        private SoundService _soundService;

        [SerializeField]
        private CanvasGroup _content;
        [SerializeField]
        private RectTransform _rect;
        [SerializeField]
        private Animator _chest;
        [SerializeField]
        private GameObject _main;
        [SerializeField]
        private TextMeshProUGUI _textMeshProUGUI;
        
        private Sequence _sequence;
        private bool _isClickable;
        
        [Inject]
        public void Init(TutorialService service, PlayerResourcesService resourcesService,
            UIResourceAnimatorService uiResourceAnimatorService, SoundService soundService)
        {
            _resourcesService = resourcesService;
            _uiResourceAnimatorService = uiResourceAnimatorService;
            _tutorialService = service;
            _soundService = soundService;
            
            _content.alpha = 0;
            _chest.gameObject.SetActive(false);
            _content.gameObject.SetActive(false);
            _main.gameObject.SetActive(false);
        }

        public void Show(TutorialStepNames name)
        {
            _textMeshProUGUI.text = name is TutorialStepNames.Ability4Shown ? "Tutorial is completed!" : "Congratulations!";
            _sequence = DOTween.Sequence();
            _content.alpha = 0;
            
            _chest.gameObject.SetActive(true);
            _content.gameObject.SetActive(true);
            _main.gameObject.SetActive(true);
            _sequence.Append(_content.DOFade(1f, 1f).OnComplete(() => { _isClickable = true;}));
            _sequence.Play();
            
            _soundService.PlayChestAppearing();
        }
        
        private void Hide() {
            _sequence = DOTween.Sequence();
            _content.alpha = 1f;
            
            _chest.SetTrigger(CLOSE_CHEST);
            _sequence.AppendInterval(1.3f).Append(_content.DOFade(0f, 0.5f)).OnComplete(()=>
            {
                _chest.gameObject.SetActive(false);
                _main.gameObject.SetActive(false);
                _content.gameObject.SetActive(false);
                _tutorialService.ContinueTutorial();
            });
            _sequence.Play();
        }

        public void GetReward()
        {
            if (!_isClickable)
            {
                return;
            }
            _isClickable = false;
            
            _sequence = DOTween.Sequence();
            _chest.SetTrigger(OPEN_CHEST);
            _soundService.PlayClick();
            _sequence.AppendInterval(2f).AppendCallback(PlayCoins).AppendInterval(0.2f).AppendCallback(PlayCoins)
                .AppendInterval(0.2f).AppendCallback(PlayCoins).AppendInterval(1f).OnComplete(Hide);
            _sequence.Play();
        }

        private void PlayCoins()
        {
            _uiResourceAnimatorService.Play(_rect.position, true);
            _soundService.PlayCurrencySound();
            _resourcesService.AddResource(ResourceNames.Soft, SOFT_REWARD);
        }
    }
}