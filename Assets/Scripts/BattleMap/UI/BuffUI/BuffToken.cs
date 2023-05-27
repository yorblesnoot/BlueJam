using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffToken : MonoBehaviour
{
    public TMP_Text stackDisplay;
    int stacks;
    Image myImage;
    public void RenderBuff(Color32 buffColor, int buffStack)
    {
        myImage = GetComponent<Image>();
        myImage.color = buffColor;
        stacks = buffStack;
        stackDisplay.text = buffStack.ToString();
    }
    public void DecrementBuff()
    {
        stacks--;
        stackDisplay.text = stacks.ToString();
    }
}
