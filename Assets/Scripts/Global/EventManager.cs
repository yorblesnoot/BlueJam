using UnityEngine;
using UnityEngine.Events;

public class TargetConfirmed : UnityEvent<GameObject> {}
public class ToggleAOE : UnityEvent<string[,]> { }

public class GetWorldDestination : UnityEvent<int, int> {}
public class RegisterSlot : UnityEvent<EssenceCrafting> { }
public class AddCard : UnityEvent<CardPlus> { }

public static class EventManager
{

    //battlemap events
    public static TargetConfirmed targetConfirmed = new();
    public static UnityEvent initalizeBattlemap = new();
    public static UnityEvent clearActivation = new();
    public static ToggleAOE showAOE = new();
    public static UnityEvent clearAOE = new();
    public static UnityEvent allowTriggers = new();

    //worldmap events
    public static UnityEvent worldMove = new();

    public static RegisterSlot registerSlot = new();
    public static AddCard addCard = new();

    public static GetWorldDestination getWorldDestination = new();
    public static UnityEvent clearWorldDestination = new();

}
