using System;
using System.Collections.Generic;
using Services;

[Serializable]
public class SimpleParameters
{
    public int BoostParameter;
    public int LastTutorialStep;
    public int WorkbenchParameter;
    public int CraftTableParameter = -1;
    public long Tick;
}
