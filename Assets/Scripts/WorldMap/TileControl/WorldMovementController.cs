using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldMovementController : MonoBehaviour
{
    public Vector3 unitPosition;

    [SerializeField] CellHighlight mySelector;

    public WorldEventHandler eventHandler;

    [SerializeField] RunData runData;

    WorldPlayerControl player;

    List<GameObject> myPath;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<WorldPlayerControl>();
        float heightAdjust = .7f;
        Vector3 myPosition = gameObject.transform.position;
        unitPosition = new Vector3(myPosition.x, myPosition.y + heightAdjust, myPosition.z);
        EventManager.clearWorldDestination.AddListener(ClearHighlight);
        EventManager.requestMapReferences.AddListener(launcher => { launcher.SubmitMapReference(gameObject); });
    }


    public void OnMouseDown()
    {
        if (!player.pathing && myPath != null && !EventSystem.current.IsPointerOverGameObject())
        {
            //move the player to the cell
            StartCoroutine(player.ChainPath(myPath));

            //deactivate targeting on unpicked cells
            EventManager.clearWorldDestination?.Invoke();
        }
    }

    private void OnMouseEnter()
    {
        if (player.pathing) return;
        Pathfinder pather = new();
        myPath = pather.FindObjectPath(MapTools.playerLocation, MapTools.VectorToMap(unitPosition));
        if (myPath != null)
        {
            foreach (GameObject cell in myPath)
            {
                cell.GetComponent<WorldMovementController>().HighlightCell();
            }
        }
    }
    private void OnMouseExit()
    {
        EventManager.clearWorldDestination.Invoke();
        myPath = null;
    }

    public void ClearHighlight()
    {
        mySelector.ChangeHighlightMode(HighlightMode.OFF);
    }

    public void HighlightCell()
    {
        mySelector.ChangeHighlightMode(HighlightMode.AOE);
    }
}
