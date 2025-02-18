using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.Recipes
{
    public class RecipesWindowView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _header;
        [SerializeField]
        private GameObject _main;
        [SerializeField]
        private GameObject _window;
        [SerializeField]
        private RecipeDescriptionView[] _recipeDescriptions;
        [SerializeField]
        private RectTransform _recipesRect;
        [SerializeField]
        private GridLayoutGroup _gridLayout;

        private RecipeService _recipeService;
        private int _currentBenchId;
        private Vector2 _bench0CellSize;
        private Vector2 _bench1CellSize;

        [Inject]
        public void Init(RecipeService recipeService)
        {
            _recipeService = recipeService;
            _bench0CellSize = _gridLayout.cellSize;
            _bench1CellSize = new Vector2(_bench0CellSize.x * 2 + _gridLayout.spacing.x, _bench0CellSize.y);
        }
        
        public void Show(int benchId)
        {
            _currentBenchId = benchId;
            _recipeService.OnRecipeUnlocked += UpdateValues;
            UpdateValues();
            
            _main.SetActive(true);
            _window.SetActive(true);
        }

        private void UpdateValues(int bench, int recipe)
        {
            UpdateValues();
        }

        private void UpdateValues()
        {
            _header.text = _currentBenchId == 0 ? "Workbench Recipes" : "Craft Table Recipes";
            if (_currentBenchId == 0)
            {
                _gridLayout.constraintCount = 2;
                _gridLayout.cellSize = _bench0CellSize;
            } else
            {
                _gridLayout.constraintCount = 1;
                _gridLayout.cellSize = _bench1CellSize;
            }
            var recipes = _recipeService.GetOpenedRecipes(_currentBenchId, out int nextRecipeIndex);

            int count = 0;
            for (int i = 0; i < recipes.Count; i++)
            {
                var openedRecipe = recipes[i];
                var description = _recipeDescriptions[i];
                description.gameObject.SetActive(true);
                description.SetRecipe(openedRecipe, _currentBenchId);
                count++;
            }

            int nextStartIndex = recipes.Count;
            if (nextRecipeIndex != -1)
            {
                var description = _recipeDescriptions[recipes.Count];
                description.SetRecipe(nextRecipeIndex, _currentBenchId, true); 
                description.gameObject.SetActive(true);
                nextStartIndex = recipes.Count + 1;
                count++;
            }
            
            for (int i = nextStartIndex; i < _recipeDescriptions.Length; i++)
            {
                var hidden = _recipeDescriptions[i];
                hidden.gameObject.SetActive(false);
            }

            float height =
                (_gridLayout.cellSize.y + _gridLayout.spacing.y) *
                (_currentBenchId == 0 ? Mathf.Ceil(count / 2f) : count) - _gridLayout.spacing.y;
            height = Mathf.Max(1115.5f, height);
            _recipesRect.sizeDelta = new Vector2(_recipesRect.sizeDelta.x, height);
            _recipesRect.anchoredPosition = new Vector2(_recipesRect.anchoredPosition.x, 0);
        }

        
        public void Close()
        {
            _recipeService.OnRecipeUnlocked -= UpdateValues;
            _main.SetActive(false);
            _window.SetActive(false);
        }
    }
}