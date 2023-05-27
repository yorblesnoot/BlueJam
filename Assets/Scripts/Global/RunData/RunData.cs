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

    public int worldX;
    public int worldY;
    public List<string> items;
    public List<Deck> essenceInventory;

    public int runDifficulty;
    public int baseEnemies;

    public int currentHealth;

    //battle map data; should be overwritten when an encounter starts
    public GenerationParameters battleParameters;

        //enemy spawn info
    public List<GameObject> spawnUnits;
    public List<int> spawnWeights;

    public int enemyBudget;

}
