using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCorrupts : CorruptionElement
{
    [SerializeField] RunData RunData;
    [SerializeField] List<CardPlus> corruptionCards;

    public override void Activate(int budget)
    {
        List<HandPlus> hands = TurnManager.turnTakers.Select(x => x.gameObject.GetComponent<HandPlus>()).ToList();
        while (budget > 0)
        {
            int select = Random.Range(0, hands.Count);
            GiveCorruptionCard(hands[select]);
            budget--;
        }
    }

    private void GiveCorruptionCard(HandPlus handPlus)
    {
        //vfx for corruption
        CardPlus chosenCard = corruptionCards[Random.Range(0, corruptionCards.Count)];
        handPlus.ConjureCard(chosenCard, handPlus.transform.position, EffectInject.InjectLocation.DECK);
    }
}
