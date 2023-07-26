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
    MUSICVICTORY

}

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "ScriptableObjects/Singletons/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    [SerializeField] List<AssignedClip> assignedClips;

    public Dictionary<SoundType, AudioClip> Cliptionary;

    public void Initialize()
    {
        Cliptionary = new();
        foreach (var cl in assignedClips)
        {
            Cliptionary.Add(cl.soundType, cl.clip);
        }
    }

}

[System.Serializable]
class AssignedClip
{
    public SoundType soundType;
    public AudioClip clip;
}
