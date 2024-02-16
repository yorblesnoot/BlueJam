using UnityEngine;

public class FlyAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    public void Walk(bool walk = true)
    {
        animator.SetBool("walking", walk);
    }

    public void Attack()
    {
        animator.Play("Attack");
    }

    public void Die()
    {
        animator.SetBool("walking", false);
        animator.Play("Die");
    }
}
