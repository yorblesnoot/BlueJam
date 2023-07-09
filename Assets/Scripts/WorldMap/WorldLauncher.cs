using UnityEngine;

public class WorldLauncher : MapLauncher
{
    public static string[,] worldMap;
    [SerializeField] RunData runData;
    [SerializeField] WorldMapRenderer mapRenderer;
    [SerializeField] WorldEventRenderer eventRenderer;

    public int amountOfWorldEnemies;

    private void Awake() 
    {
        //set variables
        worldMap = runData.worldMap;
        if (worldMap == null)
        {
            int defaultWorldSize = 10;
            worldMap = new string[defaultWorldSize, defaultWorldSize];
            for(int x = 0; x < defaultWorldSize; x++)
            {
                for(int y = 0; y < defaultWorldSize; y++)
                {
                    worldMap[x, y] = "w";
                }
            }
        }
        else
        {
            new SaveContainer(runData).SaveGame();
        }
        mapRenderer.RenderInitialWorldWindow(worldMap, 4);

        EventManager.updateWorldCounters.Invoke();
        EventManager.updateWorldHealth.Invoke();

    }
}
