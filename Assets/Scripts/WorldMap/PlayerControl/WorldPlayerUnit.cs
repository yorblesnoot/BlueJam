using System.Linq;

public class WorldPlayerUnit : Unit
{
    private void Awake()
    {
        LoadStats();
        currentHealth = runData.currentHealth;
    }
}
