using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardProjector : MonoBehaviour
{
    [SerializeField] BtlCardDisplay _projectedCard;
    static BtlCardDisplay projectedCard;

    [SerializeField] Canvas _mainCanvas;
    static Canvas mainCanvas;
    void Awake()
    {
        projectedCard = _projectedCard;
        mainCanvas = _mainCanvas;
    }

    public static void ProjectCardFromWorld(CardPlus card, BattleUnit owner, Vector3 worldLocation, Vector3 canvasDisplacement)
    {
        Vector3 canvasPosition = worldLocation.WorldToCanvasPosition(mainCanvas, Camera.main);
        canvasPosition += canvasDisplacement;
        ProjectCardFromCanvas(card, owner, canvasPosition);
    }

    public static void ProjectCardFromCanvas(CardPlus card, BattleUnit owner, Vector3 canvasPosition)
    {
        projectedCard.gameObject.SetActive(true);
        projectedCard.PopulateCard(card, owner);
        projectedCard.transform.position = canvasPosition;
    }

    public static void HideProjectedCard()
    {
        projectedCard.gameObject.SetActive(false);
    }
}
