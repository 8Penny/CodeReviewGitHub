using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIBasics.Views.ShopWindow
{
    public class SuccessPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _currencyReward;

        [SerializeField]
        private GameObject[] _textIconSpaces;
        [SerializeField]
        private Image[] _icons;
        [SerializeField]
        private TextMeshProUGUI[] _texts;

        public void SetReward(List<ResourceDemand> demands, SettingsService settingsService)
        {
            _textIconSpaces[0].transform.parent.gameObject.SetActive(demands.Count > 2);
            UiUtils.UpdateRewards(settingsService, demands, _currencyReward, _textIconSpaces, _icons, _texts);
        }
    }
}