using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMenuPlus : MonoBehaviour
{
    [SerializeField] List<ActivateableWindow> windowControls;
    GameObject currentOpen;
    private void Awake()
    {
        foreach (var window in windowControls)
        {
            if(window.button) window.button.onClick.AddListener(() => OpenWindow(window.window));
        }
    }

    private void OpenWindow(GameObject window)
    {
        if (WorldPlayerControl.playerState != WorldPlayerState.MENUS && WorldPlayerControl.playerState != WorldPlayerState.IDLE) return;
        //check if player is in menu locked state: card display, item award
        if (currentOpen == window)
        {
            CloseCurrent();
            return;
        }
        CloseCurrent();
        currentOpen = window;
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        StartCoroutine(window.SlideIn(.2f, .1f));
    }

    private void CloseCurrent()
    {
        if (WorldPlayerControl.playerState != WorldPlayerState.MENUS && WorldPlayerControl.playerState != WorldPlayerState.IDLE) return;
        if (!currentOpen) return;
        StartCoroutine(currentOpen.SlideOut(.2f));
        currentOpen = null;
    }

    private void Update()
    {
        foreach(var window in windowControls)
        {
            if(window.keyCode != KeyCode.None && Input.GetKeyDown(window.keyCode))
            {
                OpenWindow(window.window);
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape)) CloseCurrent();
    }

    [System.Serializable]
    class ActivateableWindow
    {
        public GameObject window;
        public KeyCode keyCode;
        public Button button;
    }
}
