using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InfoTagControl : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text tagText;
    [SerializeField] TMP_Text blockText;
    [SerializeField] TMP_Text shieldText;

    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text timeText;

    public static UnityEvent hideTags = new();

    private void Awake()
    {
        hideTags.AddListener(() => gameObject.SetActive(false));
    }
    public void ShowTag(BattleUnit unit, UnitStats stats)
    {
        Tutorial.CompleteStage(TutorialFor.BATTLEACTIONS, 2);
        gameObject.SetActive(true);
        healthText.text = $"{unit.currentHealth} / {Mathf.RoundToInt(unit.loadedStats[StatType.MAXHEALTH])}";
        timeText.text = $"{Math.Round(unit.loadedStats[StatType.BEATS], 1)} / {TurnManager.beatThreshold}";

        shieldText.text = unit.shieldHealth.ToString();
        blockText.text = unit.deflectHealth.ToString();

        nameText.text = stats.unitName;
        tagText.text = stats.unitDescription;
    }
}
