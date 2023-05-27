using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIProfile", menuName = "ScriptableObjects/AIProfile")]
public class AIProfile : ScriptableObject
{
    [Range(5f, 20f)] public float interestAttack;
    [Range(5f, 20f)] public float interestBuff;

    [Range(0f, 2f)] public float interestFriendly;
    [Range(0f, 2f)] public float interestHostile;

    [Range(0f, 10f)] public float proximityFriendly;
    [Range(0f, 10f)] public float proximityHostile;

    [Range(0f, 10f)] public float summonProximityHostile;
    [Range(0f, 10f)] public float summonProximityFriendly;

}
    //attack/move/summon/buff preference weight

    //move: desired distance from enemies and allies
    
    //summon: desired distance from enemies and allies

    //card priority as a stat?

    //attack: (1 or 0)* attack weight

    //move: avg(abs(desired-target)_ally + abs(desired-target)_enemy)*move weight

    //summon: *same as move

    //buff: (1 or 0) 

    //1/(abs(distance - desired distance))

