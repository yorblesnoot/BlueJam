using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleTileController : MonoBehaviour
{
    [HideInInspector]public bool availableForPlay;

    #nullable enable
    [HideInInspector] public BattleUnit? unitContents;
    #nullable disable
    [HideInInspector] public Vector3 unitPosition;

    [SerializeField] CellHighlight cellHighlighter;
    HighlightMode baseHighlight = HighlightMode.OFF;

    public enum SpawnPermission { NONE, PLAYER, ENEMY, OBJECT}
    public SpawnPermission spawns;
    public bool activateUnitStencil;

    readonly float heightAdjust = .4f;

    CardPlus loadedCard;
    List<GameObject> myPath;

    void Awake()
    {
        availableForPlay = false;
        EventManager.clearActivation.AddListener(ClearHighlight);
        EventManager.showAOE.AddListener(card => { loadedCard = card; });
        EventManager.clearAOE.AddListener(() => { cellHighlighter.ChangeHighlightMode(baseHighlight); });
        EventManager.requestMapReferences.AddListener(launcher => { launcher.SubmitMapReference(gameObject); });

        Vector3 myPosition = gameObject.transform.position;
        unitPosition = new Vector3(myPosition.x, myPosition.y + heightAdjust, myPosition.z);
    }

    void OnMouseDown()
    {
        if (availableForPlay == true)
        {
            EventManager.targetConfirmed?.Invoke(this);
            availableForPlay = false;
        }
        else if (loadedCard == null && myPath != null && myPath.Count > 0 && myPath.Count <= 3
            && PlayerUnit.playerState == PlayerBattleState.IDLE && !EventSystem.current.IsPointerOverGameObject())
        {
            Tutorial.CompleteStage(TutorialFor.BATTLEACTIONS, 1);
            StartCoroutine(TurnManager.playerUnit.ChainPath(myPath));
        }
        else EventManager.clearActivation?.Invoke();
    }

    private void OnMouseEnter()
    {
        if(PlayerUnit.playerState == PlayerBattleState.IDLE && !EventSystem.current.IsPointerOverGameObject())
        {
            Pathfinder pather = new();
            myPath = pather.FindObjectPath(MapTools.VectorToMap(TurnManager.playerUnit.transform.position), MapTools.VectorToMap(unitPosition));
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
            List<GameObject> legalCells = CellTargeting.ConvertMapRuleToTiles(loadedCard.aoePoint, transform.position);
            if(loadedCard.aoeSelf != null) legalCells.AddRange(CellTargeting.ConvertMapRuleToTiles(loadedCard.aoeSelf, TurnManager.playerUnit.transform.position));
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

    public void HighlightCell() 
    {
        baseHighlight = HighlightMode.LEGAL;
        cellHighlighter.ChangeHighlightMode(HighlightMode.LEGAL); 
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