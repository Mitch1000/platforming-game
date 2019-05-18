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
        float yStory = movementDirection.y;

        float horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(0, horizontal, 0);

        running = Input.GetAxis("Running");
        float runningModifier = runningHandler.GetIsRunningModifier(running);

        movementDirection = (transform.forward * -Input.GetAxis("Vertical"));
        movementDirection = movementDirection.normalized * movementSpeed * runningModifier;
        movementDirection.y = yStory;

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
