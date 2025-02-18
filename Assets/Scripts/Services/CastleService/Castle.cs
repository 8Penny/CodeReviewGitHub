using System;
using System.Collections.Generic;
using System.Linq;
using Services;
using Services.Boosts;
using Services.ResourceVisualizeService;
using Services.Talents;
using Settings;
using Static;
using UI.Presenters.Upgrades;
using UnityEngine;


    [Serializable]
    public class Castle : ITickUpdatable
    {
        private ResourcesHolder _resourcesHolder;
        private ResourcesHolder _catResourcesHolder;
        private float _progress;
        private int _mineSpeedLevel = 1;
        private int _catSpeedLevel = 1;
        private int _capacityLevel = 1;
        private bool _isOpen;
        private int _id;

        [NonSerialized]
        private PlanetSettings _settings;
        [NonSerialized]
        private ResourceVisualizerService _visualizerService;
        [NonSerialized]
        private List<float> _weightMultipliers;
        [NonSerialized]
        private PlayerResourcesService _playerResourcesService;
        [NonSerialized]
        private TalentsService _talentsService;
        [NonSerialized]
        private BoostService _boostService;
        [NonSerialized]
        public Action OnLevelUpdated;

        public ResourcesHolder ResourcesHolder => _resourcesHolder;
        public float Progress => _progress;
        public int MineSpeedLevel => _mineSpeedLevel;
        public int CatSpeedLevel => _catSpeedLevel;
        public int CapacityLevel => _capacityLevel;
        public bool IsOpen => _isOpen;
        public int Id => _id;
        public int Weight => 2;
        
        public PlanetSettings Settings => _settings;

        public Castle()
        {
            _resourcesHolder = new ResourcesHolder();
            _catResourcesHolder = new ResourcesHolder();
        }

        public void SetSettings(PlanetSettings settings, PlayerResourcesService playerResourcesService,
            TalentsService talentsService, BoostService boostService, ResourceVisualizerService visualizerService)
        {
            if (settings == _settings)
            {
                return;
            }

            _talentsService = talentsService;
            _playerResourcesService = playerResourcesService;
            _boostService = boostService;
            _visualizerService = visualizerService;
            
            _settings = settings;
            _id = settings.Id;
            _settings.Resources = _settings.Resources.OrderByDescending(t => t.Weight).ToList();
            _weightMultipliers = new List<float>();
            float weightSum = _settings.Resources.Sum(s => s.Weight);
            foreach (var r in _settings.Resources)
            {
                _weightMultipliers.Add(r.Weight/weightSum);
            }
        }

        public void Open()
        {
            _isOpen = true;
        }
        
        public void Update(float secondsCount)
        {
            if (!_isOpen)
            {
                return;
            }
            UpdateCastlePocketResources(secondsCount);
            UpdateCastleProgress(secondsCount);

        }

        private void UpdateCastlePocketResources(float secondsCount)
        {
            var value = CastleUtils.GetMiningCount(secondsCount, _mineSpeedLevel,
                _talentsService.MiningRateMultiplier * _boostService.MiningBoost * _boostService.GetPlanetBoost(_id));
            for (int i = 0; i < _settings.Resources.Count - 1; i++)
            {
                ResourceNames resourceId = _settings.Resources[i].ResourceId;
                float resourceValue = _weightMultipliers[i] * value;
                value -= resourceValue;

                _resourcesHolder.AddResource(resourceId, resourceValue);
            }
            _resourcesHolder.AddResource(_settings.Resources.Last().ResourceId, value);
        }
        
        private void UpdateCastleProgress(float secondsCount)
        {
            var distance = CastleUtils.GetDistance(secondsCount, _catSpeedLevel, _settings,
                _talentsService.CatSpeedMultiplier * _boostService.GetPlanetBoost(_id));
            UpdateProgress(distance);
        }

        private void UpdateProgress(float value)
        {
            var currentCapacity = CastleUtils.GetCapacity(_capacityLevel, 
                _talentsService.BackpackMultiplier * _boostService.GetPlanetBoost(_id));
            if (value > 1f)
            {
                int cycles = (int)Math.Truncate(value);
                TransferResourcesFromCastleToPocket(cycles * currentCapacity);
            }
            
            var progressTail = value - Math.Truncate(value);
            var newValue = progressTail + _progress;

            if (newValue > 1f && _catResourcesHolder.Values.Count != 0)
            {
                TransferResourcesFromCatToPlayer();
            }
            else if (newValue > 1f)
            {
                TransferResourcesFromCastleToPocket(currentCapacity);
            }
            progressTail = newValue - Math.Truncate(newValue);
            _progress = (float)progressTail;
            
            if (_progress > 0.5f && _catResourcesHolder.Values.Count == 0)
            {
                TransferResourcesFromCastleToPocket(currentCapacity, false);
            }
        }

        private void TransferResourcesFromCatToPlayer()
        {
            _visualizerService.Play(_catResourcesHolder, _id);
            foreach (var item in _catResourcesHolder.Values)
            {
                _playerResourcesService.AddResource(item.Key, item.Value);
            }

            _catResourcesHolder.Values.Clear();
        }

        private void TransferResourcesFromCastleToPocket(float resourcesCount, bool toPlayerPocket = true)
        {
            for (int i = 0; i < _settings.SortedResources.Count; i++)
            {
                if (resourcesCount < 0.001f)
                {
                    return;
                }
                ResourceNames resourceId = _settings.SortedResources[i].ResourceId;
                var currentCount = Mathf.Min(resourcesCount, _resourcesHolder.Get(resourceId));
                
                resourcesCount -= currentCount;
                _resourcesHolder.AddResource(resourceId, -currentCount);
                if (toPlayerPocket)
                {
                    _playerResourcesService.AddResource(resourceId, currentCount);
                }
                else
                {
                    _catResourcesHolder.AddResource(resourceId, currentCount);
                }
            }
        }

        public void SetLevel(CastleParameterType currentType, int nextLevel)
        {
            switch (currentType)
            {
                case CastleParameterType.Capacity:
                    _capacityLevel = nextLevel;
                    break;
                case CastleParameterType.CatSpeed:
                    _catSpeedLevel = nextLevel;
                    break;
                case CastleParameterType.MiningRate:
                    _mineSpeedLevel = nextLevel;
                    break;
            }
            OnLevelUpdated?.Invoke();
        }

    }
