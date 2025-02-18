using System;
using System.Collections.Generic;
using Services.Updater;
using UnityEngine;
using Views;
using Zenject;

namespace Services.View
{
    public class EffectsViewService : MonoBehaviour, IUpdatable
    {
        [SerializeField]
        private GameObject _bumEffect;
        [SerializeField]
        private Transform _effectsParent;

        private UpdateService _updateService;
        private UIService _uiService;
        
        private List<EffectView> _bumEffects = new List<EffectView>();
        private List<EffectView> _usedEffects = new List<EffectView>();
        private List<EffectView> _transit = new List<EffectView>();
        

        [Inject]
        public void Init(UpdateService updateService, UIService uiService)
        {
            for (int i = 0; i < 5; i++)
            {
                var bum = Instantiate(_bumEffect, _effectsParent);
                bum.SetActive(false);
                _bumEffects.Add(bum.GetComponent<EffectView>());
            }

            _updateService = updateService;
            _uiService = uiService;
            _updateService.Register(this);
        }

        public void PlayBum(Vector3 position)
        {
            if (_bumEffects.Count != 0)
            {
                var effect = _bumEffects[^1];
                _bumEffects.RemoveAt(_bumEffects.Count - 1);
                effect.gameObject.SetActive(true);
                effect.transform.position = position;
                float distance = Vector3.Distance(_uiService.Views.Camera.transform.position, position);
                float volume = 0;
                if (distance < 110f)
                {
                    volume = 0.5f - (distance - 30) / 90f;
                }

                if (volume > 0)
                {
                    effect.Play(Mathf.Clamp(volume, 0, 0.2f));
                }
                _usedEffects.Add(effect);
            }
        }

        void IUpdatable.Update()
        {
            _transit.Clear();
            foreach (var effect in _usedEffects)
            {
                if (effect.IsPlaying())
                {
                    continue;
                }
                effect.gameObject.SetActive(false);
                _bumEffects.Add(effect);
                _transit.Add(effect);
            }

            foreach (var effect in _transit)
            {
                _usedEffects.Remove(effect);
            }
        }

        public void OnDestroy()
        {
            _updateService.Unregister(this);
        }
    }
}