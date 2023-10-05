using System.Collections;
using UnityEngine;

public class WorldLauncher : MapLauncher
{
    [SerializeField] RunData runData;
    [SerializeField] WorldMapRenderer mapRenderer;
    [SerializeField] WorldPlayerControl playerControl;
    [SerializeField] EventSpawnRates rates;

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
        RunTutorials();
        Debug.Log("tutorials run");
        DynamicEventPlacer placer = new(runData, rates);
        EventManager.playerAtWorldLocation.AddListener((Vector2Int position) => placer.CheckToPopulateChunks(position));

        Debug.Log("placer initialized");

        Vector2Int localPlayer = MapTools.VectorToMap(WorldPlayerControl.player.transform.position);

        Debug.Log("got local player pos");

        GenerateBoss(placer);

        Debug.Log("boss generated");

        mapRenderer.RenderFullWindow(runData.worldMap);
        Debug.Log("window rendered");
        TriggerEventsOnOrigin(localPlayer);

        //isue is above here
        Debug.Log("passed event trigger");

        playerControl.compassMaster.DeployCompass(EventType.BOSS, Color.red);

        new SaveContainer(runData).SaveGame();

        EventManager.updateWorldCounters.Invoke();
        EventManager.updateWorldHealth.Invoke();

        SoundManager.PlayMusic(SoundType.MUSICWORLD);
    }

    private static void TriggerEventsOnOrigin(Vector2Int localPlayer)
    {
        WorldEventHandler handler = MapTools.MapToTile(localPlayer).GetComponent<WorldEventHandler>();
        if (handler.cellEvent != null)
        {
            handler.cellEvent.PreAnimate();
            handler.StartCoroutine(handler.TriggerWorldEvents());
        }
    }

    private void RunTutorials()
    {
        Tutorial.Initiate(TutorialFor.WORLDMOVE, TutorialFor.MAIN);
        Tutorial.EnterStage(TutorialFor.WORLDMOVE, 1, $"Welcome to <b>{"Slime Alchemist".GenerateRainbowText()}</b>! This is the world map. My objective is the boss, towards the red arrow. But to beat it, I'll need to become stronger. Click on a tile to move to it.");
        Tutorial.Initiate(TutorialFor.WORLDCRAFTING, TutorialFor.WORLDBATTLE);
        Tutorial.EnterStage(TutorialFor.WORLDCRAFTING, 1, "Well done! Defeating those enemies granted me their <color=blue>essences</color>, which you can use to <color=blue>add their cards to my deck</color>. Click the anvil in the top right or press C to craft essences.");
        Tutorial.EnterStage(TutorialFor.WORLDBOSS, 2, "Wow, you did it! Now you can craft a boss card for my deck... but a new, stronger foe has arisen! Looks like you've got the basics down; let's see how far you can go!");

        if (runData.essenceInventory.Count > 10)
        {
            Tutorial.Initiate(TutorialFor.WORLDCRAFTREMINDER, TutorialFor.WORLDCRAFTING);
            Tutorial.EnterStage(TutorialFor.WORLDCRAFTREMINDER, 1, "My essence inventory is starting to fill up. Don't forget to check back on the crafting screen between battles to add cards to your deck!");
        }
    }

    private void GenerateBoss(DynamicEventPlacer placer)
    {
        if (runData.bossSequence.Count == 0) GenerateBossSequence();
        if (!runData.eventMap.ContainsValue(EventType.BOSS)) placer.PlaceBoss();
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
