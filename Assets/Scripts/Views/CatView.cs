
using Services.Boosts;
using Services.Talents;
using Services.Updater;
using Services.View;
using Static;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using IUpdatable = Services.Updater.IUpdatable;

namespace Views
{
    public class CatView : MonoBehaviour, IUpdatable
    {
        private int SPEED = Animator.StringToHash("Speed");
        private int BAM_COOLDOWN = 2;
        private float BAM_DISTANCE = 2.2f;
        private int BAM_ANGLE = 100;
        
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private float _debugParameter = 2.28f;
        [SerializeField]
        private int  _debugPow = 2;
        [Space]
        [SerializeField]
        private bool _bummEffectOwner;
        [SerializeField]
        private CatView _neighbourCat;
        [SerializeField]
        private SkinnedMeshRenderer _oldCatModel;
        [SerializeField]
        private SkinnedMeshRenderer _newCatModel;
        
        private BezierSpline _curve;
        
        private Castle _castle;
        private UpdateService _updateService;
        private TalentsService _talentsService;
        private BoostService _boostService;
        private EffectsViewService _effectsViewService;
        private Transform _transform;
        private Transform _neighbourCatTransform;

        private bool _isSubscribed;
        private float _bamLastTime;

        public float Speed => _castle?.CatSpeedLevel ?? 0;
        
        
        [Inject]
        public void Init(UpdateService updateService, BoostService boostService, TalentsService talentsService, EffectsViewService effectsViewService)
        {
            _updateService = updateService;
            _boostService = boostService;
            _talentsService = talentsService;
            _effectsViewService = effectsViewService;
            
            _transform = transform;
            if (_neighbourCat != null)
            {
                _neighbourCatTransform = _neighbourCat.gameObject.transform;
            }
            if (_bummEffectOwner && _neighbourCat == null)
            {
                Debug.LogError("Please fill in bum properties for CAT", this);
            }

            _newCatModel.material = _oldCatModel.material;
        }

        public void OnEnable()
        {
            _updateService.Register(this);
            SyncPosition();
        }
        public void OnDisable()
        {
            _updateService.Unregister(this);
        }

        public void SetParameters(BezierSpline curve, Castle castle)
        {
            if (_isSubscribed)
            {
                _castle.OnLevelUpdated -= UpdateAnimatorParameters;
            }
            _curve = curve;
            _castle = castle;
            _castle.OnLevelUpdated += UpdateAnimatorParameters;
            _isSubscribed = true;
            UpdateAnimatorParameters();
        }

        public void Update()
        {
            SyncPosition();
            ProcessBumEffect();
        }

        private void SyncPosition()
        {
            if (_castle == null)
            {
                return;
            }
            var t = _castle.Progress;
            bool toCastle = t < 0.5f;
            //var fromPoint = toCastle ? _start : _end;
            var halfPathProgress = GetHalfProgress(t, toCastle);
            float nextRatio = halfPathProgress + (toCastle ? 1 : -1) * 0.1f;
            Vector3 next = _curve.GetPoint(nextRatio);
            _transform.LookAt(next);
            Vector3 finalPos = _curve.GetPoint(halfPathProgress);
            // if (_siblingPathId != -1)
            // {
            //     finalPos += _transform.right * (_isLeft&&!toCastle? 1: -1);
            // }

            _transform.position = finalPos;


            if (!_animator.gameObject.activeSelf || !_animator.enabled)
            {
                return;
            }

            return;//todo: if remove skibidi
            _animator.SetFloat(SPEED, 
                CastleUtils.GetSpeed(_castle.CatSpeedLevel,
                    _talentsService.CatSpeedMultiplier * _boostService.GetPlanetBoost(_castle.Id)) *Mathf.Pow(_debugParameter, 1f/_debugPow));
        }

        private float GetHalfProgress(float t, bool toCastle)
        {
            return toCastle ? t * 2f : 1 - ((t - 0.5f) * 2);
        }

        public void UpdateAnimatorParameters()
        {
            _animator.SetFloat(SPEED, _castle.CatSpeedLevel * Mathf.Pow(_debugParameter,(1f/_debugPow)));
        }

        private void ProcessBumEffect()
        {
            if (!_bummEffectOwner || _neighbourCatTransform == null)
            {
                return;
            }

            if (Speed >StaticValues.BumMaxSumLevel || _neighbourCat.Speed > StaticValues.BumMaxSumLevel)
            {
                return;
            }

            if (Time.time - _bamLastTime < BAM_COOLDOWN)
            {
                return;
            }

            float d = Vector3.Distance(_transform.position, _neighbourCatTransform.position);
            float a = Vector3.Angle(_transform.forward, _neighbourCatTransform.forward);
            if (d < BAM_DISTANCE && a > BAM_ANGLE)
            {
                _bamLastTime = Time.time;
                _effectsViewService.PlayBum((_transform.position + _neighbourCatTransform.position) / 2 + Vector3.up * 3.5f);
            }
        }
    }
}