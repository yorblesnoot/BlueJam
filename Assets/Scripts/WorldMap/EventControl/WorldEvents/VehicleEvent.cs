using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleEvent : WorldEvent
{
    [SerializeField] GameObject attachmentPoint;
    [SerializeField] EventType vehicleEvent;
    public List<TerrainType> compatibleTerrains;
    public override void Activate(WorldEventHandler eventHandler)
    {
        WorldPlayerControl.CurrentVehicle = this;
        eventHandler.eventComplete = true;
        RunTutorials();
    }

    public void CheckForDismount(Vector2Int newPosition)
    {
        if (!compatibleTerrains.Contains(runData.worldMap[newPosition.x, newPosition.y]) 
            || (runData.eventMap.TryGetValue(newPosition, out EventType cellEvent) && cellEvent == vehicleEvent))
        {
            transform.SetParent(null, true);
            WorldPlayerControl.CurrentVehicle = null;
            RegisterWithCell();
            StartCoroutine(HopOffVehicle());
        }
    }

    public void RelocateVehicle(Vector2Int newPosition, Vector2Int oldPosition)
    {
        WorldEventRenderer.MoveEvent(newPosition, oldPosition, vehicleEvent, gameObject);
    }

    public override void PreAnimate()
    {
        StartCoroutine(HopToVehicle());
    }
    readonly float sitDuration = .4f;
    IEnumerator HopToVehicle()
    {
        WorldPlayerControl.player.GetComponent<SlimeAnimator>().Animate(AnimType.JUMP);
        float timeElapsed = 0;
        Quaternion targetRotation = WorldPlayerControl.player.playerVisual.transform.rotation;
        Quaternion startRotiation = transform.rotation;
        while (timeElapsed < WorldPlayerControl.moveTime)
        {
            transform.rotation = Quaternion.Lerp(startRotiation, targetRotation, timeElapsed / WorldPlayerControl.moveTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
        transform.SetParent(WorldPlayerControl.player.playerVisual.transform, true);
        timeElapsed = 0;
        Transform playerModel = WorldPlayerControl.player.playerModel.transform;
        attachmentPoint.transform.SetParent(WorldPlayerControl.player.playerVisual.transform, true);
        Vector3 startPosition = playerModel.localPosition;
        Vector3 startScale = playerModel.localScale;
        while (timeElapsed < sitDuration)
        {
            playerModel.localPosition = Vector3.Lerp(startPosition, attachmentPoint.transform.localPosition, timeElapsed / sitDuration);
            playerModel.localScale = Vector3.Lerp(startScale, attachmentPoint.transform.localScale, timeElapsed / sitDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        playerModel.localPosition = attachmentPoint.transform.localPosition;
        playerModel.localScale = attachmentPoint.transform.localScale;
    }

    IEnumerator HopOffVehicle()
    {
        WorldPlayerControl.player.GetComponent<SlimeAnimator>().Animate(AnimType.JUMP);
        float timeElapsed = 0;
        Transform playerModel = WorldPlayerControl.player.playerModel.transform;
        attachmentPoint.transform.SetParent(transform, true);
        playerModel.SetParent(WorldPlayerControl.player.playerVisual.transform, true);
        Vector3 startPosition = playerModel.localPosition;
        Vector3 startScale = playerModel.localScale;
        
        while (timeElapsed < sitDuration)
        {
            playerModel.localPosition = Vector3.Lerp(startPosition, Vector3.zero, timeElapsed / sitDuration);
            playerModel.localScale = Vector3.Lerp(startScale, WorldPlayerControl.playerBaseScale, timeElapsed / sitDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        playerModel.localPosition = Vector3.zero;
        playerModel.localScale = WorldPlayerControl.playerBaseScale;
    }
}
