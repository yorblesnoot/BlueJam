using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPicker : MonoBehaviour
{
    [SerializeField] RunData runData;
    [SerializeField] SceneRelay sceneRelay;
    [SerializeField] SoundType bossMusic;
    [SerializeField] List<SoundType> musicSequence;
    [SerializeField] int musicChangeInterval;
    void Awake()
    {
        if(sceneRelay.bossEncounter == true)
        {
            SoundManager.PlayMusic(bossMusic);
            return;
        }

        int intervalsPassed = runData.ThreatLevel / musicChangeInterval;
        Mathf.Clamp(intervalsPassed, 0, musicSequence.Count - 1);
        SoundManager.PlayMusic(musicSequence[intervalsPassed]);
    }
}
