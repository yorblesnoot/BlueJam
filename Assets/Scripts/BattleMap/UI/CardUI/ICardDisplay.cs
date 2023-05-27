using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardDisplay 
{
    public GameObject owner { get; set; }
    public CardPlus thisCard { get; set; }
    public void PopulateCard(CardPlus card);
}
