using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BtlCardDisplay : PlayerCardDisplay, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] GameObject outline;
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
            Tutorial.CompleteStage(TutorialFor.BATTLEACTIONS, 3);
            Tutorial.EnterStage(TutorialFor.BATTLEACTIONS, 4, "The <color=blue>blue</color> tiles on the map are where you can play the card, but only if the tile contains the type of target described on the card.");
            
            //tell every active card to become inactive
            EventManager.clearActivation?.Invoke();
            PlayerUnit.playerState = PlayerBattleState.TARGETING_CARD;
            //find list of legal cell targets
            List<GameObject> legalCells = CellTargeting.ConvertMapRuleToTiles(thisCard.targetRules, owner.transform.position);

            //LOS logic for move cards
            if (thisCard.needsPath == true) legalCells = legalCells.EliminateUnpathable(owner.gameObject);

            //highlight on each legal cell
            for (int i = 0; i < legalCells.Count; i++)
            {
                BattleTileController cellController = legalCells[i].GetComponent<BattleTileController>();
                cellController.availableForPlay = true;
                cellController.HighlightCell(owner, thisCard);
            }
            EventManager.showAOE.Invoke(thisCard);
            //add self aoe effect
            TurnManager.ShowPossibleTurnTakers(thisCard.cost);
            EventManager.targetConfirmed.AddListener(ProxyPlayCard);
            activated = true;
            EventManager.endEmphasis.Invoke();
            outline.SetActive(true);
        }
    }

    public void ClearActive()
    {
        activated = false;
        emphasize.readyEmphasis = true;
        outline.SetActive(false);
        EventManager.showAOE.Invoke(null);
        EventManager.clearAOE?.Invoke();
        EventManager.targetConfirmed.RemoveListener(ProxyPlayCard);
        if (PlayerUnit.playerState == PlayerBattleState.TARGETING_CARD) PlayerUnit.playerState = PlayerBattleState.IDLE;
    }

    public void ProxyPlayCard(BattleTileController tile)
    {
        if (activated == true && CellTargeting.ValidPlay(tile, owner, thisCard))
        {
            Tutorial.CompleteStage(TutorialFor.BATTLEBARRIER, 1, true);
            Tutorial.CompleteStage(TutorialFor.BATTLEDODAMAGE, 1, true);
            Tutorial.CompleteStage(TutorialFor.BATTLEACTIONS, 6, true);
            Tutorial.CompleteStage(TutorialFor.BATTLEACTIONS, 5);
            Tutorial.CompleteStage(TutorialFor.BATTLEACTIONS, 4);

            PlayerUnit.playerState = PlayerBattleState.PERFORMING_ACTION;
            EventManager.clearActivation?.Invoke();
            StartCoroutine(owner.GetComponent<HandPlus>().DiscardCard(this, true));
            owner.StartCoroutine(thisCard.PlaySequence(owner, tile));
        }
        EventManager.clearActivation?.Invoke();
    }

    private void Update()
    {
        if(activated && Input.GetMouseButtonDown(1))
        {
            EventManager.clearActivation?.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PlayerUnit.playerState == PlayerBattleState.IDLE && thisCard != null)
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
