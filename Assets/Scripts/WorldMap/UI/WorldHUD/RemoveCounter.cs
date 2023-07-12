using TMPro;
using UnityEngine;

public class RemoveCounter : MonoBehaviour
{
    [SerializeField] RunData runData;
    [SerializeField] TMP_Text counter;
    private void Awake()
    {
        EventManager.updateWorldCounters.AddListener(UpdateCounter);
    }

    private void OnEnable()
    {
        UpdateCounter();
    }

    public void UpdateCounter()
    {
        counter.text = runData.RemoveStock.ToString();
    }
}
