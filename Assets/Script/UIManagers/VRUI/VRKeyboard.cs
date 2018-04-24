using UnityEngine;
using UnityEngine.UI;

public class VRKeyboard : MonoBehaviour
{
    public InputField input;

    public void ClickKey(string character)
    {
        input.text += character;
    }

    public void Backspace()
    {
        if (input.text.Length > 0)
        {
            input.text = input.text.Substring(0, input.text.Length - 1);
        }
    }

    public void Enter()
    {
        input.text = "";
    }
}