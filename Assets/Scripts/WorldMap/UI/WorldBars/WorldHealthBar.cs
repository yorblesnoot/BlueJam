using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WorldHealthBar : MonoBehaviour
{
    public Slider sliderHealth;
    [SerializeField] RunData RunData;
    private void Awake()
    {
        EventManager.updateWorldHealth.AddListener(UpdateHealth);
    }

    public void UpdateHealth()
    {
        StartCoroutine(UpdateBar(RunData.currentHealth, RunData.playerStats.maxHealth, sliderHealth));
    }

    public virtual IEnumerator UpdateBar(float current, float max, Slider slider)
    {
        float changeInterval = 10;

        float toValue = current / max;

        float mod = (slider.value - toValue) / changeInterval;
        for (int count = 0; count < changeInterval; count++)
        {
            slider.value -= mod;
            yield return new WaitForSeconds(.01f);
        }
    }
}
