using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryEntry : MonoBehaviour
{
    public HistoryMiniCardDisplay display;
    [SerializeField] Image token;

    HatControl hatControl;

    BattleUnit asignee;

    private void Awake()
    {
        hatControl = new(token);
    }
    public void AssignToUnit(BattleUnit unit)
    {
        gameObject.SetActive(true);
        asignee = unit;
        hatControl.DeployHat(unit.myHand.deckRecord.hat);
    }

    public void AssignCard(CardPlus card)
    {
        display.PopulateMiniCard(card, asignee);
    }
}
