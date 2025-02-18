using DG.Tweening;
using Services.Tutorial;
using Services.UIResourceAnimator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.Tutorial
{
    public class TutorialArea : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _alpha;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private RectTransform _position;
        [SerializeField]
        private Color _rewardColor;

        private TutorialService _tutorialService;
        private UIResourceAnimatorService _uiResourceAnimator;

        [Inject]
        public void Init(TutorialService tutorialService, UIResourceAnimatorService uiResourceAnimatorService)
        {
            _uiResourceAnimator = uiResourceAnimatorService;
            _tutorialService = tutorialService;
        }

        private void Awake()
        {
            _tutorialService.OnTutorialStepUpdated += Setup;
            _tutorialService.OnNextTutorialStep += TutorialNextStepHandler;
            _tutorialService.OnTutorialHalfStep += HalfStepHandler;
            Setup();
        }
        private void OnDestroy()
        {
            _tutorialService.OnTutorialStepUpdated -= Setup;
            _tutorialService.OnNextTutorialStep -= TutorialNextStepHandler;
            _tutorialService.OnTutorialHalfStep -= HalfStepHandler;
        }

        private void Setup()
        {
            _alpha.alpha = _tutorialService.IsComplete || _tutorialService.TutorialStep == 0 ? 0f : 1f;
            if (_tutorialService.IsComplete)
            {
                return;
            }

            UpdateText();

            if (_tutorialService.TutorialStep == 0)
            {
                _alpha.DOFade(1f, 1f);
                return;
            }

            bool isCatVisible = _tutorialService.Config.TutorialSteps.TryGetValue(
                (TutorialStepNames) (_tutorialService.TutorialStep),
                out var r);
            _alpha.alpha = isCatVisible ? 1f : 0f;
        }

        private void TutorialNextStepHandler(int reward)
        {
            float waiting = 0.1f;
            if (reward > 0)
            {
                _uiResourceAnimator.Play(_position.transform.position, true);
                waiting = 4.2f;
            }

            if (_tutorialService.TutorialStep == 1)
            {
                waiting = 0.2f;
            }
            if (reward > 0)
            {
                UpdateText(false, true);
            }
            Sequence tween = DOTween.Sequence();
            
            if (_tutorialService.IsComplete)
            {
                tween.AppendInterval(waiting).Append(_alpha.DOFade(0f, 1f));
                tween.Play();
                return;
            }
            
            bool isCatVisible = _tutorialService.Config.TutorialSteps.TryGetValue(
                (TutorialStepNames) (_tutorialService.TutorialStep),
                out var r);
            float target = isCatVisible ? 1f : 0f;
            if (Mathf.Abs(target - _alpha.alpha) > 0.1f)
            {
                if (isCatVisible)
                {
                    UpdateText();
                }
                tween.Append(_alpha.DOFade(target, 1f));
                tween.Play();
                return;
            }
            tween.AppendInterval(waiting).OnComplete(()=>
            {
                UpdateText();
            });
            tween.Play();
        }

        private void UpdateText(bool isSecond = false, bool isReward = false)
        {
            string result;

            if (_tutorialService.Config.TutorialSteps.TryGetValue(
                    (TutorialStepNames) _tutorialService.TutorialStep,
                    out var r))
            {
                if (isReward)
                {
                    result = r.RewardText;
                    _text.color = _rewardColor;
                }
                else
                {
                    result = isSecond? r.SecondText : r.Text;
                    _text.color = Color.white;
                }
                
                if (result == _text.text)
                {
                    return;
                }
    
                _text.text = result;
            }
        }

        private void HalfStepHandler()
        {
            UpdateText(true);
        }
    }
}