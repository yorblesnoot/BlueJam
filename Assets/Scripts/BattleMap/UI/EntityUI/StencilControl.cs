using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StencilControl : MonoBehaviour
{
    [SerializeField] GameObject stencil;
    public void ToggleStencil(BattleTileController tile)
    {
        if(tile.activateUnitStencil)
            stencil.SetActive(true);
        else stencil.SetActive(false);
    }
}
