using System;
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

    [SerializeField] GameObject tutorialPrompt;


    private void Start()
    {
        PlayerPrefs.SetInt(nameof(TutorialFor.MAIN), 0);
        //if there is no save file, disable the continue button~~~~~~~
        if (!System.IO.File.Exists(Application.persistentDataPath + "/runData.json"))
        {
            continueButton.SetActive(false);
        }
    }
    public void ClickNewGameButton()
    {
        if(PlayerPrefs.GetInt(nameof(TutorialFor.MAIN)) == 0)
            tutorialPrompt.SetActive(true);
        else starter.NewGame();
    }

    public void TutorialPromptNo()
    {
        PlayerPrefs.SetInt(nameof(TutorialFor.MAIN), -1);
        starter.NewGame();
    }

    public void TutorialPromptYes()
    {
        PlayerPrefs.SetInt(nameof(TutorialFor.MAIN), 1);
        foreach (string name in Enum.GetNames(typeof(TutorialFor)))
        {
            PlayerPrefs.SetInt(name, 0);
        }

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
