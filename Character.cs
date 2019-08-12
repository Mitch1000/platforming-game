using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// About Quaternion:
// Quaternions are a "Double Cover" representation angles represented in 3D space. 
// This means that there are two possible values to represent each position in 3D space.
public class Character : MonoBehaviour
{
    public float movementSpeed;
    public float jumpMovementSpeed;
    public float jumpForce;
    public float fallSpeed;
    public float running;
    public Transform pivot;
    private RunningHandler runningHandler;

    public CharacterController controller;

    private Vector3 movementDirection;
    // Start is called before the first frame update

    void Start()
    {
        runningHandler = new RunningHandler();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float originalY = movementDirection.y;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        // First and last number of a Quaternion reflect horizontal rotation(rotation around the z-axis)
        
        float joystickAngle = Mathf.Atan2(horizontalInput, verticalInput) *
          (180 / Mathf.PI);

        movementDirection = new Vector3(
          0f,
          joystickAngle + (pivot.eulerAngles.y - 180f),
          0f
        );

        if (Mathf.Abs(horizontalInput) > 0f || Mathf.Abs(verticalInput) > 0f) {
          transform.rotation = Quaternion.Euler(movementDirection);
        }

        running = Input.GetAxis("Running");
        float runningModifier = runningHandler.GetIsRunningModifier(running);

        movementDirection = movementDirection * movementSpeed * runningModifier;
        // movementDirection.y = originalY;

        if (controller.isGrounded)
        {
            movementDirection.y = 0f;
            if (Input.GetButtonDown("Jump"))
            {
                movementDirection.y = jumpForce;
            }
        }

        movementDirection.y += Physics.gravity.y * fallSpeed;

        controller.Move(movementDirection * Time.deltaTime);
    }
}
