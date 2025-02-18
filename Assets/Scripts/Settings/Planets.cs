using System.Collections.Generic;
using Services;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "CastlesConfig", menuName = "Configs/Planets")]
    public class Planets : ScriptableObject
    {
        public List<PlanetSettings> PlanetsList;
        
        public Dictionary<int, PlanetSettings> PlanetsDict;
        private List<PlanetSettings> _availableOnStart;

        public void Init(SettingsService settingsService)
        {
            _availableOnStart = new List<PlanetSettings>();
            PlanetsDict = new Dictionary<int, PlanetSettings>();
            foreach (var planet in PlanetsList)
            {
                PlanetsDict[planet.Id] = planet;
                if (planet.AvailableOnStart)
                {
                    _availableOnStart.Add(planet);
                }
                
                planet.Init(settingsService);
            }
        }

        public List<PlanetSettings> GetAvailableOnStart()
        {
            return _availableOnStart;
        }
    }
}