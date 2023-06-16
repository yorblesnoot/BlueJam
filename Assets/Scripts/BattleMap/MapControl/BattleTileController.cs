using System.Collections.Generic;
using UnityEngine;

public class BattleTileController : MonoBehaviour
{
    [HideInInspector]public bool availableMove;

    #nullable enable
    [HideInInspector] public BattleUnit? unitContents;
    #nullable disable

    [HideInInspector] public Vector3 unitPosition;

    [SerializeField] GameObject cellHighlight;
    [SerializeField] GameObject cellHighlightAOE;

    public enum SpawnPermission { NONE, PLAYER, ENEMY, OBJECT}
    public SpawnPermission spawns;

    float heightAdjust = .5f;

    string[,] aoeRules;

    void Awake()
    {
        availableMove = false;
        EventManager.clearActivation.AddListener(ClearHighlight);
        EventManager.showAOE.AddListener(aoe => { aoeRules = aoe; });
        EventManager.clearAOE.AddListener(() => { cellHighlightAOE.SetActive(false); });
        EventManager.requestMapReferences.AddListener(launcher => { launcher.SubmitMapReference(gameObject); });

        Vector3 myPosition = gameObject.transform.position;
        unitPosition = new Vector3(myPosition.x, myPosition.y + heightAdjust, myPosition.z);
    }

    // Update is called once per frame
    void OnMouseDown()
    {
        if (availableMove == true)
        {
            EventManager.targetConfirmed?.Invoke(this);
            availableMove = false;
        }
        else EventManager.clearActivation?.Invoke();
    }

    private void OnMouseEnter()
    {
        if(aoeRules != null && availableMove == true)
        {
            //find list of legal cell aoe targets
            List<GameObject> legalCells = CellTargeting.ConvertMapRuleToTiles(aoeRules, transform.position);
            //highlight on each legal cell
            for (int i = 0; i < legalCells.Count; i++)
            {
                BattleTileController cellController = legalCells[i].GetComponent<BattleTileController>();
                cellController.HighlightCellAOE();
            }
        }
    }
    private void OnMouseExit() { EventManager.clearAOE?.Invoke(); }

    public void HighlightCell() { cellHighlight.SetActive(true); }
    public void ClearHighlight()
    {
        availableMove = false;
        cellHighlight.SetActive(false);
    }

    public void HighlightCellAOE() { cellHighlightAOE.SetActive(true); }
}