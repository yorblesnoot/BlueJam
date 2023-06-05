using TMPro;
using UnityEngine;

public class KeyCounter : MonoBehaviour
{
    [SerializeField] RunData runData;
    [SerializeField] TMP_Text counter;
    private void Awake()
    {
        EventManager.updateWorldCounters.AddListener(UpdateCounter);
    }

    public void UpdateCounter()
    {
        counter.text = runData.KeyStock.ToString();
    }
}