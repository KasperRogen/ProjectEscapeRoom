using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour
{
    public UnityEvent OnInteract;

    [SerializeField]
    bool allowsPickup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void Interact()
    {
        Debug.Log("Interacting");
        OnInteract.Invoke();
    }

    internal bool AllowsPickup()
    {
        return allowsPickup;
    }
    
}
