using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AnimType { CAST, DAMAGED, WALK, JUMP, DIE, CHEER, ATTACKCLOSE, ATTACKFAR, IDLE }
public class SlimeAnimator : Animatable
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
        { AnimType.ATTACKFAR, "Attack_03" },
        { AnimType.IDLE, "Idle_02" }
    };

    readonly string yVelocity = "yVelocity";
    readonly string secondIdle = "idle02";
    readonly string damageX = "damageX";
    readonly string damageY = "damageY";
    public override void Animate(AnimType anim, GameObject source = null)
    {
        if (animator == null) return;
        //animator.SetBool("idle02", false);
        if (anim == AnimType.WALK) { StartCoroutine(LerpVelocity(0, 1)); }
        else if (anim == AnimType.DAMAGED)
        {
            TakeDamage(source);
        }
        else animator.Play(animateFor[anim]);
    }

    void TakeDamage(GameObject source)
    {
        if(source != null)
        {
            Vector3 direction = (source.transform.position - transform.position).normalized;
            direction = transform.rotation * direction;
            animator.SetFloat(damageX, direction.x);
            animator.SetFloat(damageY, direction.z);
        }
        animator.Play(animateFor[AnimType.DAMAGED]);
    }

    readonly public static float accelTime = .24f;
    public override IEnumerator EndWalk(float moveLength)
    {
        if(animator == null || animator.GetFloat(yVelocity) == 0) yield break;
        yield return new WaitForSeconds(moveLength - accelTime);

        
        StartCoroutine(LerpVelocity(1, 0));
    }

    IEnumerator LerpVelocity(float start, float end)
    {
        float timeElapsed = 0;
        while (timeElapsed < accelTime)
        {
            timeElapsed += Time.deltaTime;
            animator.SetFloat(yVelocity, Mathf.Lerp(start, end, timeElapsed / accelTime));
            yield return null;
        }
    }
}
