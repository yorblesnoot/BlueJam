using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMenuControl : MonoBehaviour
{
    public GameObject essenceCrafting;
    public GameObject deckView;

    GameObject lastOpened;

    [SerializeField] List<GameObject> hudComponents;

    public Button close;

    public void OpenCrafting()
    {
        Tutorial.CompleteStage(TutorialFor.WORLDCRAFTING, 1);
        Tutorial.EnterStage(TutorialFor.WORLDCRAFTING, 2, "Drag an essence you want cards from into the top slot, then drag some essences you don't want into the bottom slots. When you're ready, hit the hammer button.");
        ToggleWindow(essenceCrafting, true);
    }
    public void OpenDeckView()
    {
        Tutorial.CompleteStage(TutorialFor.WORLDCRAFTING, 3, true);
        Tutorial.Initiate(TutorialFor.WORLDDECK, TutorialFor.WORLDCRAFTING);
        Tutorial.EnterStage(TutorialFor.WORLDDECK, 1, "This is my current deck, which I draw cards from in battle. Click the button on the left to toggle card removal.");
        ToggleWindow(deckView, true);
    }
    public void CloseLast()
    {
        Tutorial.CompleteStage(TutorialFor.WORLDDECK, 2, true);
        ToggleWindow(lastOpened, false);
    }
    public void ToggleWindow(GameObject window, bool value)
    {
        if (WorldPlayerControl.playerState != WorldPlayerState.IDLE && WorldPlayerControl.playerState != WorldPlayerState.MENUS) return;
        lastOpened = window;
        window.SetActive(value);

        foreach (var component in hudComponents)
        {
            component.SetActive(!value);
        }
        close.gameObject.SetActive(value);

        if (value == true) WorldPlayerControl.playerState = WorldPlayerState.MENUS;
        else WorldPlayerControl.playerState = WorldPlayerState.IDLE;
    }

}
