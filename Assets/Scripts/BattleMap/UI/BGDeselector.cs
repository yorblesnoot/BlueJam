using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BGDeselector : MonoBehaviour
{
    void OnMouseDown()
    {
        EventManager.clearActivation?.Invoke();
    }
}
