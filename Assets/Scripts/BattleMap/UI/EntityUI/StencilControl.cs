using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StencilControl : MonoBehaviour
{
    //if this isn't working, check to make sure the stencil shaders are on the blocking objects
    [SerializeField] GameObject stencil;
    public void ToggleStencil(BattleTileController tile)
    {
        if(tile.activateUnitStencil)
            stencil.SetActive(true);
        else stencil.SetActive(false);
    }
}
