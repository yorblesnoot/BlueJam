using UnityEngine;

public class WorldLauncher : MonoBehaviour
{
    #nullable enable
    public static string[,]? worldMap;
    #nullable disable
    // Start is called before the first frame update
    [SerializeField] RunData runData;
    [SerializeField] MapRenderer mapRenderer;
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
        eventRenderer.RenderEnemies(runData.worldEnemies);
        eventRenderer.RenderEvents(runData.eventMap);


        EventManager.worldMove?.Invoke();
    }
}
