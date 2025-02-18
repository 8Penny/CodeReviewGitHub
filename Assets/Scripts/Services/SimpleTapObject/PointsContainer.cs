using System.Collections.Generic;
using Services.TapBonus;
using UnityEngine;
using Zenject;

namespace Services.SimpleTap
{
    public class PointsContainer : MonoBehaviour
    {
        private TapBonusService _tapBonusService;
        
        [Inject]
        public void Init(TapBonusService tapBonusService)
        {
            _tapBonusService = tapBonusService;
        }
        
        private void Awake()
        {
            var result = new List<Transform>();
            var children = GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i] == transform)
                {
                    continue;
                }
                result.Add(children[i]);
            }
            _tapBonusService.RegisterPoints(result);
            gameObject.SetActive(false);
        }
    }
}