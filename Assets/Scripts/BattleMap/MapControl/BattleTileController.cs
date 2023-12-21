using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleTileController : MonoBehaviour
{
    [HideInInspector]public bool availableForPlay;
    [HideInInspector] public Vector3 unitPosition;

    public bool IsRift { get { return isRift; } }
    bool isRift;

    [SerializeField] GameObject mapGridElement;
    [SerializeField] CellHighlight cellHighlighter;
    HighlightMode baseHighlight = HighlightMode.OFF;

    public bool activateUnitStencil;

    [SerializeField] Vector3 positionAdjust;
    [SerializeField] Vector3 tentacleModifier;

    CardPlus loadedCard;
    List<GameObject> myPath;

    void Awake()
    {
        isRift = false;
        availableForPlay = false;
        EventManager.clearActivation.AddListener(ClearHighlight);
        EventManager.showAOE.AddListener(card => { loadedCard = card; });
        EventManager.clearAOE.AddListener(() => { cellHighlighter.ChangeHighlightMode(baseHighlight); });
        EventManager.requestMapReferences.AddListener(launcher => { launcher.SubmitMapReference(gameObject); });

        unitPosition = transform.position + positionAdjust;
    }

    void OnMouseDown()
    {
        InfoTagControl.hideTags?.Invoke();
        if (availableForPlay == true)
        {
            EventManager.targetConfirmed?.Invoke(this);
            availableForPlay = false;
        }
        else if (this.OccupyingUnit() != null)
        {
            this.OccupyingUnit().ShowInfoTag();
        }
        else if (loadedCard == null && myPath != null && myPath.Count > 0 && myPath.Count <= 3
            && PlayerUnit.playerState == PlayerBattleState.IDLE && !EventSystem.current.IsPointerOverGameObject())
        {
            Tutorial.CompleteStage(TutorialFor.BATTLEACTIONS, 1);
            StartCoroutine(TurnManager.playerUnit.ChainPath(myPath));
        }
        else
        {
            EventManager.clearActivation?.Invoke();
        }
    }

    private void OnMouseEnter()
    {
        //Debug.Log($"rift: {isRift} player state {PlayerUnit.playerState}");
        if(!isRift && PlayerUnit.playerState == PlayerBattleState.IDLE && !EventSystem.current.IsPointerOverGameObject())
        {
            Pathfinder pather = new();
            myPath = pather.FindObjectPath(TurnManager.playerUnit.MapPosition(), this.ToMap());
            if (myPath != null && myPath.Count <= 3)
            {
                foreach (GameObject cell in myPath)
                {
                    cell.GetComponent<BattleTileController>().HighlightCellAOE();
                }
                TurnManager.ShowPossibleTurnTakers(myPath.Count * PlayerUnit.costPerGenericMove);
            }
        }
        else if(availableForPlay == true)
        {
            //find list of legal cell aoe targets
            List<GameObject> legalCells = CellTargeting.ConvertMapRuleToTiles(loadedCard.aoePoint, this.ToMap());
            if(loadedCard.aoeSelf != null) legalCells.AddRange(CellTargeting.ConvertMapRuleToTiles(loadedCard.aoeSelf, TurnManager.playerUnit.MapPosition()));
            //highlight on each legal cell
            for (int i = 0; i < legalCells.Count; i++)
            {
                BattleTileController cellController = legalCells[i].GetComponent<BattleTileController>();
                cellController.HighlightCellAOE();
            }
        }
    }
    private void OnMouseExit() { 
        EventManager.clearAOE?.Invoke();
        if(PlayerUnit.playerState == PlayerBattleState.IDLE) EventManager.hideTurnDisplay.Invoke();
    }

    public void BecomeRift()
    {
        unitPosition = transform.position + positionAdjust + tentacleModifier;
        isRift = true;
        mapGridElement.SetActive(false);
    }

    public void HighlightCell(BattleUnit owner, CardPlus card) 
    {
        HighlightMode mode;
        if (CellTargeting.ValidPlay(this, owner, card)) mode = HighlightMode.LEGAL;
        else mode = HighlightMode.ILLEGAL;
        //add illegal check and highlight here~~~~~~~~~~~~~~~
        baseHighlight = mode;
        cellHighlighter.ChangeHighlightMode(mode); 
    }
    public void ClearHighlight()
    {
        availableForPlay = false;
        baseHighlight = HighlightMode.OFF;
        cellHighlighter.ChangeHighlightMode(HighlightMode.OFF);
        myPath = null;
    }

    public void HighlightCellAOE() { cellHighlighter.ChangeHighlightMode(HighlightMode.AOE); }
}