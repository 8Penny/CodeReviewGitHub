using System.Linq;
using Services.Updater;
using Static;

namespace Services
{
    public partial class TickService
    {
        private bool _locked;
        
        private void UpdateEntities(float seconds)
        {
            foreach (var e in _updatables)
            {
                e.Update(seconds);
            }

            AddNewEntities();
        }

        public void AddNewEntities()
        {
            if (_newUpdatables.Count == 0)
            {
                return;
            }

            foreach (var e in _newUpdatables)
            {
                if (_updatables.Contains(e))
                {
                    continue;
                }
                _updatables.Add(e);
            }

            _newUpdatables.Clear();
            _updatables = _updatables.OrderByDescending(t => t.Weight).ToList();
        }
        
        public void AddUpdatable(ITickUpdatable unit)
        {
            _newUpdatables.Add(unit);
        }

        void IUpdatable.Update()
        {
            if (_onPause)
            {
                return;
            }

            long currentTime = CommonUtils.UnixTime();
            
            if (_wasOnPause && _updatables.Count != 0)
            {
                CalculateResourcesWhileInactive(currentTime);
                return;
            }

            if (currentTime > _tick || _boostService.Triggered)
            {
                _tick = currentTime;
                _boostService.UpdateLocalParameters();
            }

            UpdateEntities(UnityEngine.Time.deltaTime);
        }

        private void CalculateResourcesWhileInactive(long currentTime)
        {
            _maxInactiveSeconds = StaticValues.SecondsInMinute * (int) _talentsService.InactiveMinutes;
            if (_deltaTime > _maxInactiveSeconds)
            {
                _deltaTime = _maxInactiveSeconds;
            }

            float rawDeltaTime = _deltaTime;
            
            _resourcesService.FreezePocket();

            CalculateWithBoosts(currentTime);

            _boostService.CalculateActiveBoosts(_startTick + (long) rawDeltaTime);
            UpdateEntities(_deltaTime);

            bool needShow = _tutorialService.IsComplete && _deltaTime > StaticValues.SecondsInMinute * 3;
            if (needShow && _resourcesService.FrozenResourcesHolder.Values.Any(t => t.Value > 1f))
            {
                _uiService.ShowWelcomeBack(_resourcesService.FrozenResourcesHolder, (int) rawDeltaTime);
                _adsShowSystem.UpdateSessionParameters();
            }
            
            _resourcesService.UnfreezePocket();
            if (!needShow)
            {
                _resourcesService.TransferFreezeResources();
            }

            _wasOnPause = false;
        }

        private void CalculateWithBoosts(long currentTime)
        {
            long boostTimeSum = 0;
            foreach (var boostEndTime in _boostService.Boosts.Where(t => t > _startTick && t < currentTime)
                         .OrderBy(t => t))
            {
                long boostTimePeriod = boostEndTime - _startTick - 1;
                _boostService.CalculateActiveBoosts(_startTick + boostTimePeriod);

                boostTimePeriod -= boostTimeSum;
                _deltaTime -= boostTimePeriod;
                UpdateEntities(boostTimePeriod);

                boostTimeSum += boostTimePeriod;
            }
        }
    }
}