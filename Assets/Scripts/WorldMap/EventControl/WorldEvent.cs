using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEvent : MonoBehaviour
{
    public RunData runData;
    GameObject tile;
    [SerializeField] string tutorialText;
    [SerializeField] TutorialFor tutorialType;

    [SerializeField] GameObject model;
    [SerializeField] ParticleSystem glow;
    internal virtual void OnEnable()
    {
        RegisterWithCell();
    }

    internal void RegisterWithCell()
    {
        tile = MapTools.VectorToTile(transform.position);
        tile.GetComponent<WorldEventHandler>().RegisterEvent(this);
    }

    public virtual void PreAnimate()
    {
        StartCoroutine(ShrinkAway());
    }

    public virtual void Activate()
    {
        //do whatever when the player enters the cell
        RemoveEvent();
        runData.score += 50;
        Tutorial.Initiate(tutorialType, TutorialFor.WORLDMOVE);
        Tutorial.EnterStage(tutorialType, 1, tutorialText);
    }

    void RemoveEvent()
    {
        Vector2Int coords = MapTools.VectorToMap(transform.position);
        coords += WorldMapRenderer.spotlightGlobalOffset;
        runData.eventMap.Remove(coords);
        WorldEventRenderer.spawnedEvents.Remove(coords);
    }

    readonly float shrinkDuration = .7f;
    IEnumerator ShrinkAway()
    {
        glow.Stop();
        float timeElapsed = 0;
        Vector3 startPosition = model.transform.position;
        Vector3 startScale = model.transform.localScale;
        while (timeElapsed < shrinkDuration)
        {
            model.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, timeElapsed / shrinkDuration);
            model.transform.position = Vector3.Lerp(startPosition, WorldPlayerControl.player.playerModel.transform.position, timeElapsed / shrinkDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1F);
        gameObject.SetActive(false);
    }
}
