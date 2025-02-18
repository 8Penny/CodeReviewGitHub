using System;
using System.Collections.Generic;
using Static;

[Serializable]
public class ResourcesHolder
{
    public Dictionary<ResourceNames, float> Values;
    [NonSerialized]
    public Action<ResourceNames> OnItemChanged;

    public ResourcesHolder()
    {
        Values = new Dictionary<ResourceNames, float>();
    }
    
    public void AddResource(ResourceNames id, float value)
    {
        if (!Values.TryGetValue(id, out float result))
        {
            result = 0;
        }
        Values[id] = result + value;

        if (Values[id] < 0)
        {
            Values[id] = 0;
        }
        OnItemChanged?.Invoke(id);
    }
    
    public float Get(ResourceNames id)
    {
        Values.TryGetValue(id, out float result);
        return result;
    }
}

