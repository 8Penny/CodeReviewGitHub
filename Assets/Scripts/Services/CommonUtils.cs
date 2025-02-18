using System;
using Static;
using UnityEngine;


public static class CommonUtils
{
    public static long UnixTime()
    {
        var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
        return (long)timeSpan.TotalSeconds;
    }
    
    public static int GetUpgradePrice(int level, float priceMultiplier)
    {
        if (StaticValues.IsDevelop)
        {
            return 1;
        }
        return level == 2 ? 5 : Mathf.FloorToInt(5 * Mathf.Pow(1.3f * priceMultiplier, level - 1));
    }
}