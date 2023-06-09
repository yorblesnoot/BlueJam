using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunStarter : MonoBehaviour
{
    public RunData runData;
    public PlayerClass playerClass;
    public UnitStats playerStats;
    public Deck playerDeck;

    public GenerationParameters generationParameters;
    public void NewGame()
    {
        //reset the players deck and base stats to the class's
        playerClass.ResetAndInitialize(playerStats);
        playerClass.ResetAndInitialize(playerDeck);
        runData.playerDeck = playerDeck;
        runData.playerStats = playerStats;
        runData.currentHealth = playerClass.basemaxHealth;

        //build a world map for the run and set the player's position on it
        ProceduralMapGenerator proceduralGenerator = new();
        runData.worldMap = proceduralGenerator.Generate(generationParameters);

        int mapSize = runData.worldMap.GetLength(0);

        //starting position on world map; ~~~~~~~~~~add randomization and legality check
        runData.playerWorldX = mapSize/2;
        runData.playerWorldY = mapSize/2;

        //initalize gameplay lists
        runData.itemPool.awardableItems = new();
        runData.itemPool.awardableItems.AddRange(runData.itemPool.awardableItemPool);
        runData.itemInventory = new();
        runData.essenceInventory = new();
        runData.KeyStock = 0;
        runData.RemoveStock = 0;
        runData.worldSteps = 0;
        runData.eventMap = new();
        runData.exploredChunks = new bool[mapSize / DynamicEventPlacer.chunkSize, mapSize / DynamicEventPlacer.chunkSize];

        //send the player to the world map
        SceneManager.LoadScene(1);
    }
}
