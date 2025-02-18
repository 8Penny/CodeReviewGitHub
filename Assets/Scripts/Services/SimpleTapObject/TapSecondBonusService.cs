using System;
using Services.Analytics;
using Services.Talents;
using Services.Updater;
using Settings;
using Static;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Services.SimpleTap
{
    public class TapSecondBonusService : IDisposable, IUpdatable
    {
        private readonly SettingsService _settingsService;
        private readonly UpdateService _updateService;
        private readonly TalentsService _talentsService;
        private readonly PlayerResourcesService _playerResourcesService;
        private readonly AnalyticsService _analytics;

        private float _lastSpawnTime;
        private bool _hasActive;
        private CartPath _points;
        private Vector3 _direction;
        private CartView _cart;

        private PairsContainer _spawns;
        private MainGameSettings _config => _settingsService.MainGameConfig;

        public TapSecondBonusService(SettingsService settingsService, UpdateService updateService,
            TalentsService talentsService, PlayerResourcesService playerResourcesService, AnalyticsService analyticsService)
        {
            _settingsService = settingsService;
            _updateService = updateService;
            _talentsService = talentsService;
            _playerResourcesService = playerResourcesService;

            _analytics = analyticsService;


            _updateService.Register(this);
        }

        public void Dispose()
        {
            _updateService.Unregister(this);
        }

        public void Register(PairsContainer pairs)
        {
            _spawns = pairs;
        }

        public void Register(CartView cart)
        {
            _cart = cart;
            cart.gameObject.SetActive(false);
        }

        public void Update()
        {
            if (!_talentsService.HasTalent(AbilityType.Cart1))
            {
                return;
            }

            if (_hasActive)
            {
                ProcessMovement();
                return;
            }

            Spawn();
        }

        private void Spawn()
        {
            if (_lastSpawnTime + _config.CartCooldown * _talentsService.CartCooldownMultiplier > Time.time || _cart.gameObject.activeSelf)
            {
                return;
            }

            _points = _spawns.Pairs[Random.Range(0, _spawns.Pairs.Length)];
            Transform wheel = _cart.transform;
            wheel.position = new Vector3(_points.Points[0].position.x, 0, _points.Points[0].position.z);
            var localPosition = wheel.localPosition;
            localPosition = new Vector3(localPosition.x, 1.8f, localPosition.z);
            wheel.localPosition = localPosition;

            _direction = (_points.Points[1].position - _points.Points[0].position);
            _direction -= Vector3.up*_direction.y;
            _direction = _direction.normalized;
            
            wheel.GetChild(0).LookAt(wheel.position + _direction);
            _cart.gameObject.SetActive(true);
            _hasActive = true;
        }

        private void ProcessMovement()
        {
            var mainDistance = (_points.Points[0].position - _points.Points[1].position).magnitude;
            var currentDistance = (_points.Points[0].position - _cart.transform.position ).magnitude;
            if (mainDistance < currentDistance && !_cart.WasShoot)
            {
                HideObject();
                return;
            }
            _cart.transform.position += _direction * Time.deltaTime * _cart.Speed;
            _cart.ProcessRotation();
        }

        private void HideObject()
        {
            _hasActive = false;
            _cart.gameObject.SetActive(false);
            _lastSpawnTime = Time.time;
        }

        public void CollectReward(out ResourceNames currentReward, out float emeraldChance)
        {
            HideObject();
            var rewards = _settingsService.MainGameConfig.CartRewards;
            currentReward = rewards[Random.Range(0, rewards.Count)];
            var count = Random.Range(_config.MinCartRewardsCount, _config.MaxCartRewardsCount + 1);
            emeraldChance = Random.Range(0, 3) >= 2 ?_talentsService.CartEmeraldChance : 0;
            int emeraldCount = (int)(count * emeraldChance);
            count -= emeraldCount;
            if (currentReward == ResourceNames.Soft)
            {
                count *= 100;
            }
            
            _playerResourcesService.AddResource(currentReward, count);
            _playerResourcesService.AddResource(ResourceNames.Hard, emeraldCount);
            
            _analytics.OnMapObjectTap(true);
        }

    }
}