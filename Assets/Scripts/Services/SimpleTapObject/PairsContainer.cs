using UnityEngine;
using Zenject;

namespace Services.SimpleTap
{
    public class PairsContainer : MonoBehaviour
    {
        private CartPath[] _pairs;
        public CartPath[] Pairs => _pairs;

        [Inject]
        public void Init(TapSecondBonusService service)
        {
            service.Register(this);
        }
        private void Awake()
        {
            _pairs = GetComponentsInChildren<CartPath>();
        }
    }
}