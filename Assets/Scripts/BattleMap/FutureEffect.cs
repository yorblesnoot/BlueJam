using System.Collections;
using System.Collections.Generic;
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

    public float BeatCount => countdown;
    public AllegianceType Allegiance => actor.Allegiance == AllegianceType.PLAYER ? AllegianceType.ALLY : actor.Allegiance; 
    public bool isSummoned => true;
    public float TurnThreshold => delay;

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
    }
    public void ReceiveBeatsFromPlayer(int beats, PlayerUnit player)
    {
        countdown += beats;
        RecolorPips(delay, countdown);
    }

    public void TakeTurn()
    {
        actor.StartCoroutine(ActivateEffects());
    }

    IEnumerator ActivateEffects()
    {
        foreach (CardEffectPlus effect in effects)
        {
            yield return actor.StartCoroutine(effect.Execute(actor, targetCell));
        }
    }

    public void ShowBeatPreview(int beats)
    {
        RecolorPips(delay, countdown, beats);
    }

    public void RecolorPips(int delay, int countdown, int ghost = 0)
    {
        for (int i = 0; i < delay; i++)
        {
            if (i < countdown) pips[i].color = Color.blue;
            else if (i < ghost) pips[i].color = Color.red;
            else pips[i].color = Color.white;
        }
    }
}
