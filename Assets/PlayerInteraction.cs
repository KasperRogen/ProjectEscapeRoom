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
            Lift();
            Rotate();
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
            Ray aimRay = cam.ScreenPointToRay(Input.mousePosition);

            RotateObject();
            
            Vector3 movementVector = (cam.transform.position + cam.transform.forward * currentlyPickedUpGODistance)
                - currentlyPickedUpGO.transform.position;

            Debug.DrawLine(movementVector, currentlyPickedUpGO.transform.position);

            currentlyPickedUpRB.velocity = movementVector * 100 * movementVector.magnitude;


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
            if(Physics.SphereCast(aimRay, SpherecastRadius, out hit, SpherecastDistance, InteractableMask))
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
                    currentlyPickedUpGO.layer = LayerMask.NameToLayer("Ignore Raycast");
                    currentlyPickedUpRB = currentlyPickedUpGO.GetComponent<Rigidbody>();
                    currentlyPickedUpGO.transform.parent = null;
                    PlayerState._instance.IsLifting = true;
                    currentlyPickedUpRB.isKinematic = false;
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
                currentlyPickedUpRB.isKinematic = true;
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
