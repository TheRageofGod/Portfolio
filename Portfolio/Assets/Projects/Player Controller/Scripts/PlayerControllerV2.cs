 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerV2 : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    public Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool isCrouching = false;
    public float crouchHeight = 0.5f;
    public float crouchSpeed = 3.5f;
    public float crouchTransitionSpeed = 10f;
    private float originalHeight;
    public float crouchCameraOffset = -0.5f;
    private Vector3 cameraStandPosition;
    private Vector3 cameraCrouchPosition;

    public LayerMask wallLayer;
    public float wallJumpForce = 10f;
    private bool isTouchingWall = false;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalHeight = characterController.height;

        // Define camera positions for standing and crouching
        cameraStandPosition = playerCamera.transform.localPosition;
        cameraCrouchPosition = cameraStandPosition + new Vector3(0, crouchCameraOffset, 0);

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : (isCrouching ? crouchSpeed : walkingSpeed)) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : (isCrouching ? crouchSpeed : walkingSpeed)) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else if (Input.GetButton("Jump") && canMove && isTouchingWall)
        {
            moveDirection.y = wallJumpForce;

            // This adds a bit of horizontal force opposite the wall for a more dynamic jump
            if (Physics.Raycast(transform.position, transform.right, 1f, wallLayer))
            {
                moveDirection += -transform.right * wallJumpForce / 2.5f; // Adjust the divisor for stronger/weaker push.
            }
            else if (Physics.Raycast(transform.position, -transform.right, 1f, wallLayer))
            {
                moveDirection += transform.right * wallJumpForce / 2.5f;
            }
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }


        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        if (Input.GetKeyDown(KeyCode.C) && canMove)
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                characterController.height = crouchHeight;
                walkingSpeed = crouchSpeed;
            }
            else
            {
                characterController.height = originalHeight;
                walkingSpeed = 7.5f;
            }
        }
        if (isCrouching)
        {
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, cameraCrouchPosition, crouchTransitionSpeed * Time.deltaTime);
        }
        else
        {
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, cameraStandPosition, crouchTransitionSpeed * Time.deltaTime);
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, 1f, wallLayer) || Physics.Raycast(transform.position, -transform.right, out hit, 1f, wallLayer))
        {
            isTouchingWall = true;
        }
        else
        {
            isTouchingWall = false;
        }

    }
}

// Based of Tutorials found on https://sharpcoderblog.com/