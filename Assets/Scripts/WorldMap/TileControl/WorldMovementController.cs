using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldMovementController : MonoBehaviour
{
    bool availableMove;

    //biome specific procedural generation data

    public Vector3 unitPosition;

    [SerializeField] GameObject mySelector;

    [SerializeField] WorldEventHandler eventHandler;

    GameObject player;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        float heightAdjust = 1.3f;
        Vector3 myPosition = gameObject.transform.position;
        unitPosition = new Vector3(myPosition.x, myPosition.y + heightAdjust, myPosition.z);
        EventManager.getWorldDestination.AddListener(HighlightCell);
        EventManager.clearWorldDestination.AddListener(ClearHighlight);
    }


    public void OnMouseDown()
    {
        if (availableMove == true && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray camray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitPoint;
            if (Physics.Raycast(camray, out hitPoint))
            {
                if (hitPoint.collider.gameObject == this.gameObject)
                {
                    //move the player to the cell
                    StartCoroutine(player.GetComponent<WorldPlayerControl>().MoveToWorldCell(gameObject));

                    //deactivate targeting on unpicked cells
                    EventManager.clearWorldDestination?.Invoke();
                }
            }
        }
    }

    public void ClearHighlight()
    {
        availableMove = false;
        mySelector.SetActive(false);
    }

    public void HighlightCell(int playerX, int playerY)
    {
        int[] coords = GridTools.VectorToMap(gameObject.transform.position);
        //check that we are exactly one cell away from the player
        int xOffset = playerX - coords[0];
        int yOffset = playerY - coords[1];
        if ((xOffset == 0 && Mathf.Abs(yOffset) == 1) || (yOffset == 0 && Mathf.Abs(xOffset) == 1))
        {
            mySelector.SetActive(true);
            availableMove = true;
        }
    }
}
