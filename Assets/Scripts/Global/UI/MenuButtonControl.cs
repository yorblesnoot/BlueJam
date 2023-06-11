using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuButtonControl : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject optionsButton;
    [SerializeField] GameObject menuButton;

    public void MenuButtonClicked()
    {
        menu.SetActive(!menu.activeSelf);
    }

    public void MainMenuLinkClicked()
    {
        SceneManager.LoadScene(0);
    }
}
