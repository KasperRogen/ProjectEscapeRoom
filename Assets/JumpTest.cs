using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class JumpTest : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position += new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && base.hasAuthority && base.isClient)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
    }
}
