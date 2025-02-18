using System;
using System.Collections.Generic;
using Settings;
using Static;
using UnityEngine;

namespace Services
{
    public class SettingsService
    {
        public Planets Castles;
        public AbilitiesTree AbilitiesTree;
        public MainGameSettings MainGameConfig;
        public RecipeList Recipes;
        public BoostsSettings BoostsSettings;
        public MainTasksSettings TasksConfig;
        public TutorialBranchSettings TutorialTaskSettings;
        public TutorialSettings TutorialConfig;
        public InfoSettings InfoSettings;
        public ShopSettings ShopSettings;
        public Dictionary<ResourceNames, Resource> GameResources = new Dictionary<ResourceNames, Resource>();

        
        public SettingsService()
        {
            var resources = Resources.Load<ResourcesList>("ResourcesList");
            foreach (var r in resources.Resources)
            {
                GameResources.Add(r.ResourceId, r);
            }
            
            TutorialConfig = Resources.Load<TutorialSettings>("TutorialConfig");
            TutorialConfig.Init();
            
            InfoSettings = Resources.Load<InfoSettings>("InfoSettings");
            InfoSettings.Init();
            
            ShopSettings = Resources.Load<ShopSettings>("ShopSettings");
            ShopSettings.Init();
            
            Castles = Resources.Load<Planets>("CastlesConfig");
            Castles.Init(this);
            
            AbilitiesTree = Resources.Load<AbilitiesTree>("AbilitiesTree");
            AbilitiesTree.Init();

            Recipes = Resources.Load<RecipeList>("RecipeList");
            Recipes.Init();
            
            TasksConfig = Resources.Load<MainTasksSettings>("TasksConfig");
            TasksConfig.Init();
            MainGameConfig = Resources.Load<MainGameSettings>("MainGameConfig");
            BoostsSettings = Resources.Load<BoostsSettings>("BoostsSettings");
            
            TutorialTaskSettings = Resources.Load<TutorialBranchSettings>("TutorialBranchSettings");
            TutorialTaskSettings.Init();
            
        }
    }
}