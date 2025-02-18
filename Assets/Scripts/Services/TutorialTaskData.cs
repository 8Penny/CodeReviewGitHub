using System;
using Settings;

namespace Services
{
    [Serializable]
    public class TutorialTaskData
    {
        public TutorialTaskType TutorialTaskType;
        public int Value;
    }
}