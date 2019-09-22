using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
  public float movementSpeed;
  public float jumpMovementSpeed;
  public float jumpForce;
  public float fallSpeed;
  public float rotationSpeed;
  public Transform pivot;
  public CharacterController controller;

  private RunningHandler runningHandler;
  private Vector3 movementDirection;
  private bool isCharacterRotating;

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

      float absHorizontalInput = Mathf.Abs(horizontalInput);
      float absVericalInput = Mathf.Abs(verticalInput);
      
      bool isInputingDirection =  absHorizontalInput > 0f || absVericalInput > 0f;
      handleChangeInRotation(horizontalInput, verticalInput, isInputingDirection);

      calculateMovementDirection(absHorizontalInput, absVericalInput);

      handleJump();
      // fall
      movementDirection.y += Physics.gravity.y * fallSpeed;
      // Give momentum to the Character
      controller.Move(movementDirection * Time.deltaTime);
  }

  private void handleJump() {
    if (controller.isGrounded)
    {
      movementDirection.y = 0f;
      if (Input.GetButtonDown("Jump"))
      {
        movementDirection.y = jumpForce;
      }
    }
  }

  private void handleChangeInRotation(float horizontalInput, float verticalInput, bool isInputingDirection) {
    if (isInputingDirection) {
      isCharacterRotating = true;
    }

    if (!isCharacterRotating) { return; }

    // About Quaternions:
    //
    // Quaternions represent rotation.
    // Quaternions are a "Double Cover" representation angles represented in 3D space. 
    // This means that there are two possible values to represent each position in 3D space.
    // Quaternions prevent Gimble lock that is common with eulerAngle representations of rotation.
    // Quaternions are Vectors containg 4 numbers a +b(i) + c(j) + d(k)
    // Each value in the Vector cannot be modified individually because they are all codependent. 
    // and must fit a Mathematical formula i2= j2 = k2 = ijk = −1.
    // Quaternions can only be multiplied and the order they are multiplied changes the angle that
    // is produced.
    
    float joystickAngle = Mathf.Atan2(horizontalInput, verticalInput) *
      (180 / Mathf.PI);

    Quaternion rotationDirection = Quaternion.Euler(
      0f,
      joystickAngle + (pivot.eulerAngles.y - 180f),
      0f
    );
    
    transform.rotation = Quaternion.RotateTowards(
      transform.rotation,
      rotationDirection,
      Time.deltaTime * rotationSpeed 
    );

    bool isRotationComplete = transform.rotation == rotationDirection;
    if (isRotationComplete || !isInputingDirection) {
      isCharacterRotating = false;
    }
  }

  private void calculateMovementDirection(float absHorizontalInput, float absVericalInput) {
    float running = Input.GetAxis("Running");
    float runningModifier = runningHandler.GetIsRunningModifier(running);

    float actualSpeed = Mathf.Max(absVericalInput, absHorizontalInput) * movementSpeed * runningModifier; 
    movementDirection = transform.TransformDirection(new Vector3(0f, movementDirection.y, actualSpeed));
  }
}
