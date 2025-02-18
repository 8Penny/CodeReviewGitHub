
using System.Collections.Generic;
using Services;
using Services.Sounds;
using Services.Talents;
using Settings;
using TMPro;
using UnityEngine;
using Zenject;

namespace UIBasics.Views.Recipes
{
    public class RecipeBlockView : MonoBehaviour
    {
        [SerializeField]
        private List<RecipeItemView> _recipeViews;
        [SerializeField]
        private TextMeshProUGUI _progressLabel;
        [SerializeField]
        private RectTransform _progressRect;
        [SerializeField]
        private GameObject _progressGo;

        private SettingsService _settingsService;
        private PlayerResourcesService _playerResourcesService;
        private SoundService _soundService;
        private TalentsService _talentsService;

        private bool _needUpdate;
        private int _benchIndex;
        private RecipeSettings _recipe;

        private float _demandMultiplier => _benchIndex == 0
            ? _talentsService.WorkbenchDemandMultiplier
            : _talentsService.CraftTableDemandMultiplier;

        public RecipeSettings Recipe => _recipe;

        [Inject]
        public void Init(SettingsService settingsService, PlayerResourcesService playerResourcesService,
            SoundService soundService, TalentsService talentsService)
        {
            _soundService = soundService;
            _settingsService = settingsService;
            _playerResourcesService = playerResourcesService;
            _talentsService = talentsService;
        }
        
        public void SetRecipe(int benchIndex, int recipeIndex, bool needUpdate = false)
        {
            _needUpdate = needUpdate;
            if (recipeIndex == -1)
            {
                _needUpdate = false;
                return;
            }

            _benchIndex = benchIndex;
            _recipe =_settingsService.Recipes.GetRecipe(benchIndex, recipeIndex);
            for (int i = 0; i < _recipeViews.Count - 1; i++)
            {
                var view = _recipeViews[i];
                if (view == null)
                {
                    continue;
                }
                bool hasDemand = !(_recipe.Demands.Count <= i);

                view.UpdateVisibility(hasDemand);
                if (!hasDemand)
                {
                    continue;
                }

                var demand = _recipe.GetBoostedDemands(_demandMultiplier)[i];
                view.SetItem(true, demand.Value, demand.ResourceId);
            }

            var resultView = _recipeViews[^1];
            resultView.SetItem(false, 1, _recipe.RecipeResourceId);
        }

        public void Update()
        {
            if (!_needUpdate)
            {
                return;
            }
            
            for (int i = 0; i < _recipe.Demands.Count; i++)
            {
                var view = _recipeViews[i];
                var demand = _recipe.GetBoostedDemands(_demandMultiplier)[i];
                view.UpdateCount(true, _playerResourcesService.GetResource(demand.ResourceId), demand.Value);
            }
            
            //_recipeViews[3].UpdateCount(false, Mathf.FloorToInt(_playerPocketService.GetResource(_recipe.ResultResourceId)), 0);
        }

        public void SetProgress(string value, float progressValue, bool isWorking)
        {
            if (!isWorking)
            {
                _progressLabel.text = "Not enough resources";
                _progressRect.localScale = Vector3.zero;
                _progressGo.SetActive(false);
                return;
            }

            if (!_progressGo.activeSelf)
            {
                _progressGo.SetActive(true);
            }
            _progressLabel.text = value;
            _progressRect.localScale = new Vector2(progressValue, 1);
        }
    }
}