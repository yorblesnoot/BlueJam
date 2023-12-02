using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleAnimator : Animatable
{
    [SerializeField] Animation animator;

    [System.Serializable]
    class TypeToClip
    {
        public AnimType type;
        public string clip;
    }
    [SerializeField] List<TypeToClip> typeToClipList;

    Dictionary<AnimType, string> typeToClipDict;
    private void Awake()
    {
        typeToClipDict = new();
        foreach(var equivalence in typeToClipList)
        {
            typeToClipDict.Add(equivalence.type, equivalence.clip);
        }
    }
    public override void Animate(AnimType anim, GameObject source = null)
    {
        animator.CrossFade(typeToClipDict[anim], .2f);
    }

    public override IEnumerator EndWalk(float moveLength)
    {
        yield break;
    }
}
