using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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

    public void UpdateHealth()
    {
        StartCoroutine(UpdateBar(unitActions.currentHealth, unitActions.maxHealth, sliderHealth));
    }

    public void UpdateBarriers()
    {
        if(unitActions.deflectHealth > visualDeflectMax) visualDeflectMax = unitActions.deflectHealth;
        if(unitActions.shieldHealth > visualShieldMax) visualShieldMax = unitActions.shieldHealth;
        StartCoroutine(UpdateBar(unitActions.deflectHealth, visualDeflectMax, sliderDeflect));
        StartCoroutine(UpdateBar(unitActions.shieldHealth, visualShieldMax, sliderShield));
    }

    public virtual void InitializeHealth()
    {
        visualDeflectMax = unitActions.maxHealth;
        visualShieldMax = unitActions.maxHealth;
        SetBar(unitActions.currentHealth, unitActions.maxHealth, sliderHealth);
        SetBar(unitActions.deflectHealth, visualDeflectMax, sliderDeflect);
        SetBar(unitActions.shieldHealth, visualShieldMax, sliderShield);
    }

    public IEnumerator UpdateBar(float current, float max, Slider slider, bool vanish = true)
    {
        if(current > 0) { slider.gameObject.SetActive(true);}
        int changeInterval = 10;

        float toValue = current / max;

        float mod = (slider.value - toValue) / changeInterval;

        for (int count = 0; count < changeInterval; count++)
        {
            slider.value -= mod;
            yield return new WaitForSeconds(.01f);
        }
        SetBar(current, max, slider, vanish);
    }

    public void SetBar(float current, float max, Slider slider, bool vanish = true)
    {
        slider.value = current / max;
        if (slider.value <= 0 && vanish == true)
        {
            slider.gameObject.SetActive(false);
        }
    }
}
