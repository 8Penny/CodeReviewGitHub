using System;
using System.Collections.Generic;
using Static;
using UnityEngine;
using Zenject;

namespace Services.Boosts
{
    public class BoostService
    {
        private const int MiningBoostTime = 60 * 60;
        private const int CastleBoostTime = 5 * 60;
        private const int CraftBoostTime = 5 * 60;
        
        private const float MiningBoostValue = 1.2f;
        private const float CastleBoostValue = 10f;
        private const float CraftBoostValue = 10f;
        public const int SoftRewardValue = 100000;
        
        private PlayerDataManager _dataManager;
        private PlayerResourcesService _playerResourcesService;
        private TickService _tickService;

        public List<BoostType> ActiveBoosts { get; private set; }

        private Dictionary<BoostType, int> _boostTime = new Dictionary<BoostType, int>();
        public long[] Boosts => _dataManager.Activated.Boosts;
        public int BoostEntityId => _dataManager.Parameters.BoostParameter;
        public float MiningBoost => IsActive(BoostType.MiningSpeedBoost) ? MiningBoostValue : 1f;
        public float CraftBoost => IsActive(BoostType.CraftBoost) ? CraftBoostValue : 1f;
        public Dictionary<BoostType, int> BoostTime => _boostTime;
        public bool Triggered;
        public Action<BoostType, int> OnActivated;
        
        [Inject]
        public BoostService(PlayerDataManager dataManager, TickService tickService, PlayerResourcesService playerResourcesService)
        {
            _dataManager = dataManager;
            _tickService = tickService;
            _playerResourcesService = playerResourcesService;
            
            _tickService.SetService(this);
            ActiveBoosts = new List<BoostType>();
            
            _boostTime[BoostType.MiningSpeedBoost] = MiningBoostTime;
            _boostTime[BoostType.CastleBoost] = CastleBoostTime;
            _boostTime[BoostType.CraftBoost] = CraftBoostTime;
        }
        
        public float GetPlanetBoost(int id)
        {
            if (BoostEntityId != id)
            {
                return 1f;
            }
            return IsActive(BoostType.CastleBoost) ? CastleBoostValue : 1f;
        }
        public bool IsActive(BoostType boostType, long currentTime)
        {
            return Boosts[(int) boostType] > currentTime;
        }

        public bool IsActive(BoostType boostType)
        {
            return ActiveBoosts.Contains(boostType);
        }

        public void ActivateBoost(BoostType boostType, int entityId = 0)
        {
            OnActivated?.Invoke(boostType, entityId);
            if (boostType == BoostType.SoftBoost)
            {
                _playerResourcesService.AddResource(ResourceNames.Soft, SoftRewardValue);
                return;
            }
            Boosts[(int) boostType] = _tickService.Tick + _boostTime[boostType] - 1;
            if (boostType == BoostType.CastleBoost)
            {
                _dataManager.Parameters.BoostParameter = entityId;
            }

            Triggered = true;
        }

        public void CalculateActiveBoosts(long currentTime)
        {
            ActiveBoosts.Clear();
            foreach (var key in _boostTime.Keys)
            {
                if (IsActive(key, currentTime)){
                    ActiveBoosts.Add(key); 
                }
            }
        }

        public void UpdateLocalParameters()
        {
            Triggered = false;
            CalculateActiveBoosts(_tickService.Tick);
        }

        public float GetBoostRatio(BoostType boostType)
        {
            return (Boosts[(int) boostType] - _tickService.Tick) / (float)_boostTime[boostType];
        }
        public float GetTime(BoostType boostType)
        {
            return Boosts[(int) boostType] - _tickService.Tick;
        }
    }
}