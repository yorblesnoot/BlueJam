using UnityEngine;
using UnityEngine.UI;

public class HatControl
{
    GameObject currentHat;
    Image image;

    public HatControl(Image _image)
    {
        image = _image;
    }

    readonly static int hatTiltAngle = -10;
    readonly static int hatProjectionDistance = 30;
    public void DeployHat(GameObject newHat)
    {
        if (currentHat != null) GameObject.Destroy(currentHat);
        currentHat = GameObject.Instantiate(newHat);
        currentHat.transform.SetParent(image.transform, false);

        Transform scalingCube = currentHat.transform.GetChild(0);
        scalingCube.transform.SetParent(image.transform, true);
        currentHat.transform.SetParent(scalingCube, true);

        scalingCube.transform.localScale = Vector3.one * 50;
        Vector3 position = new(0, 0, -hatProjectionDistance);
        scalingCube.transform.localPosition = position;
        scalingCube.transform.localRotation = Quaternion.identity;
        currentHat.transform.localRotation = Quaternion.Euler(hatTiltAngle, 0, 0);
        Renderer[] hatRens = currentHat.GetComponentsInChildren<Renderer>();
        foreach (Renderer ren in hatRens)
        {
            ren.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
