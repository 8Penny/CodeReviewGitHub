using System.Collections.Generic;
using Services;
using Settings;
using UnityEngine;
using Zenject;

namespace Views
{
    public class ResourceHolder : MonoBehaviour
    {
        [SerializeField]
        private CastleResourceView[] _castleResourceViews;

        private SettingsService _settingsService;
        private int _count;

        [Inject]
        public void Init(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public void UpdateVisibility(bool isVisible)
        {
            for (int i = 0; i < _count; i++)
            {
                _castleResourceViews[i].gameObject.SetActive(isVisible);
            }
            
            for (int i = _count; i < _castleResourceViews.Length; i++)
            {
                _castleResourceViews[i].gameObject.SetActive(false);
            }
        }

        public void SetResources(List<PlanetResource> planetResource)
        {
            _count = 0;
            for (int i = 0; i < planetResource.Count; i++)
            {
                _castleResourceViews[i].SetSprite(_settingsService.GameResources[planetResource[i].ResourceId].Sprite);
                _count++;
            }
        }
    }
}