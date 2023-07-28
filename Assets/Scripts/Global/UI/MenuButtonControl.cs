using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject menuButton;

    [SerializeField] GameObject options;
    [SerializeField] GameObject optionsButton;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //SoundManager.PlaySound(SoundType.BUTTONPRESS);
        menu.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //SoundManager.PlaySound(SoundType.BUTTONPRESS);
        menu.SetActive(false);
    }

    public void MainMenuLinkClicked()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        SceneManager.LoadScene(0);
    }

    public void OptionButtonClicked()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        options.SetActive(true);
    }
}
