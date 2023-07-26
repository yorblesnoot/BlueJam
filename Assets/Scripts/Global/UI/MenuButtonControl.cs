using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuButtonControl : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject optionsButton;
    [SerializeField] GameObject menuButton;

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
}
