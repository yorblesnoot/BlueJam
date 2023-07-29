using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EntityUI : MonoBehaviour
{
    public Slider sliderHealth;
    public Slider sliderDeflect;
    public Slider sliderShield;

    public GameObject blankCard;
    public GameObject unitCanvas;
    public BattleUnit unitActions;

    int visualDeflectMax;
    int visualShieldMax;

    [field: SerializeField]public BuffUI buffUI { get; set; }

    public void UpdateHealth(int change)
    {
        StartCoroutine(UpdateBar(change, Mathf.RoundToInt(unitActions.loadedStats[StatType.MAXHEALTH]), sliderHealth));
    }

    public void UpdateDeflect(int amount)
    {
        if(unitActions.deflectHealth > visualDeflectMax) visualDeflectMax = unitActions.deflectHealth;
        
        StartCoroutine(UpdateBar(amount, visualDeflectMax, sliderDeflect));
        
    }

    public void UpdateShield(int amount)
    {
        if (unitActions.shieldHealth > visualShieldMax) visualShieldMax = unitActions.shieldHealth;
        StartCoroutine(UpdateBar(amount, visualShieldMax, sliderShield));
    }
    

    public virtual void InitializeHealth()
    {
        visualDeflectMax = Mathf.RoundToInt(unitActions.loadedStats[StatType.MAXHEALTH]);
        visualShieldMax = Mathf.RoundToInt(unitActions.loadedStats[StatType.MAXHEALTH]);
        SetBar(unitActions.currentHealth, unitActions.loadedStats[StatType.MAXHEALTH], sliderHealth);
        SetBar(unitActions.deflectHealth, visualDeflectMax, sliderDeflect);
        SetBar(unitActions.shieldHealth, visualShieldMax, sliderShield);
    }

    readonly int changeSteps = 20;
    public IEnumerator UpdateBar(float change, float max, Slider slider, bool vanish = true)
    {
        slider.gameObject.SetActive(true);
        float sliderChange = change / max;
        for(int i = 0; i < changeSteps; i++)
        {
            slider.value -= sliderChange / changeSteps;
            yield return null;
        }
        if(vanish && slider.value <= 0) slider.gameObject.SetActive(false);
    }

    public void SetBar(float current, float max, Slider slider, bool vanish = true)
    {
        slider.value = current / max;
        if (slider.value <= 0 && vanish == true)
        {
            slider.gameObject.SetActive(false);
        }
    }

    public virtual void UpdateBeats(float beatChange) { }
}
