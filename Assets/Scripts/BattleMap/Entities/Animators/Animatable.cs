using System.Collections;
using UnityEngine;

public abstract class Animatable : MonoBehaviour
{
    public abstract void Animate(AnimType anim, GameObject source = null);

    public abstract IEnumerator EndWalk(float moveLength);
}
