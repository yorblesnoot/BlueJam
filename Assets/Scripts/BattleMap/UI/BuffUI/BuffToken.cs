using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffToken : MonoBehaviour
{
    public TMP_Text stackDisplay;
    [SerializeField] Image myImage;
    public void RenderBuff(Color32 buffColor, int buffStack)
    {
        myImage.color = buffColor;
        SetDuration(buffStack);
    }
    public void SetDuration(int buffStack) { stackDisplay.text = buffStack.ToString(); }
}
