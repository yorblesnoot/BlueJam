using System.Collections;
using UnityEngine;

public class WorldCompass : MonoBehaviour
{
    public Vector2Int targetPosition;
    [SerializeField] GameObject pointer;
    private void OnEnable()
    {
        EventManager.playerAtWorldLocation.AddListener(PointAtLockedCoordinates);
    }

    private void OnDisable()
    {
        EventManager.playerAtWorldLocation.RemoveListener(PointAtLockedCoordinates);
    }

    public void PointAtLockedCoordinates(Vector2Int playerGlobalCoordinates)
    {
        StartCoroutine(LerpAt(targetPosition, playerGlobalCoordinates));
    }

    private IEnumerator LerpAt(Vector2Int target, Vector2Int playerGlobalLocation)
    {
        Vector3 startLocation = transform.position;
        Vector3 endLocation = MapTools.MapToVector(playerGlobalLocation - WorldMapRenderer.spotlightGlobalOffset, WorldMovementController.heightAdjust);
        Vector3 targetV3 = MapTools.MapToVector(target - WorldMapRenderer.spotlightGlobalOffset, WorldMovementController.heightAdjust);
        

        float proximityToTarget = Vector3.Distance(endLocation, targetV3);
        proximityToTarget = Mathf.Clamp(proximityToTarget, 0.1f, 5f);
        Vector3 pointerStartPosition = pointer.transform.localPosition;
        Vector3 pointerToPosition = pointerStartPosition;
        pointerToPosition.z = proximityToTarget - 2f;
        
        Vector3 direction = targetV3 - endLocation;
        Quaternion startRotation = transform.rotation;
        Quaternion toRotation = Quaternion.LookRotation(direction);

        float timeElapsed = 0;
        while (timeElapsed < WorldPlayerControl.moveTime)
        {
            float lerpValue = timeElapsed / WorldPlayerControl.moveTime;
            pointer.transform.localPosition = Vector3.Lerp(pointerStartPosition, pointerToPosition, lerpValue);
            transform.SetPositionAndRotation(Vector3.Lerp(startLocation, endLocation, lerpValue),
                Quaternion.Slerp(startRotation, toRotation, lerpValue));

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
