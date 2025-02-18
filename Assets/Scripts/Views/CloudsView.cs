using System;
using System.Collections.Generic;
using Services.Talents;
using Settings;
using UnityEngine;
using Zenject;

namespace Views
{
    public class CloudsView : MonoBehaviour
    {
        [SerializeField]
        private List<CloudsPair> _clouds;

        private TalentsService _talentsService;
        private PlayerDataManager _saves;
        
        [Inject]
        public void Init(TalentsService talentsService, PlayerDataManager saves)
        {
            _talentsService = talentsService;
            _saves = saves;
        }

        private void Awake()
        {
            foreach (var c in _clouds)
            {
                c.Clouds.SetActive(false);
            }

            UpdateCloudsState();
            _saves.OnSaveLoaded += UpdateCloudsState;
            _talentsService.OnAbilitiesUpdated += UpdateCloudsState;
        }

        private void OnDestroy()
        {
            _saves.OnSaveLoaded -= UpdateCloudsState;
            _talentsService.OnAbilitiesUpdated -= UpdateCloudsState;
        }

        private void UpdateCloudsState()
        {
            foreach (var c in _clouds)
            {
                c.Clouds.SetActive(!_talentsService.HasTalent(c.Ability));
            }
        }
    }

    [Serializable]
    public class CloudsPair
    {
        public AbilityType Ability;
        public GameObject Clouds;
    }
}