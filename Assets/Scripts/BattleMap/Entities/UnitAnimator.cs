using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AnimType { CAST, DAMAGED, WALK, JUMP, DIE, CHEER }
public class UnitAnimator : MonoBehaviour
{
    Animator animator;
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    Dictionary<AnimType, string> animateFor = new()
    {
        { AnimType.CAST, "Swell" },
        { AnimType.DAMAGED, "TakeDamage_BlendTree"},
        { AnimType.WALK, "Crawl_BlendTree"},
        { AnimType.JUMP, "Jump_BlendTree"},
        { AnimType.DIE, "Death"},
        { AnimType.CHEER, "Cheer" }
    };
    public void Animate(AnimType anim)
    {
        try
        {
            animator.Play(animateFor[anim]);
        }
        catch { }
    }
}
