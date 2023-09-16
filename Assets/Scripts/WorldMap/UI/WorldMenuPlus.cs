using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMenuPlus : MonoBehaviour
{
    [SerializeField] List<ActivateableWindow> windowControls;
    ActivateableWindow currentOpen;
    private void Awake()
    {
        foreach (var window in windowControls)
        {
            if(window.button) window.button.onClick.AddListener(() => OpenWindow(window));
            window.position = window.window.transform.localPosition;
        }
    }

    bool PlayerInLegalState()
    {
        if(WorldPlayerControl.playerState == WorldPlayerState.PATHING) return false;
        if (WorldPlayerControl.playerState == WorldPlayerState.IDLE) WorldPlayerControl.playerState = WorldPlayerState.MENUS;
        return true;
    }

    private void OpenWindow(ActivateableWindow menu)
    {
        if (!PlayerInLegalState()) return;
        if (currentOpen == menu)
        {
            CloseCurrent();
            return;
        }
        CloseCurrent();
        currentOpen = menu;
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        StartCoroutine(menu.window.SlideIn(.2f, .1f));
    }

    public void CloseCurrent()
    {
        if (currentOpen == null) return;
        ResetWindow(currentOpen);
        StartCoroutine(currentOpen.window.SlideOut(.2f));
        currentOpen = null;
        if (WorldPlayerControl.playerState == WorldPlayerState.MENUS)
            WorldPlayerControl.playerState = WorldPlayerState.IDLE;
    }

    void ResetWindow(ActivateableWindow menu)
    {
        StopAllCoroutines();
        menu.window.transform.localPosition = currentOpen.position;
    }

    private void Update()
    {
        foreach(var window in windowControls)
        {
            if(window.keyCode != KeyCode.None && Input.GetKeyDown(window.keyCode))
            {
                OpenWindow(window);
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
        [HideInInspector] public Vector3 position;
    }
}
