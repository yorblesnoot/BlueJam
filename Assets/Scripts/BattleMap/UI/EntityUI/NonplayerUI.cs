using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class NonplayerUI : EntityUI
{
    public Slider sliderBeats;
    public Slider sliderGhostBeats;
    [SerializeField] NPCHealthPips pips;

    readonly int beatBarSpace = 2;
    
    void Awake()
    {
        unitActions = GetComponentInParent<BattleUnit>();
        EventManager.hideTurnDisplay.AddListener(HideBeatGhost);
    } 

    public override void InitializeHealth()
    {
        base.InitializeHealth();
        pips.SetPips(Mathf.RoundToInt(unitActions.loadedStats[StatType.MAXHEALTH]));
        SetBar(unitActions.loadedStats[StatType.BEATS], TurnManager.beatThreshold + beatBarSpace, sliderBeats, false);
    }

    public override void UpdateBeats(float beatChange)
    {
        if (!unitActions.isDead)
        {
            SetBar(unitActions.loadedStats[StatType.BEATS], TurnManager.beatThreshold + beatBarSpace, sliderGhostBeats, false);
            SetBar(unitActions.loadedStats[StatType.BEATS] + beatChange, TurnManager.beatThreshold + beatBarSpace, sliderBeats, false);
            StartCoroutine(UpdateBar(beatChange, TurnManager.beatThreshold + beatBarSpace, sliderBeats, false));
        }
    }

    public void ShowBeatGhost(float beats)
    {
        SetBar(unitActions.loadedStats[StatType.BEATS] + beats, TurnManager.beatThreshold + beatBarSpace, sliderGhostBeats, false);
    }
    public void HideBeatGhost()
    {
        SetBar(unitActions.loadedStats[StatType.BEATS], TurnManager.beatThreshold + beatBarSpace, sliderGhostBeats, false);
    }
}
