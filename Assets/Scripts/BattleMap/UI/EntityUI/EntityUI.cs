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

    [field: SerializeField]public BuffUI buffUI { get; set; }

    public void UpdateHealth()
    {
        StartCoroutine(UpdateBar(unitActions.currentHealth, unitActions.maxHealth, sliderHealth));
        StartCoroutine(UpdateBar(unitActions.deflectHealth, unitActions.maxHealth, sliderDeflect));
        StartCoroutine(UpdateBar(unitActions.shieldHealth, unitActions.maxHealth, sliderShield));
    }

    public virtual void InitializeHealth()
    {
        SetBar(unitActions.currentHealth, unitActions.maxHealth, sliderHealth);
        SetBar(unitActions.deflectHealth, unitActions.maxHealth, sliderDeflect);
        SetBar(unitActions.shieldHealth, unitActions.maxHealth, sliderShield);
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
        if (slider.value == 0 && vanish == true)
        {
            slider.gameObject.SetActive(false);
        }
    }

    public void SetBar(float current, float max, Slider slider, bool vanish = true)
    {
        slider.value = current / max;
        if (slider.value == 0 && vanish == true)
        {
            slider.gameObject.SetActive(false);
        }
    }
}
