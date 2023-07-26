using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public static SoundLibrary Library;
    public static AudioSource Source;

    public static void PlaySound(SoundType type)
    {
        CheckForSource();
        if(Library.Cliptionary.TryGetValue(type, out AudioClip clip))
            Source.PlayOneShot(clip);
    }

    public static void PlayMusic(SoundType type)
    {
        CheckForSource();
        if (!Library.Cliptionary.TryGetValue(type, out AudioClip clip)) return;
        Source.clip = clip;
        Source.loop = true;
        Source.Play();
    }

    static void CheckForSource()
    {
        if (Source == null)
        {
            GameObject speaker = new("Speaker");
            Source = speaker.AddComponent<AudioSource>();
        }
    }
}
