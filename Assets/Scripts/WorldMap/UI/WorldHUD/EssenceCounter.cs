using TMPro;
using UnityEngine;

public class EssenceCounter : MonoBehaviour
{
    [SerializeField] RunData runData;
    [SerializeField] TMP_Text counter;
    void Awake()
    {
        EssenceCrafting.craftWindowClosed.AddListener(UpdateCounter);
    }

    private void Start()
    {
        UpdateCounter();
    }

    void UpdateCounter()
    {
        int count = runData.essenceInventory.Count;
        if (count == 0) counter.text = "";
        else counter.text = count.ToString();
    }
}
