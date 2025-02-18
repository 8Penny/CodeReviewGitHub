using System;
using System.Collections.Generic;
using Services;
using Services.Boosts;
using Services.ResourceVisualizeService;
using Services.Talents;
using Services.View;
using Settings;

namespace CastleService
{
    public class CastlesService : IDisposable
    {
        private PlayerDataManager _dataManager;
        private SettingsService _settingsService;
        private ViewCastlesService _viewCastlesService;
        private TickService _tickService;
        private BoostService _boostService;
        private ResourceVisualizerService _visualizerService;
        private TalentsService _talentsService;
        private PlayerResourcesService _resourcesService;
        
        private Dictionary<int, Castle> Castles => _dataManager.Updatables.Castles;
        
        public CastlesService(PlayerDataManager dataManager,
        SettingsService settingsService,
        ViewCastlesService viewCastlesService,
        TickService tickService,
        BoostService boostService,
        ResourceVisualizerService visualizerService,
        TalentsService talentsService,
        PlayerResourcesService resourcesService)
        {
            _dataManager = dataManager;
            _settingsService = settingsService;
            _viewCastlesService = viewCastlesService;
            _tickService = tickService;
            _boostService = boostService;
            _visualizerService = visualizerService;
            _talentsService = talentsService;
            _resourcesService = resourcesService;

            _talentsService.OnAddedAbility += AbilityAddedHandler;
        }

        public void Dispose()
        {
            _talentsService.OnAddedAbility -= AbilityAddedHandler;
        }

        private void InitCastles()
        {
            foreach (var planet in _settingsService.Castles.GetAvailableOnStart())
            {
                var newPlanet = new Castle();
                Castles.Add(planet.Id, newPlanet);
            }
        }
        private void AbilityAddedHandler(AbilityType id)
        {
            AddEntity(id);
        }

        private void AddEntity(AbilityType id)
        {
            bool wasAnyAdded = false;
            foreach (var planet in _settingsService.Castles.PlanetsList.FindAll(p => p.AbilityId == id))
            {
                if (Castles.ContainsKey(planet.Id))
                {
                    continue;
                }
                var newPlanet = new Castle();
                Castles.Add(planet.Id, newPlanet);
                wasAnyAdded = true;
            }

            if (wasAnyAdded)
            {
                UpdateCastles();
            }

            UpdateViews();
        }


        public void UpdateViews()
        {
            _viewCastlesService.UpdateCastleViews();
        }

        public void UpdateCastles()
        {
            if (Castles.Count == 0)
            {
                InitCastles();
            }

            foreach (var castle in Castles)
            {
                _tickService.AddUpdatable(castle.Value);
                castle.Value.SetSettings(_settingsService.Castles.PlanetsDict[castle.Key], _resourcesService,
                    _talentsService, _boostService, _visualizerService);
            }
        }
    }
}