using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float panSpeed;
    [SerializeField] float panBorderThickness;
    [SerializeField] float panLowerLimit;
    float panUpperLimit;
    [SerializeField] RunData runData;

    [SerializeField] SceneRelay sceneRelay;

    void Awake()
    {
        EventManager.prepareForBattle.AddListener(() => sceneRelay.cameraPosition = transform.position);
        if (sceneRelay.cameraPosition != new Vector3())
        {
            transform.position = sceneRelay.cameraPosition;
        }
        try
        {
            panUpperLimit = panLowerLimit + runData.worldMap.GetLength(0) / 2;
        }
        catch
        {
            panUpperLimit = panLowerLimit + 10;
        }
    }
    void Update()
    {
        Vector3 pos = transform.position;
        if(Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
            pos.x += panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
            pos.x -= panSpeed * Time.deltaTime;
        }

        pos.z = Mathf.Clamp(pos.z, panLowerLimit, panUpperLimit);
        pos.x = Mathf.Clamp(pos.x, panLowerLimit, panUpperLimit);

        transform.position = pos;
    }
}
