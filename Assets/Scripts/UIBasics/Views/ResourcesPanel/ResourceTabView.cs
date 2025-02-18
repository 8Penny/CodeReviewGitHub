using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIBasics.Views.ResourcesPanel
{
    public class ResourceTabView : MonoBehaviour
    {
        [SerializeField]
        private ResourceType _resourceType;
        [SerializeField]
        private GameObject _selected;
        [SerializeField]
        private Color _color;
        [SerializeField]
        private Image _backgraoundImage;

        public Color CurrentColor => _color;

        public ResourceType ResourceType => _resourceType;

        private void Awake()
        {
            _backgraoundImage.color = _color;
        }

        public void UpdatedSelected(bool isSelected)
        {
            _selected.gameObject.SetActive(isSelected);
        }
    }
}