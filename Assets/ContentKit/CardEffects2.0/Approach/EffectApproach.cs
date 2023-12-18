using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;


[CreateAssetMenu(fileName = "EffectApproach", menuName = "ScriptableObjects/CardEffects/Approach")]
public class EffectApproach : EffectMove
{
    enum ApproachType
    {
        front,
        back
    }
    [SerializeField] ApproachType approachType;
    private void Reset()
    {
        effectSound = SoundTypeEffect.WALK;
        effectClass = CardClass.ATTACK;
    }
    public override string GetEffectDescription(Unit player)
    {
        string word = approachType switch
        {
            ApproachType.front => "in front of",
            ApproachType.back => "behind",
            _ => null,
        };
        return $"move {word} the target unit";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        List<BattleTileController> possibleTiles = CellTargeting.ConvertMapRuleToTiles(aoe, targetCell.ToMap())
            .Select(x => x.GetComponent<BattleTileController>()).Where(x => x.OccupyingUnit() == null && !x.IsRift).ToList();
        if (possibleTiles.Count == 0) yield break;
        possibleTiles = possibleTiles.OrderBy(x => Vector3.Distance(x.transform.position, actor.transform.position)).ToList();
        BattleTileController finalTile = approachType switch
        {
            ApproachType.front => possibleTiles[0],
            ApproachType.back => possibleTiles.Last(),
            _ => null,
        };
        yield return actor.StartCoroutine(Move(actor, finalTile));
        Vector3 lookTarget = targetCell.transform.position;
        lookTarget.y = actor.transform.position.y;
        actor.transform.LookAt(lookTarget);
    }
}