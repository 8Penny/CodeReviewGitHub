using System;
using Static;

namespace Services
{
    [Serializable]
    public struct ResourceDemand
    {
        public ResourceNames ResourceId;
        public int Value;

        public ResourceDemand(ResourceNames id, int value)
        {
            ResourceId = id;
            Value = value;
        }
    }
}
