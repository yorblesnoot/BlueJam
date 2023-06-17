using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveContainer
{
    RunData RunData;

    public SaveContainer( RunData data )
    {
        RunData = data;
    }
    //things i need to save

    //player's current stats: UnitStats

    public List<string> playerDeck;
    public List<string> battleItems;
    public List<string> essenceInventory;

    public List<int> worldEnemiesX;
    public List<int> worldEnemiesY;

    public List<string> worldMap;
    public List<string> eventMap;

    public int currentHealth;
    public int playerX;
    public int playerY;
    public int bossX;
    public int bossY;
    public int steps;
    public int keyStock;
    public int removeStock;

    string saveJSON;
    
    public void SaveGame()
    {
        SaveInts();
        SaveLists();
        SaveArrays();
        saveJSON = JsonUtility.ToJson(this, true);
        Debug.Log(saveJSON);
    }

    void SaveInts()
    {
        currentHealth = RunData.currentHealth;
        playerX = RunData.playerWorldX;
        playerY = RunData.playerWorldY;
        bossX = RunData.bossWorldX;
        bossY = RunData.bossWorldY;
        steps = RunData.worldSteps;
        keyStock = RunData.KeyStock;
        removeStock = RunData.RemoveStock;
    }

    void SaveLists()
    {
        playerDeck = RunData.playerDeck.deckContents.Select(x => x.Id).ToList();
        battleItems = RunData.itemInventory.Select(x => x.Id).ToList();
        essenceInventory = RunData.essenceInventory.Select(x => x.Id).ToList();

        worldEnemiesX = RunData.worldEnemies.Select(x => x[0]).ToList();
        worldEnemiesY = RunData.worldEnemies.Select(x => x[1]).ToList();
    }

    void SaveArrays()
    {
        worldMap = RunData.worldMap.Flatten();
        eventMap = RunData.eventMap.Flatten();
    }
}
