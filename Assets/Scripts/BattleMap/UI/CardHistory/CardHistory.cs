using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHistory : MonoBehaviour
{
    [SerializeField] List<HistoryEntry> _historyElements;
    static List<HistoryEntry> historyElements;
    static Dictionary<BattleUnit, HistoryEntry> historyTable;

    private void Awake()
    {
        historyElements = new(_historyElements);
    }

    public static void AddCardToHistory(CardPlus card, BattleUnit owner)
    {
        historyTable ??= new();
        if(!historyTable.TryGetValue(owner, out HistoryEntry entry))
        {
            historyTable.Add(owner, historyElements[0]);
            entry = historyElements[0];
            entry.AssignToUnit(owner);
            historyElements.RemoveAt(0);
        }
        entry.AssignCard(card);
    }

    
}
