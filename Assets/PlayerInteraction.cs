using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    public float SpherecastRadius = 0.1f;
    public float SpherecastDistance = 1f;

    public LayerMask InteractableMask;
    public LayerMask BoundryMask;

    Camera cam;

    GameObject currentlyPickedUpGO;
    Rigidbody currentlyPickedUpRB;
    float currentlyPickedUpGODistance;
    Vector3 currentlyPickedUpRotation = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(base.hasAuthority && base.isClient) { 
            PickUp();
            Interact();
            Rotate();
        }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 10, 10), "");
    }

    private void FixedUpdate()
    {
        if (base.hasAuthority && base.isClient)
        {
            Lift();
        }
    }

    private void Rotate()
    {
        if (currentlyPickedUpGO == null)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            PlayerState._instance.liftRotation = true;
        } else if (Input.GetMouseButtonUp(1))
        {
            PlayerState._instance.liftRotation = false;
        }

        if (PlayerState._instance.liftRotation)
        {
            //Take input from mouse movement
            Vector2 MouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 100f * Time.deltaTime;

            currentlyPickedUpRotation.x += MouseInput.y;
            currentlyPickedUpRotation.y += MouseInput.x;

            RotateObject();
        }
    }

    private void Lift()
    {
        if(currentlyPickedUpGO != null)
        {
            if(Vector3.Distance(currentlyPickedUpGO.transform.position, transform.position) > 2)
            {
                currentlyPickedUpGO.transform.position = (cam.transform.position + cam.transform.forward * currentlyPickedUpGODistance);
            }

            Vector3 movementVector;
            Ray aimRay = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            float newDist = float.MaxValue;

            if (Physics.SphereCast(aimRay, SpherecastRadius, out hit, SpherecastDistance, BoundryMask))
            {
                newDist = Vector3.Distance(hit.point, cam.transform.position);
            }

            newDist = newDist < currentlyPickedUpGODistance ? newDist : currentlyPickedUpGODistance;

            movementVector = (cam.transform.position + cam.transform.forward * newDist)
                            - currentlyPickedUpGO.transform.position;
            
            RotateObject();

            currentlyPickedUpRB.velocity = movementVector * Mathf.Clamp(100 * movementVector.magnitude, 0, 6);


        }
    }

    private void RotateObject()
    {
        currentlyPickedUpGO.transform.LookAt(transform);
        currentlyPickedUpGO.transform.Rotate(currentlyPickedUpRotation);
    }

    private void Interact()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Ray aimRay = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.SphereCast(aimRay, SpherecastRadius * 0.25f, out hit, SpherecastDistance, InteractableMask))
            {
                hit.transform.GetComponent<Interaction>()?.Interact();
            }


        }
    }

    private void PickUp()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray aimRay = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.SphereCast(aimRay, SpherecastRadius, out hit, SpherecastDistance, InteractableMask)){
                if (hit.transform.GetComponent<Interaction>().AllowsPickup())
                {
                    CmdRequestAuthority(hit.transform.GetComponent<NetworkIdentity>());
                    currentlyPickedUpGO = hit.transform.gameObject;
                    currentlyPickedUpGODistance = Vector3.Distance(hit.transform.position, transform.position);
                    currentlyPickedUpGO.layer = LayerMask.NameToLayer("PickedUp");
                    currentlyPickedUpRB = currentlyPickedUpGO.GetComponent<Rigidbody>();
                    currentlyPickedUpGO.transform.parent = null;
                    currentlyPickedUpRB.isKinematic = false;
                    PlayerState._instance.IsLifting = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if(currentlyPickedUpGO != null)
            {
                currentlyPickedUpGO.layer = LayerMask.NameToLayer("Interactable");
                CmdRemoveAuthority(currentlyPickedUpGO.GetComponent<NetworkIdentity>());
                currentlyPickedUpGO = null;
                currentlyPickedUpGODistance = 0;
                currentlyPickedUpRB = null;
                PlayerState._instance.IsLifting = false;
            }
        }

       
    }
    

    [Command]
    private void CmdRequestAuthority(NetworkIdentity otherId)
    {
        otherId.AssignClientAuthority(base.connectionToClient);
    }

    [Command]
    private void CmdRemoveAuthority(NetworkIdentity otherId)
    {
        otherId.RemoveClientAuthority();
    }

}
