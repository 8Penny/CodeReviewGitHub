using System;
using System.Collections.Generic;
using Services.Boosts;
using Services.Talents;
using Settings;
using Static;
using BenchType = UIBasics.Views.Recipes.BenchType;

namespace Services.Craft
{
    public class CraftService : IDisposable
    {
        public const int BENCH_MAX_COUNT = 6;
        
        private BoostService _boostService;
        private TickService _tickService;
        private PlayerResourcesService _resourcesService;
        private PlayerDataManager _dataManager;
        private TalentsService _talentsService;
        private RecipeService _recipeService;
        private UIService _uiService;
        
        private Dictionary<CraftEntity, CraftEntityPresenter> _workbenchPresenters = new Dictionary<CraftEntity, CraftEntityPresenter> ();
        private Dictionary<CraftEntity, CraftEntityPresenter> _craftTablePresenters = new Dictionary<CraftEntity, CraftEntityPresenter> ();
        
        public Dictionary<CraftEntity, CraftEntityPresenter> Workbenches => _workbenchPresenters;
        public Dictionary<CraftEntity, CraftEntityPresenter> CraftTables => _craftTablePresenters;

        public Action OnBenchAdded;
        
        public CraftService(PlayerResourcesService playerResourcesService,
            PlayerDataManager playerDataManager,
            TalentsService talentsService,
            RecipeService recipeService,
            BoostService boostService,
            TickService tickService, UIService uiService)
        {
            _uiService = uiService;
            _resourcesService = playerResourcesService;
            _dataManager = playerDataManager;
            _talentsService = talentsService;
            _recipeService = recipeService;
            _boostService = boostService;
            _tickService = tickService;

            _talentsService.OnAddedAbility += AbilityAddedHandler;
        }

        public void Dispose()
        {
            _talentsService.OnAddedAbility -= AbilityAddedHandler;
        }

        public int GetBenchId(BenchType t)
        {
            return t == BenchType.Workshop ? 0 : 1;
        }

        public int GetBenchPrice(int id, bool isFirst)
        {
            if (id == 1 && isFirst)
            {
                return 100000;
            }
            return (id) * 500000 + (isFirst ? 0 : 1000000);
        }
        
        public void UpdateBenches()
        {
            UpdateBenchType(_dataManager.Updatables.Workbenches, _workbenchPresenters);
            UpdateBenchType(_dataManager.Updatables.CraftTables, _craftTablePresenters);
        }

        private void UpdateBenchType(List<CraftEntity> craftEntities, Dictionary<CraftEntity, CraftEntityPresenter> presenters)
        {
            foreach (var entity in craftEntities)
            {
                if (presenters.ContainsKey(entity))
                {
                    continue;
                }

                var presenter = new CraftEntityPresenter();
                presenter.SetSettings(_resourcesService, _recipeService, _talentsService, _boostService, entity);
                presenters[entity] = presenter;
                _tickService.AddUpdatable(presenter);
            }
        }

        private void AbilityAddedHandler(AbilityType id)
        {
            AddBenches(id);
        }
        
        private void AddBenches(AbilityType id)
        {
            bool wasAnyAdded = false;
            if (id == AbilityType.Workbench1)
            {
                _dataManager.Updatables.Workbenches.Add(new CraftEntity(0, 0));
                wasAnyAdded = true;
            }
            if (id == AbilityType.CraftTable1)
            {
                _dataManager.Updatables.CraftTables.Add(new CraftEntity(1, 0));
                wasAnyAdded = true;
            }
            if (wasAnyAdded)
            {
                UpdateBenches();
            }
        }

        public bool TryBuyNewSlot(int benchType)
        {
            var benches = benchType == 0
                ? _dataManager.Updatables.Workbenches
                : _dataManager.Updatables.CraftTables;
            int currentCount = benches.Count;
            if (currentCount >= BENCH_MAX_COUNT)
            {
                return false;
            }
            
            if (_resourcesService.TryBuy(new ResourceDemand(ResourceNames.Soft, GetBenchPrice(currentCount, benchType == 0))))
            {
                benches.Add(new CraftEntity(benchType, currentCount));
                
                UpdateBenches();
                OnBenchAdded?.Invoke();
                return true;
            }
            _uiService.OpenGoToShopPopup();

            return false;
        }
        
    }
}