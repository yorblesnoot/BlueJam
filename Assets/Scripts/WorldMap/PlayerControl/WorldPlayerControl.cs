using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WorldPlayerState { IDLE, PATHING, MENUS}

public class WorldPlayerControl : MonoBehaviour
{
    public RunData runData;

    public static WorldPlayerState playerState;

    [SerializeField] GameObject fogRing;
    [SerializeField] Camera mainCamera;
    [SerializeField] WorldMapRenderer worldRenderer;
    [SerializeField] UnitAnimator unitAnimator;
    public CompassMaster compassMaster;
    public static WorldPlayerControl player;

    public static List<string> badTiles;
    readonly int tileDamage = 2;
    readonly float moveTime = .5f;
    public void InitializePlayer()
    {
        player = this;
        badTiles = new() {"w", "u"};
        playerState = WorldPlayerState.IDLE;
        Vector2Int worldMapCoords = new(runData.playerWorldX, runData.playerWorldY);
        worldMapCoords -= WorldMapRenderer.spotlightGlobalOffset;
        Vector3 myPosition = MapTools.MapToVector(worldMapCoords, WorldMovementController.heightAdjust);
        gameObject.transform.position = myPosition;
        fogRing.transform.SetParent(null);
        compassMaster.transform.SetParent(null);
    }

    public IEnumerator ChainPath(List<GameObject> path)
    {
        Tutorial.CompleteStage(TutorialFor.WORLDHEAL, 1, true);
        Tutorial.CompleteStage(TutorialFor.WORLDITEM, 1, true);
        Tutorial.CompleteStage(TutorialFor.WORLDREMOVE, 1, true);
        playerState = WorldPlayerState.PATHING;
        foreach (GameObject tile in path)
        {
            runData.score -= 1;
            WorldMovementController tileController = tile.GetComponent<WorldMovementController>();
            transform.LookAt(tileController.unitPosition);
            Vector3 displacement = mainCamera.transform.position - transform.position;

            //modify player's world position and run difficulty in run data
            Vector2Int newCoords = MapTools.VectorToMap(tile.transform.position);

            Vector2Int globalCoords = newCoords + WorldMapRenderer.spotlightGlobalOffset;

            runData.playerWorldX = globalCoords.x;
            runData.playerWorldY = globalCoords.y;
            runData.worldSteps++;

            EventManager.playerAtWorldLocation.Invoke(globalCoords);

            float timeElapsed = 0;
            Vector3 startPosition = transform.position;
            while (timeElapsed < moveTime)
            {
                Vector3 step = Vector3.Lerp(startPosition, tileController.unitPosition, timeElapsed / moveTime);
                timeElapsed += Time.deltaTime;
                transform.position = step;
                fogRing.transform.position = step;
                mainCamera.transform.position = step + displacement;
                yield return null;
            }
            SoundManager.PlaySound(SoundType.SLIMESTEP);

            if (tileController.dangerTile == true)
            {
                runData.currentHealth -= tileDamage;
                runData.currentHealth = Mathf.Clamp(runData.currentHealth, 1, runData.playerStats.maxHealth);
                EventManager.updateWorldHealth.Invoke();
                VFXMachine.PlayAtLocation("DescendingVibes", transform.position);
                unitAnimator.Animate(AnimType.DAMAGED);
                yield return new WaitForSeconds(.4f);
            }
            yield return StartCoroutine(tileController.myEventHandler.TriggerWorldEvents());
        }
        playerState = WorldPlayerState.IDLE;
    }
}
