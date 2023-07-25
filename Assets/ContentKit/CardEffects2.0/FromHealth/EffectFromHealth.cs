using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectFromHealth", menuName = "ScriptableObjects/CardEffects/FromHealth")]
public class EffectFromHealth : CardEffectPlus
{
    [SerializeField] CardEffectPlus outputEffect;
    enum HealthType
    {
        missing,
        current
    }
    enum CheckedUnit
    {
        self,
        target
    }
    [SerializeField] HealthType healthType;
    [SerializeField] CheckedUnit checkedUnit;
    private void Reset()
    {
        effectClass = CardClass.ATTACK;
    }

    public override string GetEffectDescription(IUnitStats player)
    {
        outputEffect.Initialize();
        if(checkedUnit == CheckedUnit.self)
            return $"for every <color=red>{scalingMultiplier}%</color> of your {healthType} health, {outputEffect.GenerateDescription(player)}";
        else
            return $"for every <color=red>{scalingMultiplier}%</color> of target's {healthType} health, {outputEffect.GenerateDescription(player)}";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        float relevantHealth;
        BattleUnit relevantUnit;
        if (checkedUnit == CheckedUnit.self) relevantUnit = actor;
        else relevantUnit = targetCell.unitContents;

        if (healthType == HealthType.missing) relevantHealth = relevantUnit.maxHealth - relevantUnit.currentHealth;
        else relevantHealth = relevantUnit.currentHealth;
        relevantHealth /= relevantUnit.maxHealth;

        int repetitions = Mathf.FloorToInt(relevantHealth * 100 / scalingMultiplier);
        for(int i = 0; i < repetitions; i++)
        {
            actor.StartCoroutine(outputEffect.Execute(actor, targetCell));
            yield return new WaitForSeconds(.2f);
        }
    }
}
