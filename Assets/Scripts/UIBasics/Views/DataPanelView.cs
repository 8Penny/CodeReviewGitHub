using System.Collections.Generic;
using Services;
using Services.Analytics;
using Services.Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views
{
    public class DataPanelView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _header;
        [SerializeField]
        private GameObject _closeButton;
        [SerializeField]
        private Image _headerImage;
        [SerializeField]
        private Image _gradientImage;
        [SerializeField]
        private GameObject _content;
        [SerializeField]
        private GameObject _closeBtn;
        [Space]
        [SerializeField]
        private TextMeshProUGUI _panelName;
        [SerializeField]
        private Color _castlePanelColor;

        [Space]
        [SerializeField]
        private List<PanelByType> _rawPanels;
        

        private AnalyticsService _analyticsService;
        private UIService _uiService;
        private PanelType _currentPanelType;
        private Dictionary<PanelType, GameObject> _panels = new Dictionary<PanelType, GameObject>();
        private RectTransform _layoutGroupRectTransform;
        private Dictionary<PanelType, string> _panelNames;
        private Dictionary<PanelType, Color> _panelColors = new Dictionary<PanelType, Color>();

        public PanelType CurrentPanelType => _currentPanelType;

        [Inject]
        public void Init(UIService uiService, SoundService soundService, AnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
            _uiService = uiService;
            _panelColors[PanelType.Castles] = _castlePanelColor;
            _panelNames = new Dictionary<PanelType, string>
            {
                [PanelType.Benches] = "Production",
                [PanelType.Castles] = "",
                [PanelType.Boosters] = "Boosts",
                [PanelType.Resources] = "Resources",
                [PanelType.AbilityTree] = "Abilities",
                [PanelType.Tasks] = "Tasks"
            };
        }

        public void Awake()
        {
            _layoutGroupRectTransform = transform.parent.GetComponent<RectTransform>();
            foreach (var raw in _rawPanels)
            {
                _panels.Add(raw.PlanetType, raw.Panel);
                raw.Panel.gameObject.SetActive(true);
            }
            
            UpdateVisibility(false);
        }

        public void OnCloseButtonClick()
        {
            _uiService.CloseMainPanel();
        }

        public void UpdateVisibility(bool isVisible)
        {
            if (!isVisible)
            {
                _currentPanelType = PanelType.None;
            }
            _closeButton.SetActive(isVisible);
            _header.SetActive(isVisible);
            _content.SetActive(isVisible);
            _closeBtn.SetActive(isVisible);
        }

        public void OpenPanel(PanelType type)
        {
            if (_currentPanelType == type)
            {
                return;
            }
            _analyticsService.OnPanelOpen(type);

            _currentPanelType = type;
            if (type != PanelType.Castles)
            {
               _panelName.text = _panelNames[type]; 
            }
            HidePanels();
            _panels[type].SetActive(true);
            _headerImage.color = _panelColors[type];
            _gradientImage.color = _panelColors[type];
            _gradientImage.gameObject.SetActive(type != PanelType.Castles);
            UpdateVisibility(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroupRectTransform);
        }

        public void SetHeader(string value)
        {
            _panelName.text = value;
        }

        private void HidePanels()
        {
            foreach (var panel in _rawPanels)
            {
                if (_currentPanelType == panel.PlanetType)
                {
                    continue;
                }
                panel.Panel.SetActive(false);
            }
        }

        public void RegisterColor(PanelType type, Color color)
        {
            _panelColors[type] = color;
        }
    }
}