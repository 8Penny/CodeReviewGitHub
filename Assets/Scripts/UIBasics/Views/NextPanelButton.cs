using System;
using Services.Tutorial;
using Services.View;
using UnityEngine;
using Zenject;

namespace UIBasics.Views
{
    public class NextPanelButton : MonoBehaviour
    {
        [NonSerialized] [Inject]
        public ViewCastlesService ViewCastlesService;
        [NonSerialized] [Inject]
        public TutorialService TutorialService;

        [SerializeField]
        private bool _isRight = true;
        
        public void OnNextClicked()
        {
            if (TutorialService.IsComplete)
            {
                ViewCastlesService.NextCastle(_isRight);
            }
        }
    }
}