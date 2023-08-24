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
    private void OnEnable()
    {
        RegisterWithCell();
    }

    void RegisterWithCell()
    {
        tile = MapTools.VectorToTile(transform.position);
        tile.GetComponent<WorldEventHandler>().RegisterEvent(this);
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
        StartCoroutine(ShrinkAway());
    }

    readonly float shrinkDuration = .5f;
    IEnumerator ShrinkAway()
    {
        glow.Stop();
        float timeElapsed = 0;
        while (timeElapsed < shrinkDuration)
        {
            Vector3 step = Vector3.Lerp(Vector3.one, Vector3.zero, timeElapsed / shrinkDuration);
            timeElapsed += Time.deltaTime;
            model.transform.localScale = step;
            yield return null;
        }
        yield return new WaitForSeconds(1F);
        gameObject.SetActive(false);
    }
}
