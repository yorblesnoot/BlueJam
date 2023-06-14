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
    [HideInInspector]public string[,] eventMap;

    public int playerWorldX;
    public int playerWorldY;

    public int bossWorldX;
    public int bossWorldY;

    public List<BattleItem> itemInventory;
    public List<Deck> essenceInventory;

    [SerializeField]private int keyStock;
    public int KeyStock
    {
        get { return keyStock; }
        set
        {
            keyStock = value;
            EventManager.updateWorldCounters.Invoke();
        }
    }
    [SerializeField] private int removeStock;
    public int RemoveStock
    {
        get { return removeStock; }
        set
        {
            removeStock = value;
            EventManager.updateWorldCounters.Invoke();
        }
    }

    public int runDifficulty;
    public int baseEnemies;

    public int currentHealth;

    public BiomePool availableMaps;
    //enemy spawn info to convey from world to battle
    public List<GameObject> staticSpawns;
    public List<GameObject> spawnUnits;
    public List<int> spawnWeights;
    public int enemyBudget;
    public bool bossEncounter;

}
