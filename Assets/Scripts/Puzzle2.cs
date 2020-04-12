using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle2 : MonoBehaviour
{

    public GameObject Light;
    public List<GameObject> Texts;

    public void Toggle()
    {
        Light.SetActive(true);
        Texts.ForEach(text => text.SetActive(true));
    }
    // Start is called before the first frame update
    void Start()
    {
        Light.SetActive(false);
        Texts.ForEach(text => text.SetActive(false));
    }

    
    void Update()
    {
        
    }
}
