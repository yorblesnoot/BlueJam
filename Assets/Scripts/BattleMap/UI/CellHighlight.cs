using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HighlightMode { LEGAL, ILLEGAL, AOE, OFF }
public class CellHighlight : MonoBehaviour
{
    [SerializeField] GameObject outerSelect;
    [SerializeField] GameObject innerSelect;
    Material outerMaterial;
    Material innerMaterial;

    string baseColor = "_BaseColor";
    string emissiveColor = "_EmissionColor";

    int emissiveIntensity = 2;

    public Dictionary<HighlightMode, Color32> colorMap = new()
    {
        {HighlightMode.LEGAL, new Color32(13,157,255,255) },
        {HighlightMode.ILLEGAL, new Color32(217,38,38,255) },
        {HighlightMode.AOE, new Color32(255,103,0,255) },
        {HighlightMode.OFF, new Color32() }
    };

    int exteriorMinAlpha = 155;
    int interiorBaseAlpha = 50;
    private void Awake()
    {
        outerMaterial = outerSelect.GetComponent<Renderer>().material;
        innerMaterial = innerSelect.GetComponent<Renderer>().material;
    }

    public void ChangeHighlightMode(HighlightMode mode)
    {
        if(mode == HighlightMode.OFF)
        {
            outerSelect.SetActive(false);
            innerSelect.SetActive(false);
            return;
        }
        outerSelect.SetActive(true);
        innerSelect.SetActive(true);
        StopAllCoroutines();
        Color32 newColor = colorMap[mode];
        outerMaterial.SetColor(baseColor, new Color32(newColor.r, newColor.g, newColor.b, (byte)exteriorMinAlpha));
        outerMaterial.SetColor(emissiveColor, (Color)newColor*emissiveIntensity);
        innerMaterial.SetColor(baseColor, new Color32(newColor.r, newColor.g, newColor.b, (byte)interiorBaseAlpha));
        innerMaterial.SetColor(emissiveColor, (Color)newColor*emissiveIntensity);
        if(mode != HighlightMode.OFF)
        {
            StartCoroutine(AlphaPulse(newColor));
        }
    }

    IEnumerator AlphaPulse(Color32 newColor)
    {
        while (true)
        {
            for (int i = 0; i < 100; i++)
            {
                outerMaterial.SetColor(baseColor, new Color32(newColor.r, newColor.g, newColor.b, (byte)(exteriorMinAlpha+i)));
                yield return new WaitForSeconds(.01f);
            }
            for (int i = 0; i < 100; i++)
            {
                outerMaterial.SetColor(baseColor, new Color32(newColor.r, newColor.g, newColor.b, (byte)(255-i)));
                yield return new WaitForSeconds(.01f);
            }
        }
    }

}


