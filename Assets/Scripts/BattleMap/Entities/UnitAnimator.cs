using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AnimType { CAST, DAMAGED, WALK, JUMP, DIE }
public class UnitAnimator : MonoBehaviour
{
    Animator animator;
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if(animator == null)
        {
            Debug.LogError($"No animator found in object hierarchy for {gameObject}.");
        }
    }

    Dictionary<AnimType, string> animateFor = new()
    {
        { AnimType.CAST, "Swell" },
        { AnimType.DAMAGED, "TakeDamage_BlendTree"},
        { AnimType.WALK, "Crawl_BlendTree"},
        { AnimType.JUMP, "Jump_BlendTree"},
        { AnimType.DIE, "Death"}
    };
    public void Animate(AnimType anim)
    {
        animator.Play(animateFor[anim]);
    }
}
