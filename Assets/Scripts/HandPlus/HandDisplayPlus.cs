using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDisplayPlus : MonoBehaviour
{
    public GameObject blankCard;
    public Canvas unitCanvas;
    public BattleUnit thisUnit;
    public List<ICardDisplay> handCards = new();

    internal readonly int cardSize = 1;

    internal virtual void BuildVisualDeck(int count) { }

    public virtual void VisualConjure(Vector3 location, bool intoDeck = true, CardPlus card = null) {  }

    public virtual IEnumerator VisualDraw(CardPlus card, ICardDisplay display = null) { yield break; }

    public virtual IEnumerator VisualDiscard(ICardDisplay Idiscarded) { yield break; }
}
