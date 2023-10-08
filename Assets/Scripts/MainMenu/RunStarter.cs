using System.Collections.Generic;
using UnityEngine;

public class RunStarter : MonoBehaviour
{
    [SerializeField] RunData runData;
    [SerializeField] PlayerClass playerClass;
    [SerializeField] UnitStats playerStats;
    [SerializeField] Deck playerDeck;
    [SerializeField] GenerationParameters generationParameters;
    [SerializeField] DifficultySelector difficultySelector;
    [SerializeField] EventSpawnRates rates;

    [HideInInspector] public static readonly TerrainType[] unpathable = { TerrainType.WATER, TerrainType.DEEPWATER, TerrainType.MOUNTAIN };
    public void NewGame()
    {
        EventManager.loadSceneWithScreen.Invoke(1);
        Random.InitState(Random.Range(int.MinValue, int.MaxValue));
        runData.difficultyTier = difficultySelector.currentDifficulty;
        Settings.Balance = difficultySelector.GetDifficultyFromTier(runData.difficultyTier);


        //reset the players deck and base stats to the class's
        playerClass.ResetAndInitialize(playerStats);
        playerClass.ResetAndInitialize(playerDeck);
        runData.playerDeck = playerDeck;
        runData.playerStats = playerStats;
        runData.currentHealth = playerClass.basemaxHealth;

        //build a world map for the run and set the player's position on it
        int mapSize = BuildWorldMap();

        

        //initalize gameplay lists
        runData.itemPool.awardableItems = new();
        runData.itemPool.awardableItems.AddRange(runData.itemPool.awardableItemPool);
        runData.itemInventory = new();
        runData.essenceInventory = new();
        runData.KeyStock = 0;
        runData.RemoveStock = 0;
        runData.worldSteps = 0;
        runData.score = 0;
        runData.eventMap = new();
        runData.bossSequence = new();
        runData.exploredChunks = new bool[mapSize / DynamicEventPlacer.chunkSize, mapSize / DynamicEventPlacer.chunkSize];
        runData.endless = false;

        BuildStartingPosition(mapSize);

        //send the player to the world map
        EventManager.loadSceneWithScreen.Invoke(-1);
    }

    private int BuildWorldMap()
    {
        ProceduralMapGenerator proceduralGenerator = new();
        runData.worldMap = proceduralGenerator.Generate(generationParameters);
        int mapSize = runData.worldMap.GetLength(0);
        return mapSize;
    }

    private void BuildStartingPosition(int mapSize)
    {
        Vector2Int startPosition = runData.worldMap.SpiralSearch(unpathable, new Vector2Int(mapSize / 2, mapSize / 2), false);

        runData.playerWorldX = startPosition.x;
        runData.playerWorldY = startPosition.y;

        DynamicEventPlacer placer = new(runData, rates);
        placer.CheckToPopulateChunks(startPosition, PlayerPrefs.GetInt(nameof(TutorialFor.MAIN)) == -1);
        List<Vector2Int> adjacents = startPosition.GetAdjacentCoordinates();
        adjacents.Add(startPosition);
        foreach (var adjacent in adjacents)
        {
            runData.eventMap.Remove(adjacent);
        }
    }
}
