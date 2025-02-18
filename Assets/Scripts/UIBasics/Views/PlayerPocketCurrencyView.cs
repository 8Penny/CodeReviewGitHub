using Services;
using Static;
using TMPro;
using UnityEngine;
using Zenject;

namespace UIBasics.Views
{
    public class PlayerPocketCurrencyView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _soft;
        [SerializeField]
        private TextMeshProUGUI _hard;
        
        private PlayerResourcesService _playerResources;

        [Inject]
        public void Init(PlayerResourcesService playerResources)
        {
            _playerResources = playerResources;
        }

        public void OnEnable()
        {
            _playerResources.OnResourcesUpdated += ResourceUpdatedHandler;
            ResourceUpdatedHandler(ResourceType.None);
        }
        public void OnDisable()
        {
            _playerResources.OnResourcesUpdated -= ResourceUpdatedHandler;
        }

        private void ResourceUpdatedHandler(ResourceType resourceType)
        {
            if (resourceType != ResourceType.None)
            {
                return;
            }

            _soft.text = UiUtils.GetCountableValue(_playerResources.GetResource(ResourceNames.Soft));
            _hard.text = UiUtils.GetCountableValue(_playerResources.GetResource(ResourceNames.Hard));

        }
    }
}