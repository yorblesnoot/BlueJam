using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WorldHealthBar : MonoBehaviour
{
    public Slider sliderHealth;
    [SerializeField] RunData RunData;
    [SerializeField] TMP_Text healthDisplay;
    private void Awake()
    {
        EventManager.updateWorldHealth.AddListener(UpdateHealth);
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        StartCoroutine(UpdateBar(RunData.currentHealth, sliderHealth));
    }

    readonly float changeTime = .4f;
    public virtual IEnumerator UpdateBar(float current, Slider slider)
    {
        slider.maxValue = RunData.playerStats.maxHealth;
        float timeElapsed = 0;
        float start = slider.value;
        while (timeElapsed < changeTime)
        {
            slider.value = Mathf.Lerp(start, current, timeElapsed/changeTime);
            healthDisplay.text = $"{Mathf.RoundToInt(sliderHealth.value)}/{RunData.playerStats.maxHealth}";
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        slider.value = current;
        healthDisplay.text = $"{Mathf.RoundToInt(sliderHealth.value)}/{RunData.playerStats.maxHealth}";
    }
}
