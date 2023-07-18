using UnityEngine;

public class WorldLauncher : MapLauncher
{
    [SerializeField] RunData runData;
    [SerializeField] WorldMapRenderer mapRenderer;
    [SerializeField] WorldPlayerControl playerControl;

    [SerializeField] SpawnPool bossPool;

    private void Start() 
    {
        mapRenderer.Initialize();
        playerControl.InitializePlayer();

        DynamicEventPlacer placer = new(runData);
        placer.CheckToPopulateChunks(MapTools.VectorToMap(WorldPlayerControl.player.transform.position) + WorldMapRenderer.spotlightGlobalOffset);
        if (!runData.eventMap.ContainsValue("b"))
        {
            GenerateBossSequence();
            placer.PlaceBoss();
        }

        mapRenderer.RenderFullWindow(runData.worldMap);

        playerControl.compassMaster.DeployCompass("b", Color.red);

        new SaveContainer(runData).SaveGame();

        EventManager.updateWorldCounters.Invoke();
        EventManager.updateWorldHealth.Invoke();
    }

    void GenerateBossSequence()
    {
        for(int i = 0; i < bossPool.spawnUnits.Count; i++)
        {
            runData.bossSequence.Add(i);
        }
        runData.bossSequence.Shuffle();
    }
}
