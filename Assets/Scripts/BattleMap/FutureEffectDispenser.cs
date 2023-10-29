using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FutureEffectDispenser : MonoBehaviour
{
    [SerializeField] GameObject _tracker;
    static GameObject tracker;
    private void Awake()
    {
        tracker = _tracker;
    }

    public static void SpawnDelayedEffect(BattleUnit actor, BattleTileController targetCell, CardEffectPlus[] effects, int delay)
    {
        FutureEffect future = Instantiate(tracker, targetCell.unitPosition, Quaternion.identity).GetComponent<FutureEffect>();
        future.Initialize(actor, targetCell, effects, delay);
    }
}
