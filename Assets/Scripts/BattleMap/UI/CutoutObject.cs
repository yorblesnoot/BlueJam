using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField] Transform targetObject;
    [SerializeField] LayerMask wallMask;

    [SerializeField] Camera mainCam;

    private void Awake()
    {
        
    }

    private void Update()
    {
        Vector2 cutoutPos = mainCam.WorldToViewportPoint(targetObject.position);
        //cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObject.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

        for(int i = 0; i < hitObjects.Length; i++)
        {
            Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;
            Debug.Log(cutoutPos);
            for(int m = 0; m < materials.Length; m++)
            {
                materials[m].SetVector("_CutoutPosition", cutoutPos);
                materials[m].SetFloat("_CutoutSize", .1f);
                materials[m].SetFloat("_FalloffSize", .05f);
            }
        }

    }
}
