using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryEntry : MonoBehaviour
{
    [SerializeField] MiniCardDisplay display;
    [SerializeField] Image token;

    HatControl hatControl;

    BattleUnit asignee;

    private void Awake()
    {
        hatControl = new(token);
    }
    public void AssignToUnit(BattleUnit unit)
    {
        asignee = unit;
        hatControl.DeployHat(unit.myHand.deckRecord.hat);
    }

    public void AssignCard(CardPlus card)
    {
        gameObject.SetActive(true);
        display.PopulateCard(card, asignee);
        transform.SetAsFirstSibling();
    }
}
