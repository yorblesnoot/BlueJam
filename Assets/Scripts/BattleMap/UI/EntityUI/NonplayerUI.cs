using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;


public class NonplayerUI : EntityUI
{
    public Slider sliderBeats;
    public Slider sliderGhostBeats;
    [SerializeField] GameObject beatPip;
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
        sliderBeats.value = unitActions.loadedStats[StatType.BEATS];
        SetBeatMax();
    }

    public override void UpdateBeats(float beatChange)
    {
        if (unitActions.isDead) return;

        SetBeatMax(beatChange);
        StartCoroutine(UpdateBeatBar(beatChange));
    }

    readonly int changeSteps = 20;
    public IEnumerator UpdateBeatBar(float change)
    {
        for (int i = 0; i < changeSteps; i++)
        {
            sliderBeats.value -= change / changeSteps;
            yield return null;
        }
        sliderGhostBeats.value = sliderBeats.value;
    }

    void SetBeatMax(float beatChange = 0)
    {
        float newMax = Mathf.Max(TurnManager.beatThreshold + beatBarSpace, sliderBeats.value - beatChange);
        sliderBeats.maxValue = newMax;
        sliderGhostBeats.maxValue = newMax;

        int segmentAmount = Mathf.RoundToInt(sliderBeats.maxValue - sliderBeats.minValue);
        float barWidth = sliderBeats.GetComponent<RectTransform>().rect.width;
        float segmentSize = barWidth/segmentAmount;
        int segmentCount = TurnManager.beatThreshold - Mathf.RoundToInt(sliderBeats.minValue);
        float pipX = barWidth/-2 + (segmentSize * segmentCount);
        Vector3 finalPosition = beatPip.transform.localPosition;
        finalPosition.x = pipX;
        beatPip.transform.localPosition = finalPosition; 
    }

    public void ShowBeatGhost(float beats)
    {
        sliderGhostBeats.value = unitActions.loadedStats[StatType.BEATS] + beats;
    }
    public void HideBeatGhost()
    {
        sliderGhostBeats.value = unitActions.loadedStats[StatType.BEATS];
    }
}
