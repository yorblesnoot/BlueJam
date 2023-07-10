using UnityEngine;

public class WorldLauncher : MapLauncher
{
    [SerializeField] RunData runData;
    [SerializeField] WorldMapRenderer mapRenderer;
    [SerializeField] WorldPlayerControl playerControl;

    private void Awake() 
    {
        mapRenderer.Initialize();
        playerControl.InitializePlayer();

        DynamicEventPlacer placer = new(runData);
        placer.CheckToPopulateChunks(MapTools.VectorToMap(WorldPlayerControl.player.transform.position) + WorldMapRenderer.spotlightGlobalOffset);
        placer.PlaceBoss();

        mapRenderer.RenderFullWindow(runData.worldMap);

        playerControl.compassMaster.DeployCompass("b", Color.red);

        new SaveContainer(runData).SaveGame();

        EventManager.updateWorldCounters.Invoke();
        EventManager.updateWorldHealth.Invoke();

    }
}
