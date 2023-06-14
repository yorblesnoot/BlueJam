using UnityEngine;

public class WorldLauncher : MapLauncher
{
    #nullable enable
    public static string[,]? worldMap;
    #nullable disable
    // Start is called before the first frame update
    [SerializeField] RunData runData;
    [SerializeField] WorldMapRenderer mapRenderer;
    [SerializeField] EventRenderer eventRenderer;

    public int amountOfWorldEnemies;

    private void Start() 
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
        mapRenderer.RenderWorld(worldMap);
        RequestMapReferences();

        eventRenderer.RenderBoss(runData.bossWorldX, runData.bossWorldY);
        eventRenderer.RenderEnemies(runData.worldEnemies);
        eventRenderer.RenderEvents(runData.eventMap);


        EventManager.worldMove?.Invoke();
    }
}
