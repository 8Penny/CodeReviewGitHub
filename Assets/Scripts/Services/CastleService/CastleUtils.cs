using System;
using Settings;
using Static;
using UnityEngine;

public static class CastleUtils
{
    public static float GetMiningCount(float time, int mineSpeedLevel, float multiplier)
    {
        var value = time * multiplier *
                    (0.25f + 0.015f * Mathf.Pow(mineSpeedLevel - 1, 2) + 0.105f * (mineSpeedLevel - 1));
        return value;
    }
    public static float GetDistance(float time, int shipSpeedLevel, PlanetSettings settings, float multiplier)
    {
        var value = time * GetSpeed(shipSpeedLevel, multiplier);
        return value / (2 * (settings.Distance/ StaticValues.DistanceDivider));
    }
    public static float GetSpeed(int shipSpeedLevel, float multiplier)
    {
        var value =  0.4f // debug value
                    * multiplier * 
                    (1 + 0.015f * Mathf.Pow(shipSpeedLevel - 1, 2) + 0.195f * (shipSpeedLevel - 1));
        return value;
    }
    public static float GetCapacity(int level, float multiplier)
    {
        return (float)(multiplier *(5 + 0.15f * Math.Pow(level - 1,2) + 1.85f * (level -1)));
    }
    
}