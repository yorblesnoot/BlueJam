using UnityEngine;
using UnityEngine.Events;

public class TargetConfirmed : UnityEvent<BattleTileController> {}
public class ToggleAOE : UnityEvent<bool[,]> { }
public class CheckForTriggers : UnityEvent<CardEffectPlus, BattleUnit, BattleUnit> { }

public class GetWorldDestination : UnityEvent<int, int> {}
public class ClickedCard : UnityEvent<CardPlus, GameObject> { }

public class RequestMapReferences : UnityEvent<MapLauncher> { }

public static class EventManager
{

    //battlemap events
    public static TargetConfirmed targetConfirmed = new();
    public static UnityEvent initalizeBattlemap = new();
    public static UnityEvent clearActivation = new();
    public static ToggleAOE showAOE = new();
    public static UnityEvent clearAOE = new();
    public static UnityEvent allowTriggers = new();
    public static CheckForTriggers checkForTriggers = new();

    //worldmap events
    public static UnityEvent worldMove = new();

    public static ClickedCard clickedCard = new();

    public static GetWorldDestination getWorldDestination = new();
    public static UnityEvent clearWorldDestination = new();

    public static UnityEvent updateWorldCounters = new();

    public static UnityEvent updateItemUI = new();
    public static UnityEvent awardItem = new();
    public static UnityEvent updateWorldHealth = new();

    //global events

    public static RequestMapReferences requestMapReferences = new();
}
