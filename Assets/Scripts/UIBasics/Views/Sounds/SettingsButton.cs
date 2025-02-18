using Services;
using UnityEngine;
using Zenject;

namespace UIBasics.Views.Sounds
{
    public class SettingsButton : MonoBehaviour
    {
        [Inject]
        public UIService UIService;

        public void OnClicked()
        {
            UIService.ShowSoundsWindow();
        }
    }
}