using System.Collections;
using UnityEngine;

public class WorldLauncher : MapLauncher
{
    [SerializeField] RunData runData;
    [SerializeField] WorldMapRenderer mapRenderer;
    [SerializeField] WorldPlayerControl playerControl;

    [SerializeField] SpawnPool bossPool;

    [SerializeField] GameObject playerModel;
    [SerializeField] GameObject joinBeams;
    [SerializeField] float introDelay;

    private void Start() 
    {
        mapRenderer.Initialize();
        playerControl.InitializePlayer();
        if(runData.bossSequence.Count == 0 && runData.endless == false) StartCoroutine(PlayIntro());
        else InitializeWorld();
    }

    IEnumerator PlayIntro()
    {
        playerModel.SetActive(false);
        joinBeams.SetActive(true);
        yield return new WaitForSeconds(introDelay);
        playerModel.SetActive(true);
        InitializeWorld();
    }

    void InitializeWorld()
    {
        Tutorial.Initiate(TutorialFor.WORLDMOVE, TutorialFor.MAIN);
        Tutorial.EnterStage(TutorialFor.WORLDMOVE, 1, "Welcome to Slime Alchemist! This is the world map. My objective is the boss, towards the red arrow. But to beat it, I'll need to become stronger. Click on a tile to move to it.");
        Tutorial.Initiate(TutorialFor.WORLDCRAFTING, TutorialFor.WORLDBATTLE);
        Tutorial.EnterStage(TutorialFor.WORLDCRAFTING, 1, "Well done! Defeating those enemies granted me their essences, which you can use to add their cards to my deck. Click the anvil in the top right or press C to craft essences.");
        Tutorial.EnterStage(TutorialFor.WORLDBOSS, 2, "Wow, you did it! Now you can craft a boss card for my deck... but a new, stronger foe has arisen! Looks like you've got the basics down; let's see how far you can go!");

        if(runData.essenceInventory.Count > 10)
        {
            Tutorial.Initiate(TutorialFor.WORLDCRAFTREMINDER, TutorialFor.WORLDCRAFTING);
            Tutorial.EnterStage(TutorialFor.WORLDCRAFTREMINDER, 1, "My essence inventory is starting to fill up. Don't forget to check back on the crafting screen between battles to add cards to your deck.");
        }

        DynamicEventPlacer placer = new(runData);
        Vector2Int startPos = MapTools.VectorToMap(WorldPlayerControl.player.transform.position) + WorldMapRenderer.spotlightGlobalOffset;
        if (PlayerPrefs.GetInt(nameof(TutorialFor.MAIN)) == -1)
            placer.CheckToPopulateChunks(startPos, true);
        else placer.CheckToPopulateChunks(startPos);
        if (runData.bossSequence.Count == 0) GenerateBossSequence();
        if (!runData.eventMap.ContainsValue(EventType.BOSS)) placer.PlaceBoss();
        runData.eventMap.Remove(startPos);


        mapRenderer.RenderFullWindow(runData.worldMap);
        playerControl.compassMaster.DeployCompass(EventType.BOSS, Color.red);

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
