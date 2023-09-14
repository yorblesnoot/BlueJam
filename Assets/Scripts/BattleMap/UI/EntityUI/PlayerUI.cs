using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : EntityUI
{
    [SerializeField] TMP_Text healthDisplay;
    [SerializeField] TMP_Text shieldDisplay;
    [SerializeField] TMP_Text blockDisplay;
    public override void UpdateHealth()
    {
        base.UpdateHealth();
        SetHealthDisplay();
    }

    public override void UpdateDeflect()
    {
        base.UpdateDeflect();
        blockDisplay.text = $"{unitActions.deflectHealth}";
    }

    public override void UpdateShield()
    {
        base.UpdateShield();
        shieldDisplay.text = $"{unitActions.shieldHealth}";
    }

    public override void InitializeHealth()
    {
        int barrierMax = Mathf.RoundToInt(unitActions.loadedStats[StatType.MAXHEALTH] * unitActions.loadedStats[StatType.BARRIER] / 20);
        sliderHealth.maxValue = unitActions.loadedStats[StatType.MAXHEALTH];
        sliderHealth.value = unitActions.currentHealth;
        SetHealthDisplay();

        sliderDeflect.maxValue = barrierMax;
        sliderDeflect.value = 0;
        blockDisplay.text = $"{unitActions.deflectHealth}";

        sliderShield.maxValue = barrierMax;
        sliderShield.value = 0;
        shieldDisplay.text = $"{unitActions.shieldHealth}";
    }

    void SetHealthDisplay()
    {
        healthDisplay.text = $"{unitActions.currentHealth}/{Mathf.RoundToInt(unitActions.loadedStats[StatType.MAXHEALTH])}";
    }

}