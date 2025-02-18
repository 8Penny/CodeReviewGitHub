using System;
using Static;
using UIBasics.Views.ResourcesPanel;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class Resource
    {
        public ResourceNames ResourceId;
        public Sprite Sprite;
        public int Weight;
        public ResourceType ResourceType;
        public int Price = 5;
    }
}