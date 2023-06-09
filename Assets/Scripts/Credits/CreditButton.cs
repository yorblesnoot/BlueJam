using UnityEngine.SceneManagement;
using UnityEngine;

public class CreditButton : MonoBehaviour
{
    public void ReturnToMain()
    {
        SceneManager.LoadScene(0);
    }
}
