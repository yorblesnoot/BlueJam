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

    public static void ParabolicProjectile(this GameObject particleHolder, Vector3 targetPosition, float timeToHit)
    {
        Transform transform = particleHolder.transform;
        ParticleSystem projectile = particleHolder.GetComponentInChildren<ParticleSystem>();
        ParticleSystem.MainModule mainModule = projectile.main;
        float gravity = projectile.main.gravityModifierMultiplier * 9.8f;

        Vector3 localCoords = targetPosition - transform.position;
        Vector2 flatLocal = new(localCoords.x, localCoords.z);
        float horizontalDistance = flatLocal.magnitude;
        float verticalDistance = localCoords.y;

        Vector3 lookPosition = new(targetPosition.x, transform.position.y, targetPosition.z);
        transform.LookAt(lookPosition);

        float verticalVelocity = verticalDistance / timeToHit + gravity * timeToHit / 2;
        float horizontalVelocity = horizontalDistance / timeToHit;
        float combinedVelocity = Mathf.Sqrt(Mathf.Pow(horizontalVelocity, 2) + Mathf.Pow(verticalVelocity, 2));
        float compAngle = Mathf.Atan(verticalVelocity / horizontalVelocity);
        compAngle = Mathf.Rad2Deg * compAngle;
        Quaternion firingAngle = Quaternion.Euler(-Mathf.Abs(compAngle), 0, 0);
        transform.rotation = transform.rotation * firingAngle;
        mainModule.startSpeed = combinedVelocity;
    }
}
