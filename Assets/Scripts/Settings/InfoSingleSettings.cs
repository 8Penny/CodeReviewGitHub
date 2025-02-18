using System;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class InfoSingleSettings
    {
        public TutorialTaskType Type;
        public Sprite Image;
        public Sprite AbilityImage;
        public string Header;
        public string Description;
        public string GoToText;
    }
}