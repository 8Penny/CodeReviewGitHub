using System;
using System.Collections.Generic;
using Services.Ads;
using Services.Boosts;
using Services.Talents;
using Services.Tutorial;
using Services.Updater;
using UnityEngine;
using Zenject;

namespace Services
{
    public partial class TickService : MonoBehaviour, IUpdatable, IDisposable
    {
        private UpdateService _updateService;
        private PlayerDataManager _dataManager;
        private UIService _uiService;
        private BoostService _boostService;
        private PlayerResourcesService _resourcesService;
        private TalentsService _talentsService;
        private TutorialService _tutorialService;
        private AdsShowSystem _adsShowSystem;
        
        private List<ITickUpdatable> _updatables = new List<ITickUpdatable>();
        private List<ITickUpdatable> _newUpdatables = new List<ITickUpdatable>();
        
        private int _maxInactiveSeconds;
        private long _tick;
        private long _startTick;
        
        private long _deltaTime;
        private bool _onPause;
        private bool _wasOnPause;

        public long Tick => _tick;
        
        [Inject]
        public void Init(UpdateService updateService, SettingsService settingsService,
            PlayerDataManager dataManager, PlayerResourcesService resourcesService,
            UIService uiService, TalentsService talentsService, TutorialService tutorialService,
            AdsShowSystem adsShowSystem)
        {
            _adsShowSystem = adsShowSystem;
            _tutorialService = tutorialService;
            _resourcesService = resourcesService;
            _updateService = updateService;
            _dataManager = dataManager;
            _uiService = uiService;
            _talentsService = talentsService;

            _updateService.Register(this);
            _dataManager.OnPreSaveStep += OnPreSaveStepHandler;
        }
        
        public void Dispose()
        {
            _dataManager.OnPreSaveStep -= OnPreSaveStepHandler;
            _updateService.Unregister(this);
        }

        private void OnPreSaveStepHandler()
        {
            _onPause = true;
            _dataManager.Parameters.Tick = _tick;
        }

        public void Init()
        {
            _onPause = false;
        }

        public void PreInitStep()
        {
            if (_dataManager.Parameters.Tick == 0)
            {
                _tick = CommonUtils.UnixTime();
                return;
            }

            _startTick = _dataManager.Parameters.Tick;
            _deltaTime = CommonUtils.UnixTime() - _startTick;
            
            _wasOnPause = true;
        }
        
        public void OnApplicationFocus(bool hasFocus)
        {
            ProcessPause(!hasFocus);
        }

        public void OnApplicationPause(bool pause)
        {
            ProcessPause(pause);
        }

        private void ProcessPause(bool pause)
        {
            #if UNITY_EDITOR
            if (pause)
            {
                return;
            }
            #endif
            if (pause != _onPause && _onPause)
            {
                _deltaTime = CommonUtils.UnixTime() - _tick;
                _startTick =_tick;
                _wasOnPause = true;
            }
            
            _onPause = pause;
            if (_onPause)
            {
                _tick = CommonUtils.UnixTime();
                if (_dataManager == null || _dataManager.Parameters == null)
                {
                    return;
                }
                _dataManager.Parameters.Tick = Tick;
                _dataManager.StoreData();
            }
        }
        
        public void SetService(BoostService boosts)
        {
            _boostService = boosts;
        }
    }
}