using System;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class TutorialTaskSettings
    {
        public TutorialTaskType TaskType; 
        public string Name;
        public Sprite BG;
        public string Description;
        [Space]
        public int Count;
        [Space][Space]
        public string InfoName;
        public string InfoDescription;
    }
}