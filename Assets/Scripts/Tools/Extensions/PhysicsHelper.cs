using System.Collections;
using UnityEngine;

public static class PhysicsHelper
{
    public static IEnumerator LerpTo(this GameObject thing, Vector3 endPosition, float duration)
    {
        UnityEngine.Transform transform = thing.transform;
        float timeElapsed = 0;
        Vector3 startPosition = transform.position;
        while (timeElapsed < duration)
        {
            Vector3 step = Vector3.Lerp(startPosition, endPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            transform.position = step;
            yield return null;
        }
        transform.position = endPosition;
    }

    public static Quaternion RandomCardinalRotate()
    {
        int random = Random.Range(0, 4);
        Quaternion rot = Quaternion.Euler(0, random * 90, 0);
        return rot;
    }
}
