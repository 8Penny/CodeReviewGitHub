using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "ResourcesList", menuName = "Configs/ResourcesList")]
    public class ResourcesList : ScriptableObject
    {
        public List<Resource> Resources;
    }

}