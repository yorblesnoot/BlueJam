using UnityEngine;
using UnityEngine.Events;

public class TargetConfirmed : UnityEvent<BattleTileController> {}
public class ToggleAOE : UnityEvent<CardPlus> { }
public class CheckForTriggers : UnityEvent<CardEffectPlus, BattleUnit, BattleUnit> { }
public class ClickedCard : UnityEvent<CardPlus, GameObject> { }
public class RequestMapReferences : UnityEvent<MapLauncher> { }
public class PlayerAtWorldLocation: UnityEvent<Vector2Int> { }

public static class EventManager
{

    //battlemap events
    public static TargetConfirmed targetConfirmed = new();
    public static UnityEvent endEmphasis = new();
    public static UnityEvent clearActivation = new();
    public static ToggleAOE showAOE = new();
    public static UnityEvent clearAOE = new();
    public static UnityEvent allowTriggers = new();
    public static CheckForTriggers checkForTriggers = new();
    public static UnityEvent hideTurnDisplay = new();

    //worldmap events
    public static ClickedCard clickedCard = new();
    public static UnityEvent clearWorldDestination = new();
    public static UnityEvent updateWorldCounters = new();
    public static UnityEvent updateItemUI = new();
    public static UnityEvent awardItem = new();
    public static UnityEvent updateWorldHealth = new();
    public static UnityEvent prepareForBattle = new();
    public static PlayerAtWorldLocation playerAtWorldLocation = new();

    //global events

    public static RequestMapReferences requestMapReferences = new();
}
