using Services.Sounds;
using Services.UIResourceAnimator;
using Static;
using UnityEngine;
using Views;
using Zenject;

namespace Services.SimpleTap
{
    public class CartView : MonoBehaviour
    {
        private const float ROTATION_SPEED_RATIO = 11f;
        
        [SerializeField]
        private ColliderButton _button;
        [SerializeField]
        private Transform _wheels;

        private SoundService _soundService;
        private UIResourceAnimatorService _uiResourceAnimatorService;
        private TapSecondBonusService _tapSecondBonusService;
        private bool _wasShoot;

        public bool WasShoot => _wasShoot;
        public float Speed => 4f;
        
        [Inject]
        public void Init(TapSecondBonusService tapSecondBonusService,
            SoundService soundService,
            UIResourceAnimatorService uiResourceAnimatorService)
        {
            _uiResourceAnimatorService = uiResourceAnimatorService;
            _tapSecondBonusService = tapSecondBonusService;
            _soundService = soundService;
        }

        public void Awake()
        {
            _tapSecondBonusService.Register(this);
            gameObject.SetActive(false);
        }

        public void OnEnable()
        {
            _button.OnClick += OnWheelTap;
        }

        public void OnDisable()
        {
            _button.OnClick -= OnWheelTap;
        }
        
        private void OnWheelTap()
        {
            _tapSecondBonusService.CollectReward(out ResourceNames reward, out float emeraldChance);
            _uiResourceAnimatorService.Play(transform.position, reward, emeraldChance);
            _soundService.PlayTapOnFlower();
        }

        public void ProcessRotation()
        {
            _wheels.Rotate(0,ROTATION_SPEED_RATIO * Speed * Time.deltaTime, 0);
        }
    }
}