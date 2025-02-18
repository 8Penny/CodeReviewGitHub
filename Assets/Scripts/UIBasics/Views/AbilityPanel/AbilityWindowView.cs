using System.Collections.Generic;
using System.Linq;
using Services;
using Services.Talents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views.AbilityPanel
{
    public class AbilityWindowView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _main;
        [SerializeField]
        private GameObject _window;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TextMeshProUGUI _header;
        [SerializeField]
        private TextMeshProUGUI _description;
        [SerializeField]
        private GameObject _reachedPanel;
        [SerializeField]
        private GameObject _researchPanel;
        [SerializeField]
        private GameObject _researchButton;

        private TalentsService _talentsService;
        private List<ResourceDemandView> _demands;

        private AbilityView _currentTalent;

        [Inject]
        public void Init(TalentsService talents)
        {
            _talentsService = talents;
        }

        public void Awake()
        {
            _demands = _main.transform.GetComponentsInChildren<ResourceDemandView>().ToList();
            _main.gameObject.SetActive(false);
        }

        public void SetAbility(AbilityView ability)
        {
            _talentsService.SetCurrentTalent(ability.Id);
            _currentTalent = ability;
            UpdateParameters();
        }

        public void Show()
        {
            _main.SetActive(true);
            _window.SetActive(true);
        }

        private void UpdateParameters()
        {
            _header.text = _currentTalent.CurrentSettings.Name;
            _description.text = TalentsHelper.GetDescription(_currentTalent.CurrentSettings);
            _image.sprite = _currentTalent.CurrentSettings.Sprite;

            bool isResearched = _talentsService.HasTalent(_currentTalent.Id);
            UpdatePanelsVisibility(isResearched);
            if (!isResearched)
            {
                UpdateDemandRows();
                _researchButton.SetActive(_currentTalent.CanBuy);
            }
        }

        private void UpdateDemandRows()
        {
            for (int i = 0; i < _currentTalent.CurrentSettings.Price.Count; i++)
            {
                var demand = _currentTalent.CurrentSettings.Price[i];
                _demands[i].SetResource(demand.ResourceId, demand.Value);
                _demands[i].gameObject.SetActive(true);
            }

            for (int i = _currentTalent.CurrentSettings.Price.Count; i < _demands.Count; i++)
            {
                _demands[i].gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (_currentTalent == null)
            {
                return;
            }
            if (!_talentsService.HasTalent(_currentTalent.Id))
            {
                _researchButton.SetActive(_currentTalent.CanBuy);
            }
        }

        private void UpdatePanelsVisibility(bool isResearched)
        {
            _reachedPanel.SetActive(isResearched);
            _researchPanel.SetActive(!isResearched);
        }
        
        public void OnResearchButtonClicked()
        {
            _talentsService.TryBuy(_currentTalent.Id);
            UpdateParameters();
        }

        public void Close()
        {
            _main.SetActive(false);
            _window.SetActive(false);
        }
    }
}