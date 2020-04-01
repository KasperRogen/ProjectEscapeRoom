using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Polaroid : MonoBehaviour
{
    public string text;
    public Sprite sprite;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<TextMeshPro>().text = text;
        GetComponentInChildren<SpriteRenderer>().sprite = sprite;
    }

}
