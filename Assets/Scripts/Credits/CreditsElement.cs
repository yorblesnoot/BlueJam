using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditsElement : MonoBehaviour
{
    [SerializeField] List<string> elementNames;
    [SerializeField] Transform header;
    [SerializeField] GameObject nameLine;
    [SerializeField] Transform nameBox;
    private void Awake()
    {
        header.localPosition = new Vector3(0, nameBox.GetComponent<RectTransform>().rect.height + nameBox.transform.localPosition.y, 0);
        foreach (var element in elementNames)
        {
            GameObject spawnedLine = Instantiate(nameLine);
            spawnedLine.transform.SetParent(nameBox, false);
            spawnedLine.GetComponent<TMP_Text>().text = element;
        }
    }
}
