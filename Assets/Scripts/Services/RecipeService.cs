using System;
using System.Collections.Generic;
using Services.Craft;
using Services.Tutorial;
using Settings;
using Static;
using Zenject;

namespace Services
{
    public class RecipeService
    {
        private PlayerDataManager _dataManager;
        private SettingsService _settingsService;
        private PlayerResourcesService _playerResourcesService;
        private TutorialService _tutorialService;
        private UIService _uiService;

        public Action<int, int> OnRecipeUnlocked;

        private CraftEntityPresenter _currentCraftEntity;
        
        [Inject]
        public RecipeService(PlayerDataManager dataManager, SettingsService settingsService,
            PlayerResourcesService playerResourcesService, TutorialService tutorialService, UIService uiService)
        {
            _uiService = uiService;
            _dataManager = dataManager;
            _settingsService = settingsService;
            _playerResourcesService = playerResourcesService;
            _tutorialService = tutorialService;
        }

        public List<int> GetOpenedRecipes(int benchId, out int nextRecipeIndex)
        {
            nextRecipeIndex = -1;
            var recipes = benchId == 0
                ? _settingsService.Recipes.SortedFirstBenchRecipes
                : _settingsService.Recipes.SortedSecondBenchRecipes;

            var lastOpened = benchId == 0
                ? _dataManager.Parameters.WorkbenchParameter
                : _dataManager.Parameters.CraftTableParameter;

            var result = new List<int>();
            for(int i = 0; i < recipes.Count; i++)
            {
                if (lastOpened < 0)
                {
                    nextRecipeIndex = 0;
                    break;
                }
                var r = recipes[i];
                result.Add(r.Id);
                if (r.Id == lastOpened)
                {
                    if (r.Id < recipes.Count - 1)
                    { 
                        nextRecipeIndex = recipes[i+1].Id;  
                    }
                    break;
                }
            }
            return result;
        }

        public RecipeSettings GetRecipe(int benchId, int index)
        {
            if (benchId == 0)
            {
                return _settingsService.Recipes.FirstBenchRecipesDict[index];
            }
            
            return _settingsService.Recipes.SecondBenchRecipesDict[index];
        }

        public void SetCurrentPresenter(CraftEntityPresenter p)
        {
            _currentCraftEntity = p;
        }
        
        public void Interact(int bench, RecipeSettings current, bool isLocked, out bool canClose)
        {
            canClose = false;
            if (isLocked)
            {
                if (_playerResourcesService.TryBuy(new ResourceDemand(ResourceNames.Soft, current.RecipePrice)))
                {
                    Unlock(bench, current.Id);
                    _currentCraftEntity.SetRecipe(current.Id);
                }
                else
                {
                    _uiService.OpenGoToShopPopup();
                }
                return;
            }
            
            _currentCraftEntity.SetRecipe(current.Id);
            _tutorialService.TryUpdateRecipeSetStep();
            canClose = true;
        }

        private void Unlock(int bench, int recipe)
        {
            if (bench == 0)
            {
                _dataManager.Parameters.WorkbenchParameter = recipe;
            }
            else
            {
                _dataManager.Parameters.CraftTableParameter = recipe;
            }
            OnRecipeUnlocked?.Invoke(bench, recipe);
        }
        
    }
}