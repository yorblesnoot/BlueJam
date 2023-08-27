using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyControl : MonoBehaviour
{
    [SerializeField] List<GameObject> hudElements;
    [SerializeField] WorldMenuControl control;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) ToggleHUD();

        if (control == null) return;
        if (Input.GetKeyDown(KeyCode.D)) control.OpenDeckView();
        else if (Input.GetKeyDown(KeyCode.C)) control.OpenCrafting();
        else if (Input.GetKeyDown(KeyCode.Escape)) control.CloseLast();
    }

    private void ToggleHUD()
    {
        foreach (GameObject hudElement in hudElements)
        {
            hudElement.SetActive(!hudElement.activeSelf);
        }
    }
}
