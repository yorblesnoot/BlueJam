using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CardCorrupts : CorruptionElement
{
    [SerializeField] RunData RunData;
    [SerializeField] List<CardPlus> corruptionCards;
    int budget;

    public override void Activate(int budget)
    {
        this.budget = budget;
        TurnManager.secondaryInitializationActivities.AddListener(DistributeCorruptionCards);
        Tutorial.Initiate(TutorialFor.BATTLECORRUPTION, TutorialFor.WORLDBATTLE);
        Tutorial.EnterStage(TutorialFor.BATTLECORRUPTION, 1, "Did you see that? That enemy is bursting with corrupted energy, granting it special cards.");
    }

    void DistributeCorruptionCards()
    {
        unitsToAnimate = new();
        List<HandPlus> hands = TurnManager.turnTakers
            .Select(x => x.gameObject.GetComponent<HandPlus>())
            .Where(x => x.thisUnit.Allegiance == AllegianceType.SLIME).ToList();
        while (budget > 0)
        {
            int select = Random.Range(0, hands.Count);
            GiveCorruptionCard(hands[select]);
            if (!unitsToAnimate.Contains(hands[select].thisUnit)) unitsToAnimate.Enqueue(hands[select].thisUnit);
            budget--;
        }
        StartCoroutine(StaggerAnimations());
    }

    Queue<BattleUnit> unitsToAnimate;
    private void GiveCorruptionCard(HandPlus handPlus)
    {
        
        CardPlus chosenCard = corruptionCards[Random.Range(0, corruptionCards.Count)];
        handPlus.ConjureCard(chosenCard, handPlus.transform.position, EffectInject.InjectLocation.DECK);
    }

    [SerializeField] float staggerWait;
    IEnumerator StaggerAnimations()
    {
        while (unitsToAnimate.Count > 0)
        {
            BattleUnit target = unitsToAnimate.Dequeue();
            target.unitAnimator.Animate(AnimType.HOVER);
            VFXMachine.PlayAtLocation("CorruptAlert", target.transform.position);
            yield return new WaitForSeconds(staggerWait);
        }
    }
}
