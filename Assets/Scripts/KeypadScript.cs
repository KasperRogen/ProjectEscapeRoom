using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class KeypadScript : MonoBehaviour
{
    public UnityEvent OnUnlocked;
    public string Solution;
    public string currentInput;
    int solutionLength;

    public GameObject Display;
    TextMeshPro displayText;
    Renderer displayRenderer;

    public Color DefaultColor;

    bool _locked;

    public void AddInput(string input)
    {
        solutionLength = Solution.Length;

        if (_locked)
            return;

        currentInput += input;
        displayText.text = currentInput;

        if(currentInput.Length >= solutionLength)
        {
            _locked = true;
            StartCoroutine(Checkinput());
        }

    }

    IEnumerator Checkinput()
    {
        yield return new WaitForSeconds(0.5f);

        if(currentInput == Solution)
        {
            displayRenderer.material.SetColor("_BaseColor", Color.green);
            displayRenderer.material.SetColor("_EmissionColor", Color.green);
            OnUnlocked.Invoke();
            yield break;
        } else
        {
            displayRenderer.material.SetColor("_BaseColor", Color.red);
            displayRenderer.material.SetColor("_EmissionColor", Color.red);
            yield return new WaitForSeconds(0.5f);
            displayRenderer.material.SetColor("_BaseColor", DefaultColor);
            displayRenderer.material.SetColor("_EmissionColor", DefaultColor);
        }

        currentInput = "";
        displayText.text = currentInput;
        _locked = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(KeypadButton button in GetComponentsInChildren<KeypadButton>())
        {
            button.keypad = this;
        }

        displayText = Display.GetComponentInChildren<TextMeshPro>();
        displayRenderer = Display.GetComponent<Renderer>();
        displayText.text = currentInput;

        displayRenderer.material.SetColor("_EmissionColor", DefaultColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
