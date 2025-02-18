using Services.Sounds;
using Services.TapBonus;
using Services.UIResourceAnimator;
using UnityEngine;
using Views;
using Zenject;

namespace Services.SimpleTap
{
    public class FlowerView : MonoBehaviour
    {
        [SerializeField]
        private ColliderButton _button;
        
        private TapBonusService _tapBonusService;
        private SoundService _soundService;
        private UIResourceAnimatorService _uiResourceAnimatorService;
        
        [Inject]
        public void Init(TapBonusService tapBonusService,
            SoundService soundService,
            UIResourceAnimatorService uiResourceAnimatorService)
        {
            _uiResourceAnimatorService = uiResourceAnimatorService;
            _tapBonusService = tapBonusService;
            _soundService = soundService;
        }

        public void Awake()
        {
            _tapBonusService.Register(this);
            gameObject.SetActive(false);
        }

        public void OnEnable()
        {
            _button.OnClick += OnFlowerClicked;
        }

        public void OnDisable()
        {
            _button.OnClick -= OnFlowerClicked;
        }
        
        private void OnFlowerClicked()
        {
            _uiResourceAnimatorService.Play(transform.position);
            _soundService.PlayTapOnFlower();
            _tapBonusService.CollectReward();
        }
    }
}