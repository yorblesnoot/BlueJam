using UnityEngine;

public class Pulsate : MonoBehaviour
{
    [SerializeField] Vector3 maxScale;
    [SerializeField] float duration;

    bool grow = true;
    float timeElapsed = 0;
    private void FixedUpdate()
    {
        if(grow) transform.localScale = Vector3.LerpUnclamped(Vector3.one, maxScale, timeElapsed / duration);
        else transform.localScale = Vector3.LerpUnclamped(maxScale, Vector3.one, timeElapsed / duration);
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= duration)
        {
            grow = !grow;
            timeElapsed = 0;
        }
    }
}
