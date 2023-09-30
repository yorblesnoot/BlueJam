using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WorldMenuPlus : MonoBehaviour
{
    [SerializeField] List<ActivateableWindow> windowControls;
    [SerializeField] ActivateableWindow craftWindow;

    ActivateableWindow currentOpen;
    [SerializeField] GameObject bugWindow;

    public static UnityEvent openAltCraft = new();
    private void Awake()
    {
        foreach (var window in windowControls)
        {
            if(window.button) window.button.onClick.AddListener(() => OpenWindow(window));
            window.position = window.window.transform.localPosition;
        }
        craftWindow.position = craftWindow.window.transform.localPosition;
        openAltCraft.AddListener(() => ForceMenu(craftWindow));
    }

    bool PlayerInLegalState()
    {
        if(WorldPlayerControl.playerState == WorldPlayerState.PATHING) return false;
        if (WorldPlayerControl.playerState == WorldPlayerState.IDLE) WorldPlayerControl.playerState = WorldPlayerState.MENUS;
        return true;
    }

    private void ForceMenu(ActivateableWindow menu)
    {
        CloseCurrent();
        currentOpen = menu;
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        StartCoroutine(menu.window.SlideIn(.2f, .1f));
    }

    private void OpenWindow(ActivateableWindow menu)
    {
        if (!PlayerInLegalState()) return;
        if (currentOpen == menu)
        {
            CloseCurrent();
            return;
        }
        ForceMenu(menu);
    }

    public void CloseCurrent()
    {
        if (currentOpen == null) return;
        Tutorial.CompleteStage(TutorialFor.WORLDDECK, 2);
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
        if (Input.GetKeyDown(KeyCode.Escape)) CloseCurrent();
        if (bugWindow.activeSelf) return;
        foreach (var window in windowControls)
        {
            if(window.keyCode != KeyCode.None && Input.GetKeyDown(window.keyCode))
            {
                OpenWindow(window);
            }
        }
        
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
