using System;
using Services.Boosts;
using Services.Talents;
using Settings;
using UnityEngine;

namespace Services.Craft
{
    public class CraftEntityPresenter : ITickUpdatable
    {
        private PlayerResourcesService _playerResourcesService;
        private RecipeService _recipeService;
        private RecipeSettings _recipeSettings;
        private TalentsService _talentsService;
        private BoostService _boostsService;

        private CraftEntity _model;
        
        private float _demandMultiplier => _model.CraftTypeId == 0
            ? _talentsService.WorkbenchDemandMultiplier
            : _talentsService.CraftTableDemandMultiplier;

        public CraftEntity Model => _model;
        public int CraftTypeId => _model.CraftTypeId;
        public int RecipeId => _model.RecipeId;
        public float Progress => Mathf.Clamp(_model.CraftTime / GetFullTime(), 0f, 1f);
        public bool HasRecipe => _model.RecipeId >= 0;
        public int Weight => 1;
        public Action WasUpdated;

        public void SetSettings(PlayerResourcesService playerResourcesService, RecipeService recipeService,
            TalentsService talentsService, BoostService boostService, CraftEntity model)
        {
            _playerResourcesService = playerResourcesService;
            _talentsService = talentsService;
            _recipeService = recipeService;
            _boostsService = boostService;

            _model = model;
        }

        public void SetRecipe(int recipeId)
        {
            if (recipeId == RecipeId)
            {
                return;
            }

            FreeFrozenResources();
            _model.SetRecipeId(recipeId);
            _model.SetCraftTime(0);

            if (HasRecipe)
            {
                _recipeSettings = _recipeService.GetRecipe(CraftTypeId, RecipeId);
            }

            WasUpdated?.Invoke();
        }

        private void FreeFrozenResources()
        {
            foreach (var item in _model.Frozen.Values)
            {
                _playerResourcesService.AddResource(item.Key, item.Value);
            }

            _model.Frozen.Values.Clear();
        }

        public void Update(float seconds)
        {
            if (!HasRecipe)
            {
                return;
            }

            _recipeSettings ??= _recipeService.GetRecipe(CraftTypeId, RecipeId);

            if (_model.Frozen.Values.Count == 0)
            {
                if (!TryFreezeResources())
                {
                    _model.SetCraftTime(0);
                    return;
                }
            }

            UpdateCraftFrozenResources(seconds);
        }

        private float GetFullTime()
        {
            return _recipeSettings.Time *
                   (CraftTypeId == 0 ? _talentsService.WorkbenchMultiplier : _talentsService.CraftTableMultiplier) /
                   _boostsService.CraftBoost;
        }

        private void TransferResourcesToPlayer()
        {
            _playerResourcesService.AddResource(_recipeSettings.RecipeResourceId, 1);
            _model.Frozen.Values.Clear();
        }

        private bool TryFreezeResources()
        {
            if (_playerResourcesService.TryBuy(_recipeSettings.GetBoostedDemands(_demandMultiplier)))
            {
                foreach (var demand in _recipeSettings.GetBoostedDemands(_demandMultiplier))
                {
                    _model.Frozen.AddResource(demand.ResourceId, demand.Value);
                }

                return true;
            }

            return false;
        }

        public bool CanContinueWork()
        {
            if (_model.Frozen.Values.Count > 0 ||
                _playerResourcesService.CanBuy(_recipeSettings.GetBoostedDemands(_demandMultiplier)))
            {
                return true;
            }

            return false;
        }

        private void UpdateCraftFrozenResources(float seconds)
        {
            _model.SetCraftTime(_model.CraftTime + seconds);
            
            float recipeTime = GetFullTime();
            while (_model.CraftTime > recipeTime)
            {
                _model.SetCraftTime(_model.CraftTime - recipeTime);
                TransferResourcesToPlayer();
                if (!TryFreezeResources())
                {
                    _model.SetCraftTime(0);
                    break;
                }
            }
        }
    }
}