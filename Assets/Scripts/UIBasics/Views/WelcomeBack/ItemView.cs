using Settings;
using Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIBasics.Views.WelcomeBack
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField]
        private Image _bg;
        [SerializeField]
        private Image _itemIcon;
        [SerializeField]
        private TextMeshProUGUI _name;
        [SerializeField]
        private TextMeshProUGUI _count;

        public void Init(Resource resource, float count, Sprite bg)
        {
            _bg.sprite = bg;
            _itemIcon.sprite = resource.Sprite;
            _name.text = StaticNames.Get(resource.ResourceId);
            _count.text = UiUtils.GetCountableValue(count);
        }
    }
}