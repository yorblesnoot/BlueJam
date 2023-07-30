using UnityEngine.SceneManagement;
using UnityEngine;

public class CreditButtons : MonoBehaviour
{
    [SerializeField] RunData RunData;
    public void ReturnToMain()
    {
        SceneManager.LoadScene(0);
    }

    public void ContinueRun()
    {
        RunData.endless = true;
        SceneManager.LoadScene(1);
    }
}
