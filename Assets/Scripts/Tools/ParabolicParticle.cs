using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicParticle : MonoBehaviour
{
    public float timeToHit;
    Vector3 targetPosition;
    Vector3 firingPosition;
    float gravity = 10f;

    float targetX;
    float targetY;

    public GameObject target;
    public ParticleSystem projectile;
    public ParticleSystem.MainModule mainModule;
    private void Start()
    {
        mainModule = projectile.main;
        gravity = projectile.main.gravityModifierMultiplier * 10;
    }
    void GetParabola()
    {
        float verticalVelocity = targetY/timeToHit + gravity * timeToHit;
        float horizontalVelocity = targetX / timeToHit / 2;
        float combinedVelocity = Mathf.Sqrt(Mathf.Pow(horizontalVelocity, 2) +  Mathf.Pow(verticalVelocity, 2));
        float compAngle = Mathf.Atan(verticalVelocity / horizontalVelocity);
        compAngle = Mathf.Rad2Deg*compAngle;
        Quaternion firingAngle = Quaternion.Euler(-compAngle, 0, 0);
        transform.rotation = transform.rotation * firingAngle;
        mainModule.startSpeed = combinedVelocity;
    }

    private void Update()
    {
        Vector3 localCoords = target.transform.position - transform.position;
        targetX = localCoords.x;
        targetY = localCoords.y;

        Vector3 lookPosition = new(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.LookAt(lookPosition);

        GetParabola();
    }
}
