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
    public GameObject camera;
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

        movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
 
        if (movementDirection != Vector3.zero) {
          transform.rotation = Quaternion.LookRotation(movementDirection);
        }

        running = Input.GetAxis("Running");
        float runningModifier = runningHandler.GetIsRunningModifier(running);

        movementDirection = movementDirection * movementSpeed * runningModifier;
        movementDirection.y = originalY;

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
