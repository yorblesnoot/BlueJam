using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuButtonControl : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject menuButton;

    [SerializeField] GameObject options;
    [SerializeField] GameObject optionsButton;

    public void MenuButtonClicked()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        menu.SetActive(!menu.activeSelf);
    }

    public void MainMenuLinkClicked()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        SceneManager.LoadScene(0);
    }

    public void OptionButtonClicked()
    {
        options.SetActive(true);
    }
}
