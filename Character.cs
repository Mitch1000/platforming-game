using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float movementSpeed;
    public float jumpMovementSpeed;
    public float jumpForce;
    public float fallSpeed;
    public float running;
    public Transform pivot;
    public CharacterController controller;

    private RunningHandler runningHandler;
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
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        float joystickAngle = Mathf.Atan2(horizontalInput, verticalInput) *
          (180 / Mathf.PI);

        Vector3 rotationDirection = new Vector3(
          0f,
          joystickAngle + (pivot.eulerAngles.y - 180f),
          0f
        );

        float absHorizontalInput = Mathf.Abs(horizontalInput);
        float absVericalInput = Mathf.Abs(verticalInput);
        if (absHorizontalInput > 0f || absVericalInput > 0f) {
          // About Quaternion:
          // Quaternions represent rotation.
          // Quaternions are a "Double Cover" representation angles represented in 3D space. 
          // This means that there are two possible values to represent each position in 3D space.
          // Quaternions prevent Gimble lock that is common with eulerAngle representations of rotation.
          // First and last number of a Quaternion reflect horizontal rotation(rotation around the z-axis)
          transform.rotation = Quaternion.Euler(rotationDirection);
        }

        running = Input.GetAxis("Running");
        float runningModifier = runningHandler.GetIsRunningModifier(running);

        float actualSpeed = Mathf.Max(absVericalInput, absHorizontalInput) * movementSpeed * runningModifier; 
        movementDirection = transform.TransformDirection(new Vector3(0f, movementDirection.y, actualSpeed));

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
