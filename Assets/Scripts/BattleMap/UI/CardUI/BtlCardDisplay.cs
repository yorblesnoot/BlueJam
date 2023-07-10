using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BtlCardDisplay : CardDisplay, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    bool activated = false;
    void Awake()
    {
        EventManager.clearActivation.AddListener(ClearActive);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // activate targeting
        ActivateCard();
    }
    public void ActivateCard()
    {
        if (activated == true) return;
        if (PlayerUnit.playerState == PlayerBattleState.IDLE || PlayerUnit.playerState == PlayerBattleState.TARGETING_CARD)
        {
            //tell every active card to become inactive
            EventManager.clearActivation?.Invoke();
            PlayerUnit.playerState = PlayerBattleState.TARGETING_CARD;
            //find list of legal cell targets
            List<GameObject> legalCells = CellTargeting.ConvertMapRuleToTiles(thisCard.targetRules, owner.transform.position);

            //LOS logic for move cards
            if (thisCard.pathCheckForTargets == true) legalCells = legalCells.EliminateUnpathable(owner.gameObject);

            //highlight on each legal cell
            for (int i = 0; i < legalCells.Count; i++)
            {
                BattleTileController cellController = legalCells[i].GetComponent<BattleTileController>();
                cellController.availableForPlay = true;
                cellController.HighlightCell();
            }
            EventManager.showAOE.Invoke(thisCard);
            //add self aoe effect
            TurnManager.ShowPossibleTurnTakers(thisCard.cost);
            EventManager.targetConfirmed.AddListener(ProxyPlayCard);
            activated = true;
        }
    }

    public void ClearActive()
    {
        activated = false;
        EventManager.showAOE.Invoke(null);
        EventManager.clearAOE?.Invoke();
        EventManager.targetConfirmed.RemoveListener(ProxyPlayCard);
        if (PlayerUnit.playerState == PlayerBattleState.TARGETING_CARD) PlayerUnit.playerState = PlayerBattleState.IDLE;
    }

    public void ProxyPlayCard(BattleTileController tile)
    {
        if (activated == true && CellTargeting.ValidPlay(tile, owner, thisCard))
        {
            PlayerUnit.playerState = PlayerBattleState.PERFORMING_ACTION;
            EventManager.clearActivation?.Invoke();
            StartCoroutine(thisCard.PlaySequence(owner, tile));
            owner.GetComponent<Hand>().Discard(gameObject, true);
        }
        EventManager.clearActivation?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PlayerUnit.playerState == PlayerBattleState.IDLE)
        {
            TurnManager.ShowPossibleTurnTakers(thisCard.cost);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (PlayerUnit.playerState == PlayerBattleState.IDLE)
        {
            EventManager.hideTurnDisplay?.Invoke();
        }
    }
}
