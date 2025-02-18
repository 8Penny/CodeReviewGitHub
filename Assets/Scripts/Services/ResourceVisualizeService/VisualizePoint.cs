using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Services.ResourceVisualizeService
{
    public class VisualizePoint : MonoBehaviour
    {
        [SerializeField]
        private List<int> _castleIds;

        public List<int> CastleIds => _castleIds;
        
        [Inject]
        public void Init(ResourceVisualizerService visualizerService)
        {
            visualizerService.RegisterPoint(transform, _castleIds);
        }
    }
}