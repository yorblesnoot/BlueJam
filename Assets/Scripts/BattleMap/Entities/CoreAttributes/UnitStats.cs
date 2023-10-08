using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "ScriptableObjects/UnitKit/UnitStats")]
public class UnitStats : ScriptableObject
{
    public string unitName;
    public string unitDescription;

    public int maxHealth;
    public int handSize;

    public float startBeats;
    
    //to be used later
    public int damageScaling;
    public int barrierScaling;
    public int healScaling;

    public float turnSpeed;

    private void Reset()
    {
        maxHealth = 20;
        handSize = 2;
        startBeats = 1;
        damageScaling = 10; barrierScaling = 10; healScaling = 10;

        turnSpeed = .4f;
    }
}
