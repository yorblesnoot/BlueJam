using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldMovementController : MonoBehaviour
{

    [SerializeField] CellHighlight mySelector;
    public WorldEventHandler myEventHandler;

    [SerializeField] RunData runData;

    List<GameObject> myPath;

    public static readonly float heightAdjust = .5f;
    public Vector3 unitPosition;
    Vector2Int localCoords;
    bool dangerTile;

    void Awake()
    {
        Vector3 myPosition = gameObject.transform.position;
        unitPosition = new Vector3(myPosition.x, myPosition.y + heightAdjust, myPosition.z);
        localCoords = MapTools.VectorToMap(myPosition);
        EventManager.clearWorldDestination.AddListener(ClearHighlight);
        EventManager.requestMapReferences.AddListener(launcher => { launcher.SubmitMapReference(gameObject); });
        Vector2Int globalCoords = localCoords + WorldMapRenderer.spotlightGlobalOffset;
        if (WorldPlayerControl.badTiles.Contains(runData.worldMap[globalCoords.x, globalCoords.y]))
        {
            dangerTile = true;
        }
    }

    public void OnMouseDown()
    {
        if (WorldPlayerControl.playerState == WorldPlayerState.IDLE && myPath != null && !EventSystem.current.IsPointerOverGameObject())
        {
            //move the player to the cell
            StartCoroutine(WorldPlayerControl.player.ChainPath(myPath));

            //deactivate pathfinding displays
            EventManager.clearWorldDestination?.Invoke();
        }
    }

    private void OnMouseEnter()
    {
        if (WorldPlayerControl.playerState != WorldPlayerState.IDLE || EventSystem.current.IsPointerOverGameObject()) return;

        
        Pathfinder pather;
        if (dangerTile || MapTools.VectorToTile(WorldPlayerControl.player.gameObject.transform.position).GetComponent<WorldMovementController>().dangerTile)
        {
            pather = new();
        }
        else pather = new(runData.worldMap, WorldPlayerControl.badTiles, WorldMapRenderer.spotlightGlobalOffset);

        myPath = pather.FindObjectPath(MapTools.VectorToMap(WorldPlayerControl.player.transform.position), MapTools.VectorToMap(unitPosition));
        if (myPath != null)
            foreach (GameObject cell in myPath)
                cell.GetComponent<WorldMovementController>().HighlightRoute();
    }
    private void OnMouseExit()
    {
        EventManager.clearWorldDestination.Invoke();
        myPath = null;
    }
    public void HighlightRoute()
    {
        if (dangerTile)
        {
            mySelector.ChangeHighlightMode(HighlightMode.ILLEGAL);
        }
        else
            mySelector.ChangeHighlightMode(HighlightMode.AOE);
    }
    public void ClearHighlight() { mySelector.ChangeHighlightMode(HighlightMode.OFF); }
}
