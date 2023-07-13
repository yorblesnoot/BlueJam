using UnityEngine;

public interface ICardDisplay 
{
    public BattleUnit owner { get; set; }
    public CardPlus thisCard { get; set; }
    public void PopulateCard(CardPlus card);

    public Transform transform { get; }
    public GameObject gameObject { get; }
}
