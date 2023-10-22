using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;


public enum AnimType { CAST, DAMAGED, WALK, JUMP, DIE, CHEER, ATTACKCLOSE, ATTACKFAR }
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
        { AnimType.CHEER, "Cheer" },
        { AnimType.ATTACKCLOSE, "Attack_02" },
        { AnimType.ATTACKFAR, "Attack_03"}
    };
    public void Animate(AnimType anim, GameObject source = null)
    {
        if (anim == AnimType.WALK) animator.SetFloat("xVelocity", 1f);
        else if (anim == AnimType.DAMAGED && source != null)
        {
            Vector3 direction = (source.transform.position - transform.position).normalized;
            direction = transform.rotation * direction;
            animator.SetFloat("damageX", direction.x);
            animator.SetFloat("damageY", direction.z);
        }
        try
        {
            animator.Play(animateFor[anim]);
        }
        catch { Debug.LogWarning($"Animation {anim} not found on animator."); }
    }
}
