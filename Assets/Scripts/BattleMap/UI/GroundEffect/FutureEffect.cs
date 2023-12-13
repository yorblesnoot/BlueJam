using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FutureEffect : MonoBehaviour, ITurnTaker
{
    BattleUnit actor;
    BattleTileController targetCell;
    CardEffectPlus[] effects;
    int delay;
    int countdown = 0;

    [SerializeField] List<Image> pips;
    [SerializeField] Color32 beatColor;
    [SerializeField] Color32 ghostColor;
    [SerializeField] Color32 emptyColor;

    [SerializeField] GameObject reticleBar;

    public float BeatCount => countdown;
    public AllegianceType Allegiance => actor.Allegiance == AllegianceType.PLAYER ? AllegianceType.ALLY : actor.Allegiance; 
    public bool isSummoned => true;
    public float TurnThreshold => delay;

    private void Awake()
    {
        EventManager.hideTurnDisplay.AddListener(() => ShowBeatPreview(0));
        EventManager.clearActivation.AddListener(() => ShowBeatPreview(0));
    }
    public void Initialize(BattleUnit actor, BattleTileController targetCell, CardEffectPlus[] effects, int delay)
    {
        this.actor = actor;
        this.targetCell = targetCell;
        this.effects = effects;
        this.delay = delay;
        transform.position = targetCell.unitPosition;
        for (int i = 0; i < pips.Count; i++)
        {
            pips[i].gameObject.SetActive(i < delay);
        }
        RecolorPips(delay, countdown);
        TurnManager.ReportTurn(this);

        GenerateReticle(effects);
    }

    [SerializeField] float cellSize;
    [SerializeField] float spawnHeight;
    private void GenerateReticle(CardEffectPlus[] effects)
    {
        bool[,] combinedArea = CellTargeting.CombineAOEIndicators(effects.Select(x => x.aoe).ToList());
        List<GameObject> checkCells = CellTargeting.ConvertMapRuleToTiles(combinedArea, targetCell.ToMap());
        Dictionary<Vector2Int, GameObject> nodes = checkCells.ToDictionary( x => x.MapPosition(), x => x );
        foreach(var node in nodes.Keys )
        {
            List<Vector2Int> adjacents = node.GetAdjacentCoordinates();
            foreach(var adjacent in adjacents)
            {
                if (nodes.ContainsKey(new Vector2Int(adjacent.x, adjacent.y))) continue;
                GameObject spawned = Instantiate(reticleBar, transform);
                spawned.transform.position = new Vector3((adjacent.x - node.x) * cellSize, spawnHeight, (adjacent.y - node.y) * cellSize) + nodes[node].transform.position;
                spawned.transform.LookAt(nodes[node].transform.position);
                spawned.GetComponent<ReticleParticleSwitch>().SetAllegiance(Allegiance);
            }
        }
    }

    public void ReceiveBeatsFromPlayer(int beats, PlayerUnit player)
    {
        countdown += beats;
        StartCoroutine(TricklePips());
    }

    public void TakeTurn()
    {
        StartCoroutine(ActivateEffects());
    }

    IEnumerator ActivateEffects()
    {
        foreach (CardEffectPlus effect in effects)
        {
            yield return StartCoroutine(effect.Execute(actor, targetCell));
        }
        TurnManager.UnreportTurn(this);
        TurnManager.Main.StartCoroutine(TurnManager.FinalizeTurn());
        gameObject.SetActive(false);
    }

    public void ShowBeatPreview(int beats)
    {
        RecolorPips(delay, countdown, beats);
    }

    [SerializeField] float colorDelay;
    public void RecolorPips(int delay, int countdown, int ghost = 0)
    {
        for (int i = 0; i < delay; i++)
        {
            if (i < countdown) pips[i].color = beatColor;
            else if (i < (ghost + countdown)) pips[i].color = ghostColor;
            else pips[i].color = emptyColor;
            
        }
    }

    IEnumerator TricklePips()
    {
        for (int i = 0; i < countdown; i++)
        {
            pips[i].color = beatColor;
            yield return new WaitForSeconds(colorDelay);
        }
    }
}
