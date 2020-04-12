using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeypadButton : MonoBehaviour
{
    public KeypadScript keypad;
    Renderer renderer;
    TextMeshPro textMesh;

    public Color defaultColor;
    public Color hoverColor;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        textMesh = GetComponentInChildren<TextMeshPro>();
    }

    public void Press()
    {
        keypad.AddInput(textMesh.text);
    }

}
