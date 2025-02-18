using System;

[Serializable]
public class CraftEntity
{
    private int _craftTypeId;
    private int _id;
    private ResourcesHolder _frozenResourcesHolder;
    
    private int _recipeId = -1;
    private float _craftTime;
    
    public int CraftTypeId => _craftTypeId;
    public int Id => _id;
    public ResourcesHolder Frozen => _frozenResourcesHolder;
    public int RecipeId => _recipeId;
    public float CraftTime => _craftTime;


    public CraftEntity(int craftTypeId, int currentId)
    {
        _craftTypeId = craftTypeId;
        _frozenResourcesHolder = new ResourcesHolder();
        _id = currentId;
    }
    
    public void SetRecipeId(int id)
    {
        _recipeId = id;
    }
    public void SetCraftTime(float time)
    {
        _craftTime = time;
    }
}