using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditsElement : MonoBehaviour
{
    [SerializeField] List<string> elementNames;
    [SerializeField] GameObject nameLine;
    [SerializeField] Transform nameBox;
    private void Awake()
    {
        foreach (var element in elementNames)
        {
            GameObject spawnedLine = Instantiate(nameLine);
            spawnedLine.transform.SetParent(nameBox, false);
            spawnedLine.GetComponent<TMP_Text>().text = element;
        }
    }
}
