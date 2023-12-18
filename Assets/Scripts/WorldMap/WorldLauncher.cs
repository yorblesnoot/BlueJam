using System.Collections;
using UnityEngine;

public class WorldLauncher : MapLauncher
{
    [SerializeField] WorldMapRenderer mapRenderer;
    [SerializeField] WorldPlayerControl playerControl;
    [SerializeField] EventSpawnRates rates;

    [SerializeField] SpawnPool bossPool;

    [SerializeField] GameObject playerModel;
    [SerializeField] GameObject joinBeams;
    [SerializeField] float introDelay;

    [SerializeField] CorruptionManager corruptionManager;

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
        DynamicEventPlacer placer = new(runData, rates);
        EventManager.playerAtWorldLocation.AddListener((Vector2Int position) => placer.CheckToPopulateChunks(position));

        Vector2Int localPlayer = MapTools.VectorToMap(WorldPlayerControl.player.transform.position);

        GenerateBoss(placer);

        mapRenderer.RenderFullWindow(runData.worldMap);
        StartCoroutine(TriggerEventsOnOrigin(localPlayer));
        playerControl.compassMaster.DeployCompass(EventType.BOSS, Color.red);

        new SaveContainer(runData).SaveGame();

        EventManager.updateWorldCounters.Invoke();
        EventManager.updateWorldHealth.Invoke();

        SoundManager.PlayMusic(SoundType.MUSICWORLD);
        
        EventManager.playerAtWorldLocation.AddListener(CorruptScene);
        corruptionManager.CorruptScene();
    }

    void CorruptScene(Vector2Int _)
    {
        corruptionManager.CorruptScene();
    }

    private IEnumerator TriggerEventsOnOrigin(Vector2Int localPlayer)
    {
        WorldEventHandler handler = MapTools.TileAtMapPosition(localPlayer).GetComponent<WorldEventHandler>();
        if (handler.cellEvent != null)
        {
            handler.cellEvent.PreAnimate();
            yield return handler.StartCoroutine(handler.TriggerWorldEvents());
            WorldPlayerControl.playerState = WorldPlayerState.IDLE;
        }
    }

    private void RunTutorials()
    {
        Tutorial.Initiate(TutorialFor.WORLDMOVE, TutorialFor.MAIN);
        Tutorial.EnterStage(TutorialFor.WORLDMOVE, 1, $"<b>{"Welcome, Alchemist".GenerateRainbowText()}</b>! This is the world map. Your objective is the boss, towards the red arrow. Click a tile to move to it.");
        Tutorial.Initiate(TutorialFor.WORLDCRAFTING, TutorialFor.WORLDBATTLE);
        Tutorial.EnterStage(TutorialFor.WORLDCRAFTING, 1, "Well done! Defeating enemies granted me their <color=blue>essences</color>, which you can use to <color=blue>add their cards to my deck</color>. Click the anvil in the top right or press C to craft essences.");
        Tutorial.EnterStage(TutorialFor.WORLDBOSS, 2, "Defeating a boss awards its essence, which you can craft to obtain a powerful card. Defeat two more bosses to claim victory!");

        if (runData.essenceInventory.Count > 10)
        {
            Tutorial.Initiate(TutorialFor.WORLDCRAFTREMINDER, TutorialFor.WORLDCRAFTING);
            Tutorial.EnterStage(TutorialFor.WORLDCRAFTREMINDER, 1, "My essence inventory is starting to fill up. Don't forget to visit the crafting screen between battles to improve my deck!");
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
