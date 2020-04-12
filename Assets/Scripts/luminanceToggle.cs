using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;

public class luminanceToggle : MonoBehaviour
{
    // Start is called before the first frame update
    public UnityEvent OnSuccess;

    public TextMeshPro text;
    public TextMeshPro hiddenText;
    public GameObject light;

    bool Puzzle2Unlocked;
    
    float accumulationRate = 0.3f;
    float fadeRate = 0.6f;
    bool on;
    float AccumulatedLight;

    int blinkCount;
    float onTime;
    float offTime;
    void Start()
    {

        text.gameObject.SetActive(on);
        hiddenText.gameObject.SetActive(on);
        light.SetActive(on);
    }

    public void Toggle()
    {
        on = !on;

        text.gameObject.SetActive(on);
        light.SetActive(on);

        hiddenText.gameObject.SetActive(!on);


        if (on)
        {
            if(offTime >= 1f)
            {
                blinkCount = 0;
            }

            onTime = 0f;
            offTime = 0f;
        }
        else
        {
            if(onTime <= 1f)
            {
                blinkCount++;
            } else
            {
                blinkCount = 0;
            }

            offTime = 0f;
            onTime = 0f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        AccumulatedLight = Mathf.Clamp(AccumulatedLight, 0, 1);

        if (on)
        {
            AccumulatedLight += accumulationRate * Time.deltaTime;
        } else
        {
            Color faceColor = hiddenText.faceColor;
            faceColor.a = AccumulatedLight;
            hiddenText.faceColor = faceColor;
            AccumulatedLight -= fadeRate * Time.deltaTime;
        }

        if (on)
        {
            onTime += Time.deltaTime;
        } else
        {
            offTime += Time.deltaTime;

            if(Puzzle2Unlocked == false && blinkCount == 3 && offTime >= 3f)
            {
                text.gameObject.SetActive(true);
                hiddenText.gameObject.SetActive(false);
                light.SetActive(false);
                Debug.Log("Puzzle 1 solved");
                OnSuccess.Invoke();
                Puzzle2Unlocked = true;
            }
        }
    }
}
