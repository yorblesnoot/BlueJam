using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleTileController : MonoBehaviour
{
    [HideInInspector]public bool availableMove;

    #nullable enable
    [HideInInspector] public BattleUnit? unitContents;
    #nullable disable

    [HideInInspector] public Vector3 unitPosition;

    [SerializeField] CellHighlight cellHighlighter;

    HighlightMode baseHighlight = HighlightMode.OFF;

    public enum SpawnPermission { NONE, PLAYER, ENEMY, OBJECT}
    public SpawnPermission spawns;
    readonly float heightAdjust = .4f;

    CardPlus loadedCard;

    List<GameObject> myPath;

    void Awake()
    {
        availableMove = false;
        EventManager.clearActivation.AddListener(ClearHighlight);
        EventManager.showAOE.AddListener(card => { loadedCard = card; });
        EventManager.clearAOE.AddListener(() => { cellHighlighter.ChangeHighlightMode(baseHighlight); });
        EventManager.requestMapReferences.AddListener(launcher => { launcher.SubmitMapReference(gameObject); });

        Vector3 myPosition = gameObject.transform.position;
        unitPosition = new Vector3(myPosition.x, myPosition.y + heightAdjust, myPosition.z);
    }

    void OnMouseDown()
    {
        if(loadedCard == null && PlayerUnit.playerState == PlayerBattleState.IDLE && !EventSystem.current.IsPointerOverGameObject())
        {
            TurnManager.EndTurn();
            StartCoroutine(TurnManager.playerUnit.ChainPath(myPath));
        }
        else if (availableMove == true)
        {
            EventManager.targetConfirmed?.Invoke(this);
            availableMove = false;
        }
        else EventManager.clearActivation?.Invoke();
    }

    private void OnMouseEnter()
    {
        if(PlayerUnit.playerState == PlayerBattleState.IDLE && !EventSystem.current.IsPointerOverGameObject())
        {
            Pathfinder pather = new();
            myPath = pather.FindObjectPath(MapTools.VectorToMap(TurnManager.playerUnit.transform.position), MapTools.VectorToMap(unitPosition));
            if (myPath != null)
            {
                myPath = myPath.Take(3).ToList();
                foreach (GameObject cell in myPath)
                {
                    cell.GetComponent<BattleTileController>().HighlightCellAOE();
                }
                TurnManager.ShowPossibleTurnTakers(myPath.Count * PlayerUnit.costPerGenericMove);
            }
        }
        else if(availableMove == true)
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
        availableMove = false;
        baseHighlight = HighlightMode.OFF;
        cellHighlighter.ChangeHighlightMode(HighlightMode.OFF);
    }

    public void HighlightCellAOE() { cellHighlighter.ChangeHighlightMode(HighlightMode.AOE); }
}