using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : NetworkBehaviour
{
    CharacterController cc;

    [Header("Movement")]
    public float speed = 12f;
    public float JumpHeight = 1;
    public float CrouchHeight = 0.25f;

    [Space(20), Header("Factors")]
    public float Gravity = -9.81f;

    [Space(20), Header("Looking")]
    public float MouseSensitivity = 100f;

    [Space(20), Header("Grounding")]
    public Transform GroundCheck;
    public float GroundDistance;
    public LayerMask GroundMask;


    //Current State
    bool isGrounded;
    bool isCrouched;
    float xRotation = 0f;
    public Vector3 velocity;

    //For looking
    public Transform camPos;
    Transform cam;


    void Start()
    {
        //If the code in run on the autorized player
        if (base.hasAuthority && base.isClient)
        {
            cc = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            if (base.isLocalPlayer)
            {
                //Grab the camera for the player to control
                cam = Camera.main.transform;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (base.hasAuthority && base.isClient)
        {
            //Check if we stand on the ground
            isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

            Fall();
            Crouch();
            Move();
            Look();
            Jump();
        }
            

    }

    private void Crouch()
    {
        //Do the player wish to crouch?
        bool crouchRequest = Input.GetKey(KeyCode.LeftControl);

        //If we wish to crouch, are we aren't already
        if (crouchRequest && isCrouched == false)
        {
            isCrouched = true;

            //Scale the player and character Controller to crouch
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * CrouchHeight, transform.localScale.z);
            cc.height = cc.height * CrouchHeight;
            cc.radius = cc.height;
            transform.position = new Vector3(transform.position.x, cc.height / 2, transform.position.z);
        }

        //If we wish to stand up, and i aren't already AND we have room to stand up
        if (crouchRequest == false && isCrouched == true && HasRoomToStand())
        {
            //scale the player and character controller to stand up
            isCrouched = false;
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / CrouchHeight, transform.localScale.z);
            cc.height = cc.height / CrouchHeight;
            cc.radius = 0.5f;
            transform.position = new Vector3(transform.position.x, cc.height / 2, transform.position.z);
        }
    }

    private void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            //set the player to jump specified height
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }
    }

    bool HasRoomToStand()
    {
        float PlayerWidth = 0.5f;
        //TODO: Make this not ground mask
        return Physics.SphereCast( //See if a sphere the width of the player can be cast upwards without hitting anything
                new Ray(transform.position, transform.up),
                PlayerWidth,
                (cc.height / CrouchHeight) - (PlayerWidth / 2),
                GroundMask) == false
        && Physics.CheckSphere(transform.position + transform.up * (PlayerWidth/2.1f), PlayerWidth) == false;
        //Check the immediate surroundings for space to stand up

    }

    private void Fall()
    {
        //If we are grounded
        if(isGrounded && velocity.y < 0)
        {
            //Lock the velocity to avoid accumulation
            velocity.y = -2f; 
        } else
        { 
            //We aren't moving, accumulate gravity
            velocity.y += Gravity * Time.deltaTime;
            cc.Move(velocity * 0.5f * Time.deltaTime);
        } 
    }

    private void Look()
    {

        if (PlayerState._instance.liftRotation)
            return;

        //Take input from mouse movement
        Vector2 MouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * MouseSensitivity * Time.deltaTime;

        //Store the current camera rotation along x axis
        xRotation -= MouseInput.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Rotate the player body along the y axis
        transform.Rotate(transform.up * MouseInput.x);

        //Rotate the camera along the x axis
        cam.rotation = transform.rotation;
        cam.Rotate(new Vector3(xRotation, 0, 0));
        cam.position = camPos.position;
    }

    private void Move()
    {
        //Store the input for movement
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //create the movement vector
        Vector3 move = transform.right * input.x + transform.forward * input.z;

        //Apply the movement to the player
        cc.Move(move * speed * Time.deltaTime);
    }
}
