using System;
using System.Collections.Generic;
using Services.Analytics;
using Services.SimpleTap;
using Services.Talents;
using Services.Tasks;
using Services.Updater;
using Settings;
using Static;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Services.TapBonus
{
    public class TapBonusService: IDisposable, IUpdatable
    {
        private readonly SettingsService _settingsService;
        private readonly UpdateService _updateService;
        private readonly  TalentsService _talentsService;
        private readonly  PlayerResourcesService _playerResourcesService;
        private readonly  TaskService _taskService;
        private readonly AnalyticsService _analytics;
        private MainGameSettings _config => _settingsService.MainGameConfig;
        
        private FlowerView _flower;
        private List<Transform> _positions;

        private float _lastSpawnTime;

        public TapBonusService(SettingsService settingsService, UpdateService updateService,
            TalentsService talentsService, PlayerResourcesService playerResourcesService, TaskService taskService,
            AnalyticsService analyticsService)
        {
            _settingsService = settingsService;
            _updateService = updateService;
            _talentsService = talentsService;
            _playerResourcesService = playerResourcesService;
            _taskService = taskService;
            _analytics = analyticsService;

            _updateService.Register(this);
        }

        public void Dispose()
        {
            _updateService.Unregister(this);
        }

        public void Register(FlowerView flowerView)
        {
            _flower = flowerView;
            _flower.gameObject.SetActive(false);
        }

        public void RegisterPoints(List<Transform> positions)
        {
            _positions = positions;
        }

        public void CollectReward()
        {
            _flower.gameObject.SetActive(false);
            _lastSpawnTime = Time.time;
            
            var count = Random.Range(_config.MinFlowerRewardsCount, _config.MaxFlowerRewardsCount + 1) * _talentsService.FlowerSoftMultiplier;
            _playerResourcesService.AddResource(ResourceNames.Soft, count);

            _taskService.OnSimpleTapDestroyed();
        }

        public void Update()
        {
            if (!_talentsService.HasTalent(AbilityType.Flower1))
            {
                return;
            }

            float cooldown = _talentsService.FlowerCooldownMultiplier * _config.FlowersCooldown;

            if (_lastSpawnTime + cooldown > Time.time || _flower.gameObject.activeSelf)
            {
                return;
            }

            _lastSpawnTime = Time.time;
            _flower.transform.position = _positions[Random.Range(0, _positions.Count)].position;
            _flower.gameObject.SetActive(true);
            
            _analytics.OnMapObjectTap();
        }
    }
}