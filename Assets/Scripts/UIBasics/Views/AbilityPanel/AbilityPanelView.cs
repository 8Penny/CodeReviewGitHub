using System;
using System.Collections.Generic;
using System.Linq;
using Services;
using Services.Talents;
using Services.Tutorial;
using Settings;
using UIBasics.Views.Tutorial;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.AbilityPanel
{
    public class AbilityPanelView : MonoBehaviour
    {
        [SerializeField]
        private AbilityTutorialController _abilityTutorialController;

        
        private TalentsService _talentsService;
        private UIService _uiService;
        private PlayerResourcesService _playerResourcesService;
        private Dictionary<AbilityType, AbilityView> _abilities = new Dictionary<AbilityType, AbilityView>();
        private HashSet<AbilityType> _rawAvailable = new HashSet<AbilityType>();

        [Inject]
        public void Init(UIService uiService, TalentsService talentsService, PlayerResourcesService playerResourcesService)
        {
            _talentsService = talentsService;
            _uiService = uiService;
            _playerResourcesService = playerResourcesService;
        }

        public void Awake()
        {
            foreach (var view in GetComponentsInChildren<AbilityView>().ToList().OrderBy(v => v.Id))
            {
                try
                {

                    _abilities.Add(view.Id, view);
                }
                catch (Exception e)
                {
                    Debug.Log($"{view.gameObject.name}");
                }
            }
        }

        public void OnEnable()
        {
            _talentsService.OnAbilitiesUpdated += UpdateViews;
            _playerResourcesService.OnResourcesUpdated += UpdateViews;
            UpdateViews();
        }
        public void OnDisable()
        {
            _talentsService.OnAbilitiesUpdated -= UpdateViews;
            _playerResourcesService.OnResourcesUpdated -= UpdateViews;
        }

        private void UpdateViews(ResourceType _)
        {
            UpdateViews();
        }

        private void UpdateViews()
        {
            _rawAvailable.Clear();
            foreach (var ability in _abilities)
            {
                var view = ability.Value;
                if (_talentsService.HasTalent(view.Id))
                {
                    view.InitAbility(true, true);
                    foreach (var next in view.CurrentSettings.ToAbilities)
                    {
                        _rawAvailable.Add(next);
                    }
                    continue;
                }

                if (view.CurrentSettings.Available)
                {
                    view.InitAbility(true, false, _talentsService.CanBuy(view.Id));
                    continue;
                }
                view.InitAbility(false, false);
            }
            
            var available = _rawAvailable.Except(_talentsService.UnlockedAbilities);
            foreach (var index in available)
            {
                var view = _abilities[index];
                if (_talentsService.CanBuy(view.Id))
                {
                    view.InitAbility(true, false, true);
                }
                else
                {
                    view.InitAbility(true, false);
                }
            }
        }

        public void OnAbilityClicked(AbilityView view)
        {
            if (_abilityTutorialController.CanClick())
            {
                _abilityTutorialController.Click(view);
                return;
            }

            if (!_abilityTutorialController.CanClickViaTutorial(view.Id))
            {
                return;
            }
                
            if (!view.IsClickable)
            {
                return;
            }
            _uiService.ShowAbilityWindow(view);
        }
    }
}