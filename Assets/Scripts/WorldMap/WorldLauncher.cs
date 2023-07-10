using UnityEngine;

public class WorldLauncher : MapLauncher
{
    [SerializeField] RunData runData;
    [SerializeField] WorldMapRenderer mapRenderer;
    [SerializeField] WorldPlayerControl playerControl;

    readonly int windowSize = 4;
    private void Awake() 
    {
        mapRenderer.RenderInitialWorldWindow(runData.worldMap, windowSize);

        playerControl.InitializePlayer();

        DynamicEventPlacer placer = new(runData);
        placer.CheckToPopulateChunks(MapTools.VectorToMap(WorldPlayerControl.player.transform.position) + WorldMapRenderer.spotlightGlobalOffset);
        placer.PlaceBoss();

        playerControl.compassMaster.DeployCompass("b", Color.red);

        new SaveContainer(runData).SaveGame();

        EventManager.updateWorldCounters.Invoke();
        EventManager.updateWorldHealth.Invoke();

    }
}
