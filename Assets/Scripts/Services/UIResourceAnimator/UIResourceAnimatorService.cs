using System;
using System.Collections.Generic;
using Static;
using UIBasics.Views.Tutorial;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Services.UIResourceAnimator
{
    public class UIResourceAnimatorService : IDisposable
    {
        private UIService _uiService;
        private SettingsService _settingsService;
        private ResourceAnimator _prefab;

        private Sprite _soft;
        private Sprite _hard;

        public List<ResourceAnimator> _pool = new List<ResourceAnimator>();

        public UIResourceAnimatorService(UIService uiService, SettingsService settingsService)
        {
            _settingsService = settingsService;
            _uiService = uiService;
            _prefab = Resources.Load<ResourceAnimator>("UIRewards");

            _uiService.OnViewsRegistered += ViewsInitHandler;
        }

        public void Dispose()
        {
            _uiService.OnViewsRegistered -= ViewsInitHandler;
        }

        private void ViewsInitHandler()
        {
            _soft = _settingsService.GameResources[ResourceNames.Soft].Sprite;
            _hard = _settingsService.GameResources[ResourceNames.Hard].Sprite;
            Prewarm(4);
        }

        private void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _pool.Add(CreateOne());
            }
        }

        private ResourceAnimator CreateOne()
        {
            var result = Object.Instantiate(_prefab, _uiService.Views.Canvas);
            result.transform.localPosition = Vector3.zero;
            return result;
        }

        public void Play(Vector3 transformPosition, bool isScreen = false, bool isHard = false)
        {
            ResourceAnimator r = GetFree();
            Vector3 position;
            if (!isScreen)
            {
                position = _uiService.Views.Camera.WorldToScreenPoint(transformPosition); 
            }
            else
            {
                position = transformPosition;
            }
            
            r.SetImagesHolderPosition(position);
            SetImage(r, isHard? _hard: _soft);
            r.Play();
        }

        public void Play(Vector3 transformPosition, ResourceNames reward, float emeraldChance)
        {
            ResourceAnimator r = GetFree();
            
            Vector3 position = _uiService.Views.Camera.WorldToScreenPoint(transformPosition);
            r.SetImagesHolderPosition(position);
            
            int emeraldsCount = (int) (r.Images.Length * emeraldChance);
            for (int i = 0; i < r.Images.Length; i++)
            {
                if (i < emeraldsCount)
                {
                    r.Images[i].sprite = _hard;
                    continue;
                }
                r.Images[i].sprite = _settingsService.GameResources[reward].Sprite;
            }

            r.Play();
        }

        private void SetImage(ResourceAnimator r, Sprite sprite)
        {
            for (int i = 0; i < r.Images.Length; i++)
            {
                r.Images[i].sprite = sprite;
            }
        }
        private ResourceAnimator GetFree()
        {
            ResourceAnimator unit = null;
            foreach (var r in _pool)
            {
                if (!r.IsPlaying)
                {
                    unit = r;
                }
            }

            if (unit == null)
            {
                unit = CreateOne();
                _pool.Add(unit);
            }
            return unit;
        }
    }
}