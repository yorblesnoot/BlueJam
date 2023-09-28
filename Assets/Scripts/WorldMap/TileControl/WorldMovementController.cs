using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class WorldMovementController : MonoBehaviour
{
    [SerializeField] CellHighlight mySelector;
    public WorldEventHandler myEventHandler;

    [SerializeField] RunData runData;

    List<Vector2Int> myPath;

    public static readonly float heightAdjust = .5f;
    public Vector3 unitPosition;
    
    void OnEnable()
    {
        myPath = new();
        Vector3 myPosition = gameObject.transform.position;
        unitPosition = new Vector3(myPosition.x, myPosition.y + heightAdjust, myPosition.z);
        EventManager.clearWorldDestination.AddListener(ClearHighlight);
        EventManager.requestMapReferences.AddListener(launcher => { launcher.SubmitMapReference(gameObject); });
        Vector2Int globalCoords = MapTools.VectorToMap(myPosition) + WorldMapRenderer.spotlightGlobalOffset;
    }

    public void OnMouseDown()
    {
        if (WorldPlayerControl.playerState == WorldPlayerState.IDLE && myPath != null && !EventSystem.current.IsPointerOverGameObject())
        {
            //move the player to the cell
            Tutorial.CompleteStage(TutorialFor.WORLDPICKUPS, 1, true);
            Tutorial.CompleteStage(TutorialFor.WORLDMOVE, 1, true);
            Tutorial.Initiate(TutorialFor.WORLDPICKUPS, TutorialFor.WORLDMOVE);
            Tutorial.EnterStage(TutorialFor.WORLDPICKUPS, 1, "To grow, I'll need to explore, defeat enemies, and gather bombs, hearts, and chests. But I'll also need to proceed cautiously; with every step, my enemies gain power...");

            Tutorial.CompleteStage(TutorialFor.WORLDCRAFTING, 4, true);
            Tutorial.CompleteStage(TutorialFor.WORLDBOSS, 2, true);
            WorldPlayerControl.player.StartCoroutine(WorldPlayerControl.player.ChainPath(myPath));
            
            //deactivate pathfinding displays
            EventManager.clearWorldDestination?.Invoke();
        }
    }

    private void OnMouseEnter()
    {
        if (WorldPlayerControl.playerState != WorldPlayerState.IDLE || EventSystem.current.IsPointerOverGameObject()) return;
        Pathfinder pather = new(runData.worldMap, runData.eventMap, WorldMapRenderer.spotlightGlobalOffset);
        myPath = pather.FindVectorPath(MapTools.VectorToMap(WorldPlayerControl.player.transform.position), MapTools.VectorToMap(unitPosition));
        if (myPath != null)
            foreach (var cell in myPath)
                MapTools.MapToTile(cell).GetComponent<WorldMovementController>().HighlightRoute();
    }
    private void OnMouseExit()
    {
        EventManager.clearWorldDestination.Invoke();
        myPath = null;
    }
    public void HighlightRoute()
    {
        mySelector.ChangeHighlightMode(HighlightMode.AOE);
    }
    public void ClearHighlight() { mySelector.ChangeHighlightMode(HighlightMode.OFF); }
}
