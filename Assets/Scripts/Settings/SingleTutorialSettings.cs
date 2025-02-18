using System;
using Services.Tutorial;

namespace Settings
{
    [Serializable]
    public class SingleTutorialSettings
    {
        public TutorialStepNames Id;
        public string Text;
        public bool HasSecondText;
        public string SecondText;
        public string RewardText;
    }
}