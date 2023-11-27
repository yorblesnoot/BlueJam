using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunData", menuName = "ScriptableObjects/Singletons/RunData")]
public class RunData : ScriptableObject
{
    //unity engine random for gameplay randomness, system random for visual randomness
    public UnityEngine.Random.State randomState;
    //world map data
    public UnitStats playerStats;
    public Deck playerDeck;


    [HideInInspector] public TerrainType[,] worldMap;
    [HideInInspector] public Dictionary<Vector2Int, EventType> eventMap;
    [HideInInspector] public bool[,] exploredChunks;

    public int playerWorldX;
    public int playerWorldY;

    public List<Item> itemInventory;
    public List<Deck> essenceInventory;
    public ItemPool itemPool;

    public int KeyStock;
    public int RemoveStock;
    
    public int worldSteps;
    public int ThreatLevel { get { return worldSteps / Mathf.RoundToInt(Settings.Balance[BalanceParameter.StepsPerThreat]) - Mathf.RoundToInt(Settings.Balance[BalanceParameter.ThreatHandicap]); } }
    public int difficultyTier;

    public int currentHealth;

    public int score;
    public List<int> bossSequence;
    public bool endless;
}
