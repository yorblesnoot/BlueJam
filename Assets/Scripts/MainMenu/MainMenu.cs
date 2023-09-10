using System;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] RunStarter starter;
    [SerializeField] RunData runData;
    [SerializeField] LoadLibrary loadLibrary;

    [SerializeField] GameObject continueButton;

    [SerializeField] GameObject tutorialPrompt;

    [SerializeField] GameObject optionsWindow;

    [SerializeField] DifficultySelector difficultySelector;


    private void Start()
    {
        SoundManager.PlayMusic(SoundType.MUSICMENUS);
        if (!System.IO.File.Exists(Application.persistentDataPath + "/runData.json"))
        {
            continueButton.SetActive(false);
        }
    }
    public void NewGameButton()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        if(PlayerPrefs.GetInt(nameof(TutorialFor.MAIN)) == 0)
            tutorialPrompt.SetActive(true);
        else starter.NewGame();
    }

    public void TutorialPromptNo()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        PlayerPrefs.SetInt(nameof(TutorialFor.MAIN), -1);
        starter.NewGame();
    }

    public void TutorialPromptYes()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        PlayerPrefs.SetInt(nameof(TutorialFor.MAIN), 1);
        foreach (string name in Enum.GetNames(typeof(TutorialFor)))
        {
            PlayerPrefs.SetInt(name, 0);
        }
        starter.NewGame();
    }

    public void ContinueButton()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        SaveContainer saver = new(runData, loadLibrary, difficultySelector);
        saver.LoadGame();
    }

    public void OptionsButton()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        optionsWindow.SetActive(true);
    }
    
    public void ExitButton()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        Application.Quit();
    }
}
