using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    BUTTONPRESS,
    SLIMESTEP,
    GOTCHEST,
    GOTHEART,
    GOTBOMB,
    INVENTORYGRAB,
    INVENTORYDROP,
    CRAFTCONFIRMED,
    CARDREMOVED,

    CARDPLAYED,
    CARDDEALT,
    HITPHYSICAL,
    HITMAGICAL,

    MUSICMENUS,
    MUSICWORLD,
    MUSICBATTLE,
    MUSICBOSS,
    MUSICVICTORY,
    MUSICGAMEOVER,

    MUSICBATTLE2,
    MUSICBATTLE3,

    MUSICWORLD2,
    MUSICWORLD3,

    MUSICBOSS2

}

public enum SoundTypeEffect
{
    NONE,
    BARRIER,
    BLINK,
    BLOODPRICE,
    HEAL,
    INJECT,
    PUSH,
    PULL,
    STATUP,
    STATDOWN,
    SUMMON,
    WALK,
    DIE,
    ATTACKBLUNT,
    ATTACKSHARP,
    ATTACKEXPLOSIVE,
    ATTACKWET,
    ATTACKMYSTICAL
}

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "ScriptableObjects/Singletons/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    [SerializeField] List<AssignedClip> assignedClips;
    [SerializeField] List<AssignedEffectClip> assignedEffectClips;

    public Dictionary<SoundType, AudioClip> Cliptionary;
    public Dictionary<SoundTypeEffect, AudioClip> EffectCliptionary;

    public void Initialize()
    {
        Cliptionary = new();
        EffectCliptionary = new();
        foreach (var cl in assignedClips)
        {
            Cliptionary.Add(cl.soundType, cl.clip);
        }
        foreach (var cl in assignedEffectClips)
        {
            EffectCliptionary.Add(cl.soundType, cl.clip);
        }
    }

}

[System.Serializable]
class AssignedClip
{
    public SoundType soundType;
    public AudioClip clip;
}

[System.Serializable]
class AssignedEffectClip
{
    public SoundTypeEffect soundType;
    public AudioClip clip;
}
