using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMenuControl : MonoBehaviour
{
    public GameObject essenceCrafting;
    public Button openCrafting;
    public Button close;
    public void OpenCrafting()
    {
        //toggle relevant buttons
        essenceCrafting.SetActive(true);
        openCrafting.gameObject.SetActive(false);
        close.gameObject.SetActive(true);
    }
    public void CloseCrafting()
    {
        //toggle relevant buttons
        essenceCrafting.SetActive(false);
        openCrafting.gameObject.SetActive(true);
        close.gameObject.SetActive(false);
    }

}
