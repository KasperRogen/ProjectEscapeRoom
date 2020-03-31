using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : NetworkBehaviour
{
    CharacterController cc;

    public float speed = 12f;
    public float CrouchHeight = 0.25f;
    public float MouseSensitivity = 100f;


    public Transform GroundCheck;
    public float GroundDistance;
    public LayerMask GroundMask;

    public Transform camPos;
    bool isGrounded;
    bool isCrouched;

    Transform cam;

    float xRotation = 0f;

    public Vector3 velocity;

    public float Gravity = -9.81f;

    public float JumpHeight;


    // Start is called before the first frame update
    void Start()
    {
        if (base.hasAuthority && base.isClient)
        {
            cc = GetComponent<CharacterController>();

            if (base.isLocalPlayer)
            {
                cam = Camera.main.transform;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (base.hasAuthority && base.isClient)
        {
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
        bool crouchRequest = Input.GetKey(KeyCode.LeftControl);

        if (crouchRequest && isCrouched == false)
        {
            isCrouched = true;
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * CrouchHeight, transform.localScale.z);
            cc.height = cc.height * CrouchHeight;
            cc.radius = cc.height;
            transform.position = new Vector3(transform.position.x, cc.height / 2, transform.position.z);
        }

        if (crouchRequest == false && isCrouched == true && HasRoomToStand())
        {
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
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }
    }

    bool HasRoomToStand()
    {
        float PlayerWidth = 0.5f;
        //TODO: Make this not ground mask)
        return Physics.SphereCast(
                new Ray(transform.position, transform.up),
                PlayerWidth,
                (cc.height / CrouchHeight) - (PlayerWidth / 2),
                GroundMask) == false
        && Physics.CheckSphere(transform.position + transform.up * (PlayerWidth/2.1f), PlayerWidth) == false;

    }

    private void Fall()
    {
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        } else
        {
            velocity.y += Gravity * Time.deltaTime;
            cc.Move(velocity * 0.5f * Time.deltaTime);
        }
    }

    private void Look()
    {
        Vector2 MouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * MouseSensitivity * Time.deltaTime;

        xRotation -= MouseInput.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


        transform.Rotate(transform.up * MouseInput.x);

        cam.rotation = transform.rotation;
        cam.Rotate(new Vector3(xRotation, 0, 0));
        cam.position = camPos.position;
    }

    private void Move()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 move = transform.right * input.x + transform.forward * input.z;

        cc.Move(move * speed * Time.deltaTime);
    }
}
