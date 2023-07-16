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
    [HideInInspector] public Dictionary<Vector2Int, string> eventMap;
    [HideInInspector] public bool[,] exploredChunks;

    public int playerWorldX;
    public int playerWorldY;

    public List<BattleItem> itemInventory;
    public List<Deck> essenceInventory;

    public ItemPool itemPool;

    public int KeyStock;
    public int RemoveStock;
    
    public int worldSteps;
    public int runDifficulty { get { return worldSteps / 10 - difficultyCushion; } }
    public readonly int difficultyCushion = 2;

    public int currentHealth;

    public int score;
}
