using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private CharacterController controller; // refrences the Character Controller component on the Player Gameobject
    private Vector3 playerVelocity;
    public bool groundedPlayer;
    public float playerSpeed = 2.0f; //Controls Player Speed
    public float jumpHeight = 1.0f;//Controls how high the player can jump
    public float gravityValue = -9.81f;// controls how "Heavy" the Gravity is

    private void Start()
    {
        controller = controller = GetComponent<CharacterController>();//Links the Component into the script
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;//Links the Grounded check on the controller into the script
        if (groundedPlayer && playerVelocity.y < 0)//Checks if the Object is Grounded
        {
            playerVelocity.y = 0f;//Sets velocity to 0 enabling smooth movement
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));//Creates a new Velocity based on the Axis inputs
        controller.Move(move * Time.deltaTime * playerSpeed);//Links controls with "real time" and the required speed

        if (move != Vector3.zero)//enables Movement based on the inputs
        {
            gameObject.transform.forward = move;//Moves the object
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)//checks if the jump button has been pressed and if the object is on the ground
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);//Jump math
        }

        playerVelocity.y += gravityValue * Time.deltaTime;//inforces the Y Velocity
        controller.Move(playerVelocity * Time.deltaTime);//inforces the other axis velocity
    }
}
