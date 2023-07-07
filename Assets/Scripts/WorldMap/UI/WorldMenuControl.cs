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

    [SerializeField] CameraController cameraController;
    public void OpenCrafting()
    {
        ToggleWindow(essenceCrafting, true);
    }
    public void OpenDeckView()
    {
        ToggleWindow(deckView, true);
    }
    public void CloseLast()
    {
        ToggleWindow(lastOpened, false);
    }
    public void ToggleWindow(GameObject window, bool value)
    {
        if (WorldPlayerControl.playerState != WorldPlayerState.IDLE && WorldPlayerControl.playerState != WorldPlayerState.MENUS) return;
        lastOpened = window;
        window.SetActive(value);
        cameraController.enabled = !value;

        foreach (var component in hudComponents)
        {
            component.SetActive(!value);
        }
        close.gameObject.SetActive(value);

        if (value == true) WorldPlayerControl.playerState = WorldPlayerState.MENUS;
        else WorldPlayerControl.playerState = WorldPlayerState.IDLE;
    }

}
