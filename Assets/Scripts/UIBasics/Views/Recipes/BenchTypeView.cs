using UnityEngine;

namespace UIBasics.Views.Recipes
{
    public class BenchTypeView : MonoBehaviour
    {
        [SerializeField]
        private BenchType _benchType;

        public BenchType BenchType => _benchType;
    }
}