using UnityEngine;

public class WebButton : MonoBehaviour
{
    [SerializeField] string url;
    public void Open()
    {
        Application.OpenURL(url);
    }
}
