using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    public AsyncOperation loading;
    [SerializeField] Slider loadBar;
    [SerializeField] float animationSpeedMult;
    public void LoadSceneWithScreen(int scene)
    {
        loading = SceneManager.LoadSceneAsync(scene);
        loading.allowSceneActivation = false;
    }

    
    private void Update()
    {
        if (loading != null)
        {
            loadBar.value = loading.progress;
            //loadBar.value = Mathf.MoveTowards(loadBar.value, loading.progress, animationSpeedMult * Time.deltaTime);
        }
    }


}
