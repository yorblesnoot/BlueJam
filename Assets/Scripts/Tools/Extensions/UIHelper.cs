using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIHelper
{
    public static Vector3 WorldToCanvasPosition(this Vector3 position, Canvas canvas, Camera camera)
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : camera, out Vector2 result);

        return canvas.transform.TransformPoint(result);
    }
}
