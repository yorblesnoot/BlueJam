using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] RunStarter starter;
    [SerializeField] RunData runData;
    [SerializeField] LoadLibrary loadLibrary;

    [SerializeField] GameObject continueButton;


    private void Start()
    {
        //if there is no save file, disable the continue button~~~~~~~
        if (!System.IO.File.Exists(Application.persistentDataPath + "/runData.json"))
        {
            continueButton.SetActive(false);
        }
    }
    public void ClickPlayButton()
    {
        starter.NewGame();
    }

    public void ClickContinueButton()
    {
        SaveContainer saver = new(runData, loadLibrary);
        saver.LoadGame();
    }
    
    public void ClickExitButton()
    {
        Application.Quit();
    }
}
