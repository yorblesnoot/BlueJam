using UnityEngine;

public interface ICardDisplay 
{
    public BattleUnit owner { get; set; }
    public CardPlus thisCard { get; set; }
    public void PopulateCard(CardPlus card, bool limited = false);

    public Transform transform { get; }
    public GameObject gameObject { get; }
    public bool forceConsume { get; set; }
}
