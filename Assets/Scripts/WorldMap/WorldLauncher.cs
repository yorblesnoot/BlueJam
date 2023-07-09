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
        new DynamicEventPlacer(runData).CheckToPopulateChunks(MapTools.playerLocation + WorldMapRenderer.spotlightGlobalOffset);
        new SaveContainer(runData).SaveGame();
        mapRenderer.RenderInitialWorldWindow(worldMap, 4);

        EventManager.updateWorldCounters.Invoke();
        EventManager.updateWorldHealth.Invoke();

    }
}
