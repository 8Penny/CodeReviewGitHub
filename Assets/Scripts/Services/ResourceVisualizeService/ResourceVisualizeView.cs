using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Services.ResourceVisualizeService
{
    public class ResourceVisualizeView : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TextMeshProUGUI _count;

        public void Set(Sprite icon, int count)
        {
            _image.sprite = icon;
            _count.text = $"+{count}";
        }
    }
}