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
        return $"slide {Mathf.Abs(Mathf.RoundToInt(scalingMultiplier))} cell{(scalingMultiplier > 1 ? "s": "")} back from target";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        int distance = Mathf.RoundToInt(scalingMultiplier);
        yield return actor.StartCoroutine(Backstep(actor, targetCell, distance, moveDuration));
    }

    IEnumerator Backstep(BattleUnit actor, BattleTileController targetCell, int distance, float duration)
    {
        Vector2 direction;
        Vector2 startPosition = actor.MapPosition();
        direction = startPosition - targetCell.ToMap();
        direction.Normalize();
        
        Vector2 destination = startPosition;
        for (int i = 0; i < distance; i++)
        {
            //evaluate each cell in turn as a destination
            Vector2 possibleDestination = destination + direction;
            Vector2Int closestRealTile = possibleDestination.RoundToInt();
            GameObject cellObj = closestRealTile.TileAtMapPosition();
            if(cellObj != null)
            {
                BattleTileController cell = cellObj.GetComponent<BattleTileController>();
                if (cell.OccupyingUnit() == null && !cell.IsRift)
                {
                    destination += direction;
                    continue;
                }
            }
            break;
        }
        BattleTileController destinationTile = destination.RoundToInt().TileAtMapPosition().GetComponent<BattleTileController>();
        yield return actor.StartCoroutine(Move(actor, destinationTile));
    }
}

