using System.Collections.Generic;
using UnityEngine;

public class BattleTileController : MonoBehaviour
{
    [HideInInspector]public bool availableMove;

    #nullable enable
    [HideInInspector] public GameObject? unitContents;
    #nullable disable

    [HideInInspector] public string tileType;
    [HideInInspector] public int[] mapLocation;

    [HideInInspector] public Vector3 unitPosition;

    [SerializeField] GameObject cellHighlight;
    [SerializeField] GameObject cellHighlightAOE;

    float heightAdjust;

    string[,] aoeRules;

    void Awake()
    {
        availableMove = false;
        mapLocation = GridTools.VectorToMap(gameObject.transform.position);
        EventManager.clearActivation.AddListener(ClearHighlight);
        EventManager.showAOE.AddListener(AreaMode);
        EventManager.clearAOE.AddListener(ClearAOE);

        heightAdjust = 1.1f;
        Vector3 myPosition = gameObject.transform.position;
        unitPosition = new Vector3(myPosition.x, myPosition.y + heightAdjust, myPosition.z);
    }

    // Update is called once per frame
    void OnMouseDown()
    {
        if(availableMove == true)
        {
            Ray camray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitPoint;
            if(Physics.Raycast(camray, out hitPoint))
            {
                if(hitPoint.collider.gameObject == this.gameObject)
                {
                    EventManager.targetConfirmed?.Invoke(this.gameObject);
                }
            }
            availableMove = false;
        }
    }

    private void OnMouseEnter()
    {
        if(aoeRules != null && availableMove == true)
        {
            EventManager.clearAOE?.Invoke();
            //find list of legal cell aoe targets
            List<GameObject> legalCells = ZoneTargeter.ConvertMapRuleToTiles(aoeRules, transform.position);
            //highlight on each legal cell
            for (int i = 0; i < legalCells.Count; i++)
            {
                BattleTileController cellController = legalCells[i].GetComponent<BattleTileController>();
                cellController.HighlightCellAOE();
            }
        }
    }

    public void AreaMode(string[,] aoe)
    {
        aoeRules = aoe;
    }

    public void HighlightCell()
    {
        cellHighlight.SetActive(true);
    }
    public void ClearHighlight()
    {
        availableMove = false;
        cellHighlight.SetActive(false);
    }

    public void HighlightCellAOE()
    {
        cellHighlightAOE.SetActive(true);
    }
    public void ClearAOE()
    {
        cellHighlightAOE.SetActive(false);
    }
}