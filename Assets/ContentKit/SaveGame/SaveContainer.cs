using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveContainer
{
    RunData RunData;
    string saveJSON;
    LoadLibrary loadLibrary;

    public SaveContainer( RunData data )
    {
        RunData = data;
    }

    public SaveContainer(RunData data, LoadLibrary load)
    {
        RunData = data;
        loadLibrary = load;
    }

    public List<string> playerDeck;
    public List<string> battleItems;
    public List<string> essenceInventory;

    public List<string> worldMap;

    public List<string> eventsOnMap;
    public List<int> eventsX;
    public List<int> eventsY;
    public List<bool> chunks;

    public int currentHealth;
    public int playerX;
    public int playerY;
    public int bossX;
    public int bossY;
    public int steps;
    public int keyStock;
    public int removeStock;

    //unitstats
    public int maxHealth;
    public int handSize;
    public int damageScaling;
    public int barrierScaling;
    public int healScaling;
    public float turnSpeed;
    
    public void SaveGame()
    {

        SaveNums();
        SaveCollectibles();
        SaveArrays();
        SaveEvents();
        saveJSON = JsonUtility.ToJson(this, false);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/runData.json", saveJSON);
        //Debug.Log(saveJSON);
    }

    void SaveNums()
    {
        currentHealth = RunData.currentHealth;
        playerX = RunData.playerWorldX;
        playerY = RunData.playerWorldY;
        steps = RunData.worldSteps;
        keyStock = RunData.KeyStock;
        removeStock = RunData.RemoveStock;

        maxHealth = RunData.playerStats.maxHealth;
        handSize = RunData.playerStats.handSize;
        damageScaling = RunData.playerStats.damageScaling;
        barrierScaling = RunData.playerStats.barrierScaling;
        healScaling = RunData.playerStats.healScaling;
        turnSpeed = RunData.playerStats.turnSpeed;
    }

    void SaveCollectibles()
    {
        playerDeck = RunData.playerDeck.deckContents.Select(x => x.Id).ToList();
        battleItems = RunData.itemInventory.Select(x => x.Id).ToList();
        essenceInventory = RunData.essenceInventory.Select(x => x.Id).ToList();
    }

    void SaveArrays()
    {
        worldMap = RunData.worldMap.Flatten();
        chunks = RunData.exploredChunks.Flatten();
    }

    void SaveEvents()
    {
        eventsX = new();
        eventsY = new();
        eventsOnMap = new();
        foreach(Vector2Int location in RunData.eventMap.Keys)
        {
            eventsX.Add(location.x);
            eventsY.Add(location.y);
            eventsOnMap.Add(RunData.eventMap[location]);
        }
    }

    public void LoadGame()
    {

        //get JSON from file or elsewhere
        saveJSON = System.IO.File.ReadAllText(Application.persistentDataPath + "/runData.json");
        JsonUtility.FromJsonOverwrite(saveJSON, this);

        LoadNums();
        LoadArrays();
        LoadCollectibles();
        LoadEvents();
        LoadDerived();

        SceneManager.LoadScene(1);
    }

    private void LoadEvents()
    {
        RunData.eventMap = new();
        for (int i = 0; i < eventsOnMap.Count; i++)
        {
            RunData.eventMap.Add(new Vector2Int(eventsX[i], eventsY[i]), eventsOnMap[i]);
        }
    }

    void LoadNums()
    {
        RunData.currentHealth = currentHealth;
        RunData.playerWorldX = playerX;
        RunData.playerWorldY = playerY;
        RunData.worldSteps = steps;
        RunData.KeyStock = keyStock;
        RunData.RemoveStock = removeStock;

        RunData.playerStats.maxHealth = maxHealth;
        RunData.playerStats.handSize = handSize;
        RunData.playerStats.damageScaling = damageScaling;
        RunData.playerStats.barrierScaling = barrierScaling;
        RunData.playerStats.healScaling = healScaling;
        RunData.playerStats.turnSpeed = turnSpeed;
    }

    void LoadArrays()
    {
        RunData.worldMap = worldMap.Unflatten();
        RunData.exploredChunks = chunks.Unflatten();
    }

    void LoadCollectibles()
    {
        loadLibrary.Initialize();
        RunData.playerDeck.deckContents = playerDeck.Select(x => loadLibrary.cards[x]).ToList();
        RunData.itemInventory = battleItems.Select(x => loadLibrary.items[x]).ToList();
        RunData.essenceInventory = essenceInventory.Select(x => loadLibrary.decks[x]).ToList();
    }

    void LoadDerived()
    {
        RunData.itemPool.awardableItems = RunData.itemPool.awardableItemPool.Where(x => !RunData.itemInventory.Contains(x)).ToList();
    }
}
