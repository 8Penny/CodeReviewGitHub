using System;
using System.Collections.Generic;

[Serializable]
public class UpdatableEntities
{
    public Dictionary<int, Castle> Castles;
    public List<CraftEntity> Workbenches;
    public List<CraftEntity> CraftTables;

    public UpdatableEntities()
    {
        Castles = new Dictionary<int, Castle>();
        Workbenches = new List<CraftEntity>();
        CraftTables = new List<CraftEntity>();
    }
}
