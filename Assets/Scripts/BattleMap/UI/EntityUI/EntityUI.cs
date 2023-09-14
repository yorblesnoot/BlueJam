using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EntityUI : MonoBehaviour
{
    [SerializeField] internal Slider sliderHealth;
    [SerializeField] internal Slider sliderDeflect;
    [SerializeField] internal Slider sliderShield;

    public BattleUnit unitActions;

    [field: SerializeField] internal BuffUI buffUI { get; set; }
    private void Update()
    {
        UpdateHealth();
        UpdateDeflect();
        UpdateShield();
    }

    readonly int barSpeedMod = 3;
    public virtual void UpdateHealth()
    {
        sliderHealth.value = Mathf.Lerp(sliderHealth.value, unitActions.currentHealth, Time.deltaTime * barSpeedMod);
    }

    public virtual void UpdateDeflect()
    {
        sliderDeflect.maxValue = Mathf.Max(sliderDeflect.maxValue, unitActions.deflectHealth);
        sliderDeflect.value = Mathf.Lerp(sliderDeflect.value, unitActions.deflectHealth, Time.deltaTime * barSpeedMod);
    }

    public virtual void UpdateShield()
    {
        sliderShield.maxValue = Mathf.Max(sliderShield.maxValue, unitActions.shieldHealth);
        sliderShield.value = Mathf.Lerp(sliderShield.value, unitActions.shieldHealth, Time.deltaTime * barSpeedMod);
    }
    

    public virtual void InitializeHealth()
    {
        int barrierMax = Mathf.RoundToInt(unitActions.loadedStats[StatType.MAXHEALTH]);
        sliderHealth.maxValue = unitActions.loadedStats[StatType.MAXHEALTH];
        sliderHealth.value = unitActions.currentHealth;

        sliderDeflect.maxValue = barrierMax;
        sliderDeflect.value = 0;

        sliderShield.maxValue = barrierMax;
        sliderShield.value = 0;
    }

    public virtual void UpdateBeats(float beatChange) { }
}
