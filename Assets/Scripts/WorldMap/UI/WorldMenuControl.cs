using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMenuControl : MonoBehaviour
{
    public GameObject essenceCrafting;
    public GameObject deckView;

    GameObject lastOpened;

    public Button openCrafting;
    public Button openMenu;
    public Button openDeck;

    public Button close;

    [SerializeField] CameraController cameraController;
    public void OpenCrafting()
    {
        //toggle relevant buttons
        ToggleWindow(essenceCrafting, true);
        lastOpened = essenceCrafting;
    }
    public void OpenDeckView()
    {
        //toggle relevant buttons
        ToggleWindow(deckView, true);
        lastOpened = deckView;
    }
    public void CloseLast()
    {
        //toggle relevant buttons
        ToggleWindow(lastOpened, false);
    }


    public void ToggleWindow(GameObject window, bool value)
    {
        window.SetActive(value);
        cameraController.enabled = !value;

        openCrafting.gameObject.SetActive(!value);
        openMenu.gameObject.SetActive(!value);
        openDeck.gameObject.SetActive(!value);

        close.gameObject.SetActive(value);
    }

}
