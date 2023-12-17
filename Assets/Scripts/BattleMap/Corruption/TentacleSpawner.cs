using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TentacleSpawner : CorruptionElement
{
    [SerializeField] GameObject tentacle;
    List<BattleTileController> validSpots;
    int baseAmount;

    private void Awake()
    {
        validSpots = new();
    }

    public override void Activate(int amount)
    {
        baseAmount = amount;
        InitialTentacles(amount);
        TurnManager.globalDeathCheck.AddListener(ResummonDeadTentacles);
    }

    public void InitialTentacles(int amount)
    {
        Dictionary<Vector2Int, GameObject> battleMap = MapTools.tileMap.forward;
        validSpots = battleMap.Values.Select(x => x.GetComponent<BattleTileController>()).ToList();
        validSpots = validSpots.Where(x => x.IsRift).ToList();
        SummonTentacles(amount);
    }

    private List<NonplayerUnit> SummonTentacles(int amount)
    {
        List<NonplayerUnit> output = new();
        while (amount > 0)
        {
            int selection = Random.Range(0, validSpots.Count);
            NonplayerUnit spawned = Instantiate(tentacle, validSpots[selection].unitPosition, PhysicsHelper.RandomCardinalRotate()).GetComponent<NonplayerUnit>();
            MapTools.ReportPositionChange(spawned, validSpots[selection]);
            output.Add(spawned);
            validSpots.RemoveAt(selection);
            amount--;
            if (validSpots.Count == 0) return null;
        }
        return output;
    }

    void ResummonDeadTentacles()
    {
        List<ITurnTakingNonplayer> tentacles = TurnManager.turnTakers.Where(x => x.Allegiance == AllegianceType.VOID).ToList();
        Debug.Log(tentacles.Count);
        if (tentacles.Count >= baseAmount) return;

        List<NonplayerUnit> tentacleUnits = SummonTentacles(baseAmount - tentacles.Count);
        foreach(NonplayerUnit unit in tentacleUnits)
        {
            HandPlus hand = unit.myHand;
            unit.myUI.InitializeHealth();
            hand.BuildVisualDeck();
            hand.DrawPhase();
            TurnManager.ReportTurn(unit);
        }
    }
}
