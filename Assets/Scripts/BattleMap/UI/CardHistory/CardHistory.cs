using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardHistory : MonoBehaviour
{
    [SerializeField] List<HistoryEntry> _historyElements;
    static List<HistoryEntry> historyElements;

    public static CardHistory Main;

    static CardPlus lastPlayed;

    private void Awake()
    {
        historyElements = new(_historyElements);
        Main = this;
        lastPlayed = null;
    }

    public static void AddCardToHistory(CardPlus card, BattleUnit owner)
    {
        if(card == lastPlayed)
        {
            historyElements[0].display.AugmentUseCount();
            return;
        }

        lastPlayed = card;
        HistoryEntry used = historyElements.Last();
        historyElements.Remove(used);
        used.AssignToUnit(owner);
        used.AssignCard(card);
        historyElements.Insert(0, used);
        Main.CycleElements();
    }

    [SerializeField] float spacing;
    [SerializeField] float moveTime;
    [SerializeField] float enterTime;
    [SerializeField] float exitTime;
    float exitDistance = 300;
    void CycleElements()
    {
        Vector3 startPosition = Vector3.zero;
        historyElements[0].transform.localPosition = new Vector3(startPosition.x-spacing, startPosition.y, startPosition.z);
        StartCoroutine(historyElements[0].gameObject.LerpTo(startPosition, enterTime, true));
        //play an entry animation
        for (int i = 1; i < historyElements.Count-1; i++)
        {
            startPosition.y -= spacing;
            StartCoroutine(historyElements[i].gameObject.LerpTo(startPosition, moveTime, true));
        }
        startPosition.x += exitDistance;
        StartCoroutine(historyElements.Last().gameObject.LerpTo(startPosition, exitTime, true));
    }

    
}
