using System.Collections.Generic;
using UnityEngine;

namespace Services.SimpleTap
{
    public class CartPath : MonoBehaviour
    {
        private List<Transform> _points = new List<Transform>();
        public List<Transform> Points => _points;
        private void Awake()
        {
            var transforms = GetComponentsInChildren<Transform>();
            for (int i = 1; i < transforms.Length; i++)
            {
                _points.Add(transforms[i]);
                transforms[i].gameObject.SetActive(false);
            }
        }
    }
}