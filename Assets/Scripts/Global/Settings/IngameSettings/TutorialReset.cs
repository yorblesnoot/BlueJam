using UnityEngine;

public class TutorialReset : MonoBehaviour
{
    public void ResetTutorials()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        PlayerPrefs.SetInt(nameof(TutorialFor.MAIN), 0);
    }
}
