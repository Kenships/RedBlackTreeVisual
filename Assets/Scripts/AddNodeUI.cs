using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class AddNodeUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private RedBlackRenderer rbRenderer;

    public void AddNode()
    {
        if (!int.TryParse(inputField.text, out int value))
            return;
        inputField.text = "";
        inputField.ActivateInputField();
        rbRenderer.Insert(value);
    }

    public void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            AddNode();
        }
    }
}
