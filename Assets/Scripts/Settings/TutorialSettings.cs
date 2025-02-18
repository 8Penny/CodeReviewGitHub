using System.Collections.Generic;
using Services.Tutorial;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "TutorialConfig", menuName = "Configs/TutorialConfig")]
    public class TutorialSettings : ScriptableObject
    {
        [SerializeField]
        private List<SingleTutorialSettings> _tutorialSteps;

        public Dictionary<TutorialStepNames, SingleTutorialSettings> TutorialSteps;
        public void Init()
        {
            TutorialSteps = new Dictionary<TutorialStepNames, SingleTutorialSettings>();

            foreach (var step in _tutorialSteps)
            {
                TutorialSteps[step.Id] = step;
            }
        }
    }
}