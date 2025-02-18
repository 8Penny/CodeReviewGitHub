using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "RecipeList", menuName = "Configs/RecipeList")]
    public class RecipeList : ScriptableObject
    {
        public List<RecipeSettings> FirstBenchRecipes;
        public List<RecipeSettings> SecondBenchRecipes;
        public Dictionary<int, RecipeSettings> FirstBenchRecipesDict;
        public Dictionary<int, RecipeSettings> SecondBenchRecipesDict;
        
        private List<RecipeSettings> _sortedFirstBenchRecipes;
        private List<RecipeSettings> _sortedSecondBenchRecipes;
        
        public List<RecipeSettings> SortedFirstBenchRecipes => _sortedFirstBenchRecipes;
        public List<RecipeSettings> SortedSecondBenchRecipes => _sortedSecondBenchRecipes;

        public void Init()
        {
            _sortedFirstBenchRecipes = FirstBenchRecipes.OrderBy(r => r.Weight).ToList();
            _sortedSecondBenchRecipes = SecondBenchRecipes.OrderBy(r => r.Weight).ToList();

            FirstBenchRecipesDict = InitDictionary(FirstBenchRecipes);
            SecondBenchRecipesDict = InitDictionary(SecondBenchRecipes);
        }

        private Dictionary<int, RecipeSettings>  InitDictionary(List<RecipeSettings> raw)
        {
            var d = new Dictionary<int, RecipeSettings>();
            foreach (var r in raw)
            {
                d[r.Id] = r;
            }

            return d;
        }

        public RecipeSettings GetRecipe(int benchId, int recipeId)
        {
            if (benchId == 0)
            {
                return FirstBenchRecipesDict[recipeId];
            }
            return SecondBenchRecipesDict[recipeId];
        }
    }
}