using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "ScriptableObjects/UnitStats")]
public class UnitStats : ScriptableObject
{
    public string unitName;

    public int maxHealth;
    public int handSize;

    public bool playerAllied;

    public float startBeats;
    public bool myTurn;
    
    //to be used later
    public int damageScaling;
    public int barrierScaling;
    public int healScaling;

    public float turnSpeed;

}
