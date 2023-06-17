using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    public RunData RunData;

    public void PressSave()
    {
        SaveContainer container = new SaveContainer(RunData);
        container.SaveGame();
    }
}
