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
        Tutorial.Initiate(TutorialFor.WORLDMOVE, TutorialFor.MAIN);
        Tutorial.EnterStage(TutorialFor.WORLDMOVE, 1, "Welcome to the world map! My objective is the boss, towards the red arrow. But to beat it, I'll need to become stronger. Click on a tile to move to it. ");

        Tutorial.Initiate(TutorialFor.WORLDCRAFTING, TutorialFor.WORLDBATTLE);
        Tutorial.EnterStage(TutorialFor.WORLDCRAFTING, 1, "Well done! Defeating those enemies granted me their essences, which you can use to add their cards to my deck. Click the anvil in the top right to craft essences.");

        
        Tutorial.EnterStage(TutorialFor.WORLDBOSS, 2, "Wow, you did it! Now you can craft a boss card for my deck... but a new, stronger foe has arisen! Looks like you've got the basics down; let's see how far you can go!");

        DynamicEventPlacer placer = new(runData);
        if(PlayerPrefs.GetInt(nameof(TutorialFor.MAIN)) == -1)
            placer.CheckToPopulateChunks(MapTools.VectorToMap(WorldPlayerControl.player.transform.position) + WorldMapRenderer.spotlightGlobalOffset);
        if(runData.bossSequence.Count == 0) GenerateBossSequence();
        if (!runData.eventMap.ContainsValue("b")) placer.PlaceBoss();

        mapRenderer.RenderFullWindow(runData.worldMap);

        playerControl.compassMaster.DeployCompass("b", Color.red);

        new SaveContainer(runData).SaveGame();

        EventManager.updateWorldCounters.Invoke();
        EventManager.updateWorldHealth.Invoke();

        SoundManager.PlayMusic(SoundType.MUSICWORLD);
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
