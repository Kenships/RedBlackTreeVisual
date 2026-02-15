using TMPro;
using UnityEngine;

public class AddNodeUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private RedBlackRenderer rbRenderer;

    public void AddNode()
    {
        rbRenderer.Insert(int.Parse(inputField.text));
    }
}
