using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;//Reffrences Camera
    [SerializeField] float mouseSensitivity = 3.5f;//Controls Mouse Sensitivity
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.3f;//controls how smooth the Camera movement is

    [SerializeField] float walkSpeed = 6.0f;//controls how fast you walk
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;//controls how smooth the movement is
    [SerializeField] float gravity = -13.0f;//Gravity Value

    [SerializeField] bool lockCursor = true;//bool to lock Cursor

    CharacterController characterController = null;//CharacterController Refference
    float cameraPitch = 0.0f;//Controls Camera Pitch
    float velocityY = 0.0f;//Gravity Velocity

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();//Plugs in the Controller on the object
        if (lockCursor == true)
        {
            Cursor.lockState = CursorLockMode.Locked;//locks the cursor
            Cursor.visible = false;//sets the cursor Invisible
        }
    }
    private void Update()
    {
        UpdateCameraLook();//Calls the Camera look function
        UpdateMovement();//Calls teh Movement Function
    }

    void UpdateCameraLook()
    {
        Vector2 tragetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));//Gets the Mouse Inputs from Unitys systems

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, tragetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);//Smooth Camera

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;//controls the Vertical Mouse Pitch
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);//sets the Clamp Angle

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;//Clamps the camera so you cant spin it

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);//Controls the Horizontal Mouse look
    } //Controls Camera Look

    void UpdateMovement()//Controls Character Movement
    {
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));//Stores the input values
        targetDir.Normalize();//normalizes the Vectoprs so you dont go faster on diagonals

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);//smooths movement

        if (characterController.isGrounded == true)
        {
            velocityY = 0.0f;
            velocityY += gravity * Time.deltaTime;
        }//Defines Gravity

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed + Vector3.up * velocityY;//creates velocity for both movement and gravity

        characterController.Move(velocity * Time.deltaTime);//Applies the Velocity and makes it independent of framerate
    }
}
