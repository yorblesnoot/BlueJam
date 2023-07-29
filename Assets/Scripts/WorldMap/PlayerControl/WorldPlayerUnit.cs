using System.Linq;

public class WorldPlayerUnit : Unit
{
    private void Awake()
    {
        LoadStats();
        ApplyItemModifiers();
    }

    private void ApplyItemModifiers()
    {
        foreach(var item in runData.itemInventory.OfType<WorldItem>())
        {
            item.ActivateItem(this);
        }
    }
}
