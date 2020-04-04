using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Range(-1, 1)]
    public int RotationDir = 1;

    public float ClosedAngle;
    public float OpenAngle;

    // Start is called before the first frame update
    void Start()
    {
        ClosedAngle = transform.localRotation.eulerAngles.y;
    }

    public void Open()
    {
        Debug.Log("OPENING " + transform.gameObject.name);
        StartCoroutine(OpenProcess());
    }

    IEnumerator OpenProcess()
    {
        OpenAngle = OpenAngle < 0 ? 360 + OpenAngle : OpenAngle;
        
        float rotDiff = Mathf.Abs(transform.localEulerAngles.y - OpenAngle);

        while (rotDiff > 5)
        {
            rotDiff = Mathf.Abs(transform.localEulerAngles.y - OpenAngle);
            transform.Rotate(new Vector3(0, 10 * RotationDir * Time.deltaTime, 0));
            yield return new WaitForEndOfFrame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
