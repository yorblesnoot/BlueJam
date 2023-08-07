using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectBackstep", menuName = "ScriptableObjects/CardEffects/Backstep")]
public class EffectBackstep : CardEffectPlus
{
    private void Reset()
    {
        effectSound = SoundTypeEffect.PUSH;
        effectClass = CardClass.ATTACK;
        duration = .3f;
    }
    [Range(.1f, 1f)] public float duration;
    public override string GetEffectDescription(Unit player)
    {
        return $"slide {Mathf.Abs(Mathf.RoundToInt(scalingMultiplier))} cells back from target";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        int distance = Mathf.RoundToInt(scalingMultiplier);
        yield return actor.StartCoroutine(Backstep(actor, targetCell, distance, duration));
    }

    IEnumerator Backstep(BattleUnit actor, BattleTileController targetCell, int distance, float duration)
    {
        Vector3 direction;
        direction = actor.transform.position - targetCell.transform.position;
        direction.y = 0f;
        direction.Normalize();
        Vector3 startPosition = actor.transform.position;
        Vector3 destination = startPosition;
        for (int i = 0; i < distance; i++)
        {
            //evaluate each cell in turn as a destination
            Vector3 possibleDestination = destination + direction;
            GameObject cellObj = MapTools.VectorToTile(possibleDestination);
            if(cellObj != null)
            {
                BattleTileController cell = cellObj.GetComponent<BattleTileController>();
                if (cell.unitContents == null)
                {
                    destination += direction;
                    continue;
                }
            }
            break;
        }
        BattleTileController destinationTile = MapTools.VectorToTile(destination).GetComponent<BattleTileController>();
        MapTools.ReportPositionChange(actor, destinationTile);
        yield return actor.StartCoroutine(actor.gameObject.LerpTo(destinationTile.unitPosition, duration));
    }
}

