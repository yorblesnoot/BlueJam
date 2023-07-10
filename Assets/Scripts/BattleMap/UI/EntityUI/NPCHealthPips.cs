using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHealthPips : MonoBehaviour
{
    [SerializeField] List<GameObject> pips;
    [SerializeField] RectTransform rectTransform;
    readonly int pipFactor = 10;

    public void SetPips(int maxHealth)
    {
        //divide the health bar into regions based on max health and health/region
        int pipAmount = maxHealth / pipFactor;
        while(pipAmount > pips.Count)
        {
            GameObject newPip = Instantiate(pips[0], rectTransform);
            pips.Add(newPip);
        }
        //get size of each region
        float pipSpacing = rectTransform.rect.width / pipAmount;
        //start at the far left of the overlay
        Vector3 startPoint = new();
        startPoint.x -= rectTransform.rect.width/2;
        
        //place a pip every *region*
        for(int i = 1; i < pipAmount; i++)
        {
            pips[i].SetActive(true);
            Vector3 placement = startPoint;
            placement.x += pipSpacing*(i);
            pips[i].transform.localPosition = placement;
        }
    }
}
