using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialQuit : MonoBehaviour
{
    [SerializeField] private GameObject tutorial;
    
    private void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            tutorial.SetActive(false);
        }
    }
}
