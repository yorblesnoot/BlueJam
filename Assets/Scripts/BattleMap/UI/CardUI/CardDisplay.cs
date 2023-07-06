using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Data;

public class CardDisplay : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ICardDisplay
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] List<GameObject> costPips;
    [SerializeField] Image cardArt;
    [SerializeField] Image targetPane;
    [SerializeField] TMP_Text targetType;
    [SerializeField] TMP_Text effectText;
    [SerializeField] TMP_Text keywordPane;

    public BattleUnit owner { get; set; }
    public CardPlus thisCard { get; set; }

    public int sceneIndex;

    [SerializeField] EmphasizeCard emphasize;
    bool activated = false;

    void Awake()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        EventManager.clearActivation.AddListener(ClearActive);
    }

    //fill the details of a blank card
    public void PopulateCard(CardPlus card)
    {
        thisCard = card;
        nameText.text = card.displayName;
        foreach(GameObject pip in costPips) pip.SetActive(false);
        for(int pips = 0; pips < card.cost; pips++)
        {
            costPips[pips].SetActive(true);
        }
        effectText.text = card.description;
        if(card.cardClass.Contains(CardClass.MOVE) || card.cardClass.Contains(CardClass.SUMMON))
        {
            targetPane.color = new Color32(144, 144, 144, 255);
            targetType.text = "Target Empty";
        }
        else if(card.cardClass.Contains(CardClass.ATTACK))
        {
            if (card.aoePoint.GetLength(0) > 1) targetType.text = "AOE Enemy";
            else targetType.text = "Target Enemy";
            targetPane.color = new Color32(241, 124, 124, 255);
        }
        else if (card.cardClass.Contains(CardClass.BUFF))
        {
            if (card.aoePoint.GetLength(0) > 1) targetType.text = "AOE Ally";
            else targetType.text = "Target Ally";
            targetPane.color = new Color32(47, 231, 122, 255);
        }
        keywordPane.text = card.keywords;
        emphasize.PrepareForEmphasis();
    }

    //become clickable
    public void OnPointerClick(PointerEventData eventData)
    {
        if (sceneIndex == 2)
        {
            // activate targeting
            ActivateCard();
        }
        else
        {
            // award the card
            EventManager.clickedCard.Invoke(thisCard, gameObject);
        }
    }

    public void ActivateCard()
    {
        if(owner.myTurn == true && activated == false)
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
                cellController.availableMove = true;
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
        if (activated == true && CellTargeting.ValidPlay(tile, owner.tag, thisCard))
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
        if(PlayerUnit.playerState == PlayerBattleState.IDLE)
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
