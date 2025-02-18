using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "InfoSettings", menuName = "Configs/InfoSettings")]
    public class InfoSettings : ScriptableObject
    {
        [SerializeField]
        private List<InfoSingleSettings> _infos;

        public Dictionary<TutorialTaskType, InfoSingleSettings> InfoDict;

        public void Init()
        {
            InfoDict = new Dictionary<TutorialTaskType, InfoSingleSettings>();

            foreach (var step in _infos)
            {
                InfoDict[step.Type] = step;
            }
        }
    }
}