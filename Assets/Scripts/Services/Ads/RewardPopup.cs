using System;
using Static;
using TMPro;
using UnityEngine;
using Zenject;

namespace Services.Ads
{
    public class RewardPopup : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text;

        private PlayerResourcesService _playerResourcesService;

        private ResourceNames _resource;
        private int _count;

        [Inject]
        public void Init(PlayerResourcesService playerResourcesService)
        {
            _playerResourcesService = playerResourcesService;
        }
        public void SetReward(ResourceNames rName, int count)
        {
            _resource = rName;
            _count = count;
            if (rName != ResourceNames.Hard && rName != ResourceNames.Soft)
            {
                return;
            }
            _text.text = UiUtils.GetCountableValue(count, rName == ResourceNames.Hard? 1 : 0);
        }

        public void Close()
        {
            _playerResourcesService.AddResource(_resource, _count);
            gameObject.SetActive(false);
        }
    }
}