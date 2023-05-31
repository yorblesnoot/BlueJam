using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunData", menuName = "ScriptableObjects/RunData")]
public class RunData : ScriptableObject
{
    //world map data
    public UnitStats playerStats;
    public Deck playerDeck;
    [HideInInspector]public string[,] worldMap;
    [HideInInspector]public List<int[]> worldEnemies;
    [HideInInspector] public string[,] eventMap;

    public int worldX;
    public int worldY;
    public List<BattleItem> itemInventory;
    public List<Deck> essenceInventory;
    public int keyStock;

    public int runDifficulty;
    public int baseEnemies;

    public int currentHealth;

    //battle map data; should be overwritten when an encounter starts
    public GenerationParameters battleParameters;

    //enemy spawn info to convey from world to battle
    public List<GameObject> spawnUnits;
    public List<int> spawnWeights;

    public int enemyBudget;

}
