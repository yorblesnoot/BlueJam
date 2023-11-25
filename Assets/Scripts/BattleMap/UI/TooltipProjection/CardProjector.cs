using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardProjector : MonoBehaviour
{
    [SerializeField] BtlCardDisplay _projectedCard;
    static BtlCardDisplay projectedCard;
    static RectTransform cardRect;

    [SerializeField] Canvas _mainCanvas;
    static Canvas mainCanvas;
    void Awake()
    {
        projectedCard = _projectedCard;
        mainCanvas = _mainCanvas;
        cardRect = projectedCard.GetComponent<RectTransform>();
    }

    public static void ProjectCard(CardPlus card, BattleUnit owner, Vector3 worldLocation)
    {
        projectedCard.gameObject.SetActive(true);
        projectedCard.PopulateCard(card, owner);
        Vector3 canvasPosition = worldLocation.WorldToCanvasPosition(mainCanvas, Camera.main);
        canvasPosition.x += cardRect.rect.width / 2;
        projectedCard.transform.position = canvasPosition;
    }

    public static void HideProjectedCard()
    {
        projectedCard.gameObject.SetActive(false);
    }
}
