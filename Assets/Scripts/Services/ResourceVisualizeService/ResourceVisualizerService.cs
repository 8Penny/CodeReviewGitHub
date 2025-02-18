using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Services.ResourceVisualizeService
{
    public class ResourceVisualizerService
    {
        private List<VisualizeComponent> _visualizers = new List<VisualizeComponent>();
        
        private Dictionary<int, Transform> _points = new Dictionary<int, Transform>();

        public void Register(VisualizeComponent component)
        {
            _visualizers.Add(component);
        }

        public void RegisterPoint(Transform point, List<int> castleIds)
        {
            foreach (var id in castleIds)
            {
                _points[id] = point;
            }
        }

        public void Play(ResourcesHolder resourcesHolder, int id)
        {
            foreach (var visualizer in _visualizers)
            {
                if (!visualizer.IsActive)
                {
                    if (!_points.TryGetValue(id, out var point))
                    {
                        point = _points.Values.First();
                    }
                    visualizer.Show(resourcesHolder, point);
                    return;
                }
            }
        }
    }
}