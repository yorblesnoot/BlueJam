using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


public class WorldMovementController : MonoBehaviour
{
    [SerializeField] CellHighlight mySelector;
    public WorldEventHandler myEventHandler;

    [SerializeField] RunData runData;

    List<Vector2Int> myPath;

    public static readonly float heightAdjust = .3f;
    public Vector3 unitPosition;

    public static UnityEvent pathingComplete = new();

    public static WorldMovementController activeTile;
    
    void OnEnable()
    {
        myPath = new();
        Vector3 myPosition = transform.position;
        unitPosition = new Vector3(myPosition.x, myPosition.y + heightAdjust, myPosition.z);
        EventManager.clearWorldDestination.RemoveListener(ClearHighlight);
        EventManager.clearWorldDestination.AddListener(ClearHighlight);

        EventManager.requestMapReferences.AddListener(launcher => { launcher.SubmitMapReference(gameObject); });
        pathingComplete.AddListener(RerenderPath);
    }

    private void OnDisable()
    {
        pathingComplete.RemoveListener(RerenderPath);
    }

    public void OnMouseDown()
    {
        if (WorldPlayerControl.playerState == WorldPlayerState.IDLE && myPath != null && !EventSystem.current.IsPointerOverGameObject())
        {
            //move the player to the cell
            TutorialProgression();
            WorldPlayerControl.player.StartCoroutine(WorldPlayerControl.player.ChainPath(myPath));
        }
    }

    private static void TutorialProgression()
    {
        Tutorial.CompleteStage(TutorialFor.WORLDPICKUPS, 1, true);
        Tutorial.CompleteStage(TutorialFor.WORLDMOVE, 1, true);
        Tutorial.Initiate(TutorialFor.WORLDPICKUPS, TutorialFor.WORLDMOVE);
        Tutorial.EnterStage(TutorialFor.WORLDPICKUPS, 1, "To grow, I'll need to explore, gather <color=blue>pickups</color> around the world map, and defeat <color=red>enemies</color> in battle. But I'll also need to proceed cautiously; with every step, my enemies gain power...");

        Tutorial.CompleteStage(TutorialFor.WORLDCRAFTING, 4, true);
        Tutorial.CompleteStage(TutorialFor.WORLDBOSS, 2, true);
        Tutorial.CompleteStage(TutorialFor.WORLDIMPASSABLE, 1, true);
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        activeTile = this;
        if (WorldPlayerControl.playerState == WorldPlayerState.PATHING) return;
        RerenderPath();
    }

    public void RerenderPath()
    {
        if (activeTile != this) return;
        EventManager.clearWorldDestination.Invoke();
        Vector2Int myPosition = MapTools.VectorToMap(unitPosition);
        Pathfinder pather = new(runData.worldMap, runData.eventMap, WorldMapRenderer.spotlightGlobalOffset, myPosition);
        Vector2Int playerPosition = new Vector2Int(runData.playerWorldX, runData.playerWorldY) - WorldMapRenderer.spotlightGlobalOffset;
        myPath = pather.FindVectorPath(playerPosition, myPosition);
        if(myPath == null) return;
        foreach (var cell in myPath)
            MapTools.MapToTile(cell).GetComponent<WorldMovementController>().HighlightRoute();
    }
    private void OnMouseExit()
    {
        
        myPath = null;
        activeTile = null;
        if (WorldPlayerControl.playerState == WorldPlayerState.PATHING) return;
        EventManager.clearWorldDestination.Invoke();
    }
    public void HighlightRoute()
    {
        mySelector.ChangeHighlightMode(HighlightMode.AOE);
    }
    public void ClearHighlight() { mySelector.ChangeHighlightMode(HighlightMode.OFF); }
}
