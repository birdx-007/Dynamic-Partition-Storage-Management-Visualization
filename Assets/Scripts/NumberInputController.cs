using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberInputController : MonoBehaviour
{
    public InputField inputField;
    public MemoryController memoryController;
    public int number;
    public void UpdateNumber()
    {
        int.TryParse(inputField.text,out number);
    }
    public void OnNumberButtonPressed(Text buttonText)
    {
        if (inputField.text.Length > 0 || buttonText.text != "0")
        {
            inputField.text += buttonText.text;
            UpdateNumber();
        }
    }
    public void OnBackspaceButtonPressed()
    {
        if(inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Remove(inputField.text.Length - 1);
            UpdateNumber();
        }
    }

    public void OnSubmitButtonPressed()
    {
        memoryController.TryAllocate(number);
        inputField.text = "";
    }
}
