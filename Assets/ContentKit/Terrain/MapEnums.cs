using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainType
{
    EMPTY,
    WATER,
    DEEPWATER,
    MOUNTAIN,
    FOREST,
    TUNDRA,
    DESERT,
    RUINS,
    BROKEN
}

public enum EventType
{
    NONE,
    ENEMY,
    HEAL,
    REMOVE,
    ITEM,
    BOSS,
    BOAT,
    BALLOON,
    ORBCLONE,
    ORBCONSUME,
    ORBGAMBLE
}
