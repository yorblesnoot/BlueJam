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
    public void InitializeRun()
    {
        //starting position on world map; ~~~~~~~~~~add randomization and legality check
        int startX = 1;
        int startY = 1;

        //reset the players deck and base stats to the class's
        playerClass.ResetAndInitialize(playerStats);
        playerClass.ResetAndInitialize(playerDeck);
        runData.playerDeck = playerDeck;
        runData.playerStats = playerStats;
        runData.currentHealth = playerClass.basemaxHealth;

        //build a world map for the run and set the player's position on it
        ProceduralMapGenerator proceduralGenerator = new();
        runData.worldMap = proceduralGenerator.Generate(generationParameters);
        

        ProceduralEventPlacer eventPlacer = new(runData);
        runData.worldEnemies = eventPlacer.PlaceEnemies();
        runData.eventMap = eventPlacer.PlaceWorldEvents();

        runData.worldX = startX;
        runData.worldY = startY;

        //set difficulty parameters
        runData.baseEnemies = playerClass.baseEnemies;
        runData.runDifficulty = 0;

        //initalize gameplay lists
        runData.itemInventory = new();
        runData.essenceInventory = new();
        runData.keyStock = 0;

        //send the player to the world map
        SceneManager.LoadScene(1);
    }
}
