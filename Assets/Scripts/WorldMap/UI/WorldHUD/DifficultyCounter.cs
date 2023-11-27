using TMPro;
using UnityEngine;

public class DifficultyCounter : MonoBehaviour
{
    [SerializeField] RunData runData;
    [SerializeField] TMP_Text counter;
    private void Awake()
    {
        EventManager.updateWorldCounters.AddListener(UpdateCounter);
        UpdateCounter();
    }

    public void UpdateCounter()
    {
        counter.text = $"Threat: {runData.ThreatLevel + Mathf.RoundToInt(Settings.Balance[BalanceParameter.ThreatHandicap])}";
    }
}
