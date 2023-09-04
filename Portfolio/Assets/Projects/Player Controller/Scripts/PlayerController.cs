using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;//Reffrences Camera
    [SerializeField] Rigidbody rb;
    [SerializeField] float mouseSensitivity = 3.5f;//Controls Mouse Sensitivity
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.3f;//controls how smooth the Camera movement is

    [SerializeField] float walkSpeed = 6.0f;//controls how fast you walk
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;//controls how smooth the movement is
    [SerializeField] float gravity = -13.0f;//Gravity Value
    [SerializeField] float jumpPower = 5.0f;

    [SerializeField] bool lockCursor = true;//bool to lock Cursor

    CharacterController characterController = null;//CharacterController Refference
    float cameraPitch = 0.0f;//Controls Camera Pitch
    public float velocityY = 0.0f;//Gravity Velocity
    public bool isGrounded = false;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    public Vector3 debugVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();//Plugs in the Controller on the object
        //isGrounded = characterController.isGrounded;

        if (lockCursor == true)
        {
            Cursor.lockState = CursorLockMode.Locked;//locks the cursor
            Cursor.visible = false;//sets the cursor Invisible
        }
    }
    private void FixedUpdate()
    {
        UpdateCameraLook();//Calls the Camera look function
        UpdateMovement();//Calls the Movement Function
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

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed + Vector3.zero;//creates velocity for movement 

        
        if (Input.GetKeyDown(KeyCode.Space))//checks if the characer is grounded
        {

            velocity.y = jumpPower;
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);//Applies the Velocity and makes it independent of framerate
        velocityY = velocity.y;
        debugVelocity = velocity;
    }

  
    private void OnCollisionEnter(Collision other)// Resets the Gravity
    {
        if (other.gameObject.CompareTag("Ground"))
       {
            Debug.Log("Ground");
           isGrounded = characterController.isGrounded;
       }
    }



}
