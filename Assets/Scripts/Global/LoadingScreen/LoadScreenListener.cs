using UnityEngine;

public class LoadScreenListener : MonoBehaviour
{
    [SerializeField] Loader loader;
    private void Awake()
    {
        EventManager.loadSceneWithScreen.AddListener(ActivateLoadingScreen);
    }

    private void ActivateLoadingScreen(int scene)
    {
        if(scene == -1)
        {
            loader.loading.allowSceneActivation = true;
            return;
        }
        loader.gameObject.SetActive(true);
        loader.LoadSceneWithScreen(scene);
    }
}
