using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsWIndow : MonoBehaviour
{
    public void ResetTutorials()
    {
        PlayerPrefs.SetInt(nameof(TutorialFor.MAIN), 0);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
