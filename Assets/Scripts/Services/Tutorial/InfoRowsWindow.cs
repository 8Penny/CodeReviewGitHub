using Services.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Services.Tutorial
{
    public class InfoRowsWindow : MonoBehaviour
    {
        [SerializeField]
        private GameObject _currentContainer;
        [SerializeField]
        private GameObject _parentWindow;
        [SerializeField]
        private InfoRow _row;

        private bool _isInit;

        private TutorialTaskService _tutorialTaskService;
        private TutorialService _tutorialService;
        private PlayerDataManager _dataManager;
        private DiContainer _container;
        private SettingsService _settingsService;


        [Inject]
        public void Init(SettingsService service, DiContainer container)
        {
            _settingsService = service;
            _container = container;
        }

        private void InitRows()
        {
            Transform parent = _row.transform.parent;
            bool isFirst = true;
            foreach (var info in _settingsService.TutorialTaskSettings.Tasks)
            {
                if (isFirst)
                {
                    _row.Setup(info);
                    isFirst = false;
                    continue;
                }
                GameObject go = _container.InstantiatePrefab(_row.gameObject, parent);
                var tutorialRow = go.GetComponent<InfoRow>();
                tutorialRow.Setup(info);
            }
        }

        public void Show()
        {
            if (!_isInit)
            {
                InitRows();
                _isInit = true;
            }
            _currentContainer.SetActive(true);
            _parentWindow.SetActive(true);
        }
        
        public void Close()
        {
            _currentContainer.SetActive(false);
            _parentWindow.SetActive(false);
        }
    }
}