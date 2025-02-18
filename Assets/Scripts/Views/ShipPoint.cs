using System;
using UnityEngine;

namespace Views
{
    public class ShipPoint : MonoBehaviour
    {
        [SerializeField]
        private GameObject _debugView;

        public void Awake()
        {
            _debugView.SetActive(false);
        }
    }
}