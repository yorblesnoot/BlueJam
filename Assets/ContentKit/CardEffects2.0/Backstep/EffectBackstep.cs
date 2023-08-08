using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectBackstep", menuName = "ScriptableObjects/CardEffects/Backstep")]
public class EffectBackstep : EffectMove
{
    private void Reset()
    {
        effectSound = SoundTypeEffect.PUSH;
        effectClass = CardClass.ATTACK;
    }
    public override string GetEffectDescription(Unit player)
    {
        return $"slide {Mathf.Abs(Mathf.RoundToInt(scalingMultiplier))} cells back from target";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        int distance = Mathf.RoundToInt(scalingMultiplier);
        yield return actor.StartCoroutine(Backstep(actor, targetCell, distance, moveDuration));
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
        yield return actor.StartCoroutine(Move(actor, destinationTile));
    }
}

