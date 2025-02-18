using System;
using System.Collections.Generic;
using Services.Analytics;
using Services.Sounds;
using Services.Tasks;
using Services.Tutorial;
using Services.Updater;
using Static;
using UnityEngine;
using Views;
using Zenject;
using IUpdatable = Services.Updater.IUpdatable;

namespace Services.View
{
    public class ViewCastlesService : IUpdatable, IDisposable
    {
        private readonly PlayerResourcesService _playerResources;
        private readonly UIService _uiService;
        private readonly TaskService _taskService;
        private readonly SoundService _soundService;
        private readonly UpdateService _updateService;
        private readonly TutorialService _tutorialService;
        private readonly AnalyticsService _analytics;
        private readonly PlayerDataManager _playerDataManager;
        
        private Dictionary<int, CastleView> _views = new Dictionary<int, CastleView>();
        private Dictionary<int, Castle> _presenters = new Dictionary<int, Castle>();

        private int _lastShownCastleId;
        private bool _freeze;

        [Inject]
        public ViewCastlesService(PlayerResourcesService playerResources, UIService uiService,
            TaskService taskService, SoundService soundService, UpdateService updateService,
            TutorialService tutorialService, AnalyticsService analytics, PlayerDataManager playerDataManager)
        {
            _playerResources = playerResources;
            _uiService = uiService;
            _taskService = taskService;
            _soundService = soundService;
            _updateService = updateService;
            _tutorialService = tutorialService;
            _analytics = analytics;
            _playerDataManager = playerDataManager;
            
            updateService.Register(this);
        }

        public void Dispose()
        {
            _updateService.Unregister(this);
        }
        public void Register(CastleView view)
        {
            if (_views.ContainsKey(view.Id))
            {
                return;
            }
            
            _views.Add(view.Id, view);
        }

        public void UpdateCastleViews()
        {
            foreach (var presenter in _playerDataManager.Updatables.Castles.Values)
            {
                UpdateCastle(presenter);
            }

            foreach (var view in _views)
            {
                if (!view.Value.HasPresenter)
                {
                    view.Value.UpdateState(true, false);
                }
            }
            _presenters = _playerDataManager.Updatables.Castles;
        }

        private void UpdateCastle(Castle presenter)
        {
            if (!_views.ContainsKey(presenter.Id))
            {
                Debug.LogError($"There is no planet view with {presenter.Id} id");
                return;
            }
            var currentView = _views[presenter.Id];
            currentView.SetPresenter(presenter); 
            currentView.SetUnlockPrice(presenter.Settings.UnlockPrice);
            currentView.UpdateState(false, presenter.IsOpen);// todo : is planet hidden? 
        }

        public void InteractCastle(int id)
        {
            if (!_tutorialService.IsComplete && id > 0 || _tutorialService.TutorialStep == 0)
            {
                return;
            }
            if (!_presenters.TryGetValue(id, out var presenter))
            {
                return;
            }

            if (_uiService.ChoosingCastle)
            {
                if (presenter.IsOpen)
                {
                    _uiService.Views.ChooseCastleBoost.ChooseCastle(id);
                }
                return;
            }
            
            if (presenter.IsOpen)
            {
                _tutorialService.HalfStep();
                ShowCastleWindow(presenter);
                _soundService.PlayCastleClick();
                return;
            }

            if (_freeze)
            {
                return;
            }

            if (CanBuyCastle(presenter.Settings.UnlockPrice))
            {
                _freeze = true;
                _views[presenter.Id].PlayOpen();
                _soundService.PlayOpenCastleSound();
                if (presenter.Id == 0)
                {
                    _tutorialService.UnlockFirstCastle();
                }
            }
        }

        private void ShowCastleWindow(Castle presenter)
        {
            _lastShownCastleId = presenter.Id;
            _uiService.ShowCastlePanel(presenter);
        }

        private bool TryBuyCastle(int price)
        {
            return _playerResources.TryBuy(new ResourceDemand(ResourceNames.Soft, price));
        }
        private bool CanBuyCastle(int price)
        {
            return _playerResources.CanBuy(new ResourceDemand(ResourceNames.Soft, price));
        }

        public void NextCastle(bool next)
        {
            _soundService.PlayClick();
            int previousId = _lastShownCastleId;
            int id;
            while (true)
            {
                id = next ? previousId + 1 : previousId - 1;
                if (id < 0 || id >= _presenters.Count)
                {
                    id = next ? 0 : _presenters.Count - 1;  
                }
                if (_presenters.TryGetValue(id, out var presenter) && presenter.IsOpen)
                {
                    ShowCastleWindow(presenter);
                    return;
                }

                previousId = id;
            }
        }

        public void Update()
        {
            foreach (var view in _views)
            {
                if (!view.Value.CanOpen)
                {
                    continue;
                }

                _freeze = false;
                view.Value.CanOpen = false;
                var presenter = _presenters[view.Value.Id];
                if (!TryBuyCastle(presenter.Settings.UnlockPrice))
                {
                    continue;
                }
                presenter.Open();
                _analytics.OnCastleUnlocked(presenter.Id);
                UpdateCastle(presenter);
                _taskService.OnCastleUnlocked();
            }
        }
    }
}