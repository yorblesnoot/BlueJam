using TMPro;
using UnityEngine;

public class DifficultyCounter : MonoBehaviour
{
    [SerializeField] RunData runData;
    [SerializeField] TMP_Text counter;
    private void Awake()
    {
        EventManager.updateWorldCounters.AddListener(UpdateCounter);
    }

    public void UpdateCounter()
    {
        counter.text = $"Threat: {runData.runDifficulty + Settings.Profile.BaseThreatReduction}";
    }
}
