using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunData", menuName = "ScriptableObjects/Singletons/RunData")]
public class RunData : ScriptableObject
{
    //world map data
    public UnitStats playerStats;
    public Deck playerDeck;
    [HideInInspector] public string[,] worldMap;
    [HideInInspector] public List<Vector2Int> worldEnemies;
    [HideInInspector] public string[,] eventMap;

    public int playerWorldX;
    public int playerWorldY;

    public int bossWorldX;
    public int bossWorldY;

    public List<BattleItem> itemInventory;
    public List<Deck> essenceInventory;

    public ItemPool itemPool;

    public int KeyStock;
    public int RemoveStock;
    
    public int worldSteps;
    public int runDifficulty { get { return worldSteps / 5; } }

    public int currentHealth;
}
