using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CardCorrupts : CorruptionElement
{
    [SerializeField] RunData RunData;
    [SerializeField] List<CardPlus> corruptionCards;

    public override void Activate(int budget)
    {
        TurnManager.initialPositionReport.AddListener(() => DistributeCorruptionCards(budget));
    }

    void DistributeCorruptionCards(int budget)
    {
        List<HandPlus> hands = TurnManager.turnTakers
            .Select(x => x.gameObject.GetComponent<HandPlus>())
            .Where(x => x.thisUnit.Allegiance == AllegianceType.SLIME).ToList();
        while (budget > 0)
        {
            int select = Random.Range(0, hands.Count);
            GiveCorruptionCard(hands[select]);
            budget--;
        }
    }

    Coroutine sequencer;
    Queue<HandPlus> handsToAnimate;
    private void GiveCorruptionCard(HandPlus handPlus)
    {
        
        handsToAnimate ??= new();
        //vfx for corruption
        handsToAnimate.Enqueue(handPlus);
        sequencer ??= StartCoroutine(StaggerAnimations());
        
        CardPlus chosenCard = corruptionCards[Random.Range(0, corruptionCards.Count)];
        handPlus.ConjureCard(chosenCard, handPlus.transform.position, EffectInject.InjectLocation.DECK);
    }

    [SerializeField] float staggerWait;
    IEnumerator StaggerAnimations()
    {
        while (handsToAnimate.Count > 0)
        {
            handsToAnimate.Dequeue().thisUnit.unitAnimator.Animate(AnimType.HOVER);
            yield return new WaitForSeconds(staggerWait);
        }
    }
}
