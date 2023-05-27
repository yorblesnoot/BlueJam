using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerClass", menuName = "ScriptableObjects/PlayerClass")]
public class PlayerClass : ScriptableObject
{
    [field: SerializeField] public int basemaxHealth { get; private set; }
    [field: SerializeField] public int basehandSize { get; private set; }

    [field: SerializeField] public int baseDamage { get; private set; }
    [field: SerializeField] public int baseBarrier { get; private set; }
    [field: SerializeField] public int baseHealing { get; private set; }

    [field: SerializeField] public List<CardPlus> basedeckContents { get; private set; }

    [field: SerializeField] public int baseEnemies { get; private set; }

    public void ResetAndInitialize(Deck deck)
    {
        deck.deckContents = new List<CardPlus>(basedeckContents);
    }

    public void ResetAndInitialize(UnitStats stats)
    {
        stats.maxHealth = basemaxHealth;
        stats.handSize = basehandSize;

        stats.damageScaling = baseDamage;
        stats.barrierScaling = baseBarrier;
        stats.healScaling = baseHealing;
    }
}
