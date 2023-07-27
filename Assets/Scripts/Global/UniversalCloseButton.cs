using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalCloseButton : MonoBehaviour
{
    [SerializeField] GameObject windowToClose;
    public void CloseButton()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        windowToClose.SetActive(false);
    }
}
