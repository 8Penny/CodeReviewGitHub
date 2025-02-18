using DG.Tweening;
using Services.Sounds;
using Services.UIResourceAnimator;
using Static;
using UnityEngine;
using Zenject;

namespace Services.Tutorial
{
    public class GiftEmeraldsElement : MonoBehaviour
    {
        private int HARD_REWARD = 35;
        private TutorialService _tutorialService;
        private PlayerResourcesService _resourcesService;
        private UIResourceAnimatorService _uiResourceAnimatorService;
        private SoundService _soundService;
        private UIService _uiService;

        [SerializeField]
        private CanvasGroup _content;

        [SerializeField]
        private RectTransform _rect;

        private Sequence _sequence;
        private bool _isClickable;
        [Inject]
        public void Init(TutorialService service, PlayerResourcesService resourcesService,
            UIResourceAnimatorService uiResourceAnimatorService, SoundService soundService,
            UIService uiService)
        {
            _uiService = uiService;
            _soundService = soundService;
            _uiResourceAnimatorService = uiResourceAnimatorService;
            _resourcesService = resourcesService;
            _tutorialService = service;
            _content.alpha = 0;
            _content.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _tutorialService.OnNextTutorialStep += NextTutorialStepHandler;
            NextTutorialStepHandler(0);
        }
        private void OnDisable()
        {
            _tutorialService.OnNextTutorialStep -= NextTutorialStepHandler;
        }

        private void NextTutorialStepHandler(int _)
        {
            bool isFit = (TutorialStepNames) _tutorialService.TutorialStep is TutorialStepNames.ClosedTaskWindow
                or TutorialStepNames.SetRecipe;
            if (isFit)
            {
                _uiService.CloseMainPanel();
                _sequence = DOTween.Sequence();
                _content.alpha = 0;
                _content.gameObject.SetActive(true);
                _sequence.AppendInterval(0.3f).Append(_content.DOFade(1f, 1f)).OnComplete(()=>{_isClickable = true;});
                _sequence.Play();
                return;
            }

            bool isEnd = (TutorialStepNames) _tutorialService.TutorialStep is TutorialStepNames.TakenEmeralds
                or TutorialStepNames.TakenEmeralds2;
            if (isEnd)
            {
                _sequence = DOTween.Sequence();
                _content.alpha = 1f;
                
                _sequence.Append(_content.DOFade(0f, 0.5f)).OnComplete(()=>
                {
                    _content.gameObject.SetActive(false);
                });
                _sequence.Play();
            }
        }

        public void GetReward()
        {
            if (!_isClickable)
            {
                return;
            }

            _isClickable = false;
            _tutorialService.ActivateBoostStep();
            _uiResourceAnimatorService.Play(_rect.position,true, true);
            _resourcesService.AddResource(ResourceNames.Hard, HARD_REWARD);
            _soundService.PlayCurrencySound();
        }
    }
}