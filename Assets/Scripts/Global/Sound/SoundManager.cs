using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public static SoundLibrary Library;
    public static AudioSource FXSource;
    public static AudioSource MusicSource;

    public static void PlaySound(SoundType type)
    {
        SourceCheck();
        FXSource.volume = Settings.Player[PlayerSetting.fx_volume] * Settings.Player[PlayerSetting.master_volume] * Settings.Dev.FXVolumeMod;
        if (Library.Cliptionary.TryGetValue(type, out AudioClip clip))
            FXSource.PlayOneShot(clip);
    }

    public static void PlayMusic(SoundType type)
    {
        SourceCheck();
        MusicSource.volume = Settings.Player[PlayerSetting.music_volume] * Settings.Player[PlayerSetting.master_volume] * Settings.Dev.MusicVolumeMod;
        if (!Library.Cliptionary.TryGetValue(type, out AudioClip clip)) return;
        MusicSource.clip = clip;
        MusicSource.loop = true;
        MusicSource.Play();
    }

    static void SourceCheck()
    {
        if (FXSource == null)
        {
            GameObject speaker = new("Speaker");
            FXSource = speaker.AddComponent<AudioSource>();
        }
        if (MusicSource == null)
        {
            GameObject speaker = new("Speaker");
            MusicSource = speaker.AddComponent<AudioSource>();
        }
    }

    public static void UpdateVolume()
    {
        MusicSource.volume = Settings.Player[PlayerSetting.music_volume] * Settings.Player[PlayerSetting.master_volume] * Settings.Dev.MusicVolumeMod;
        FXSource.volume = Settings.Player[PlayerSetting.fx_volume] * Settings.Player[PlayerSetting.master_volume] * Settings.Dev.FXVolumeMod;
    }
}
