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
  public float spinSpeed;
  public float jumpDelay;
  public float maximumFallSpeed;
  public Transform pivot;
  public CharacterController controller;
  public CharacterAnimator characterAnimator;
  
  private bool isDrasticallyChangingDirection;
  private bool isCharacterRotating;
  private bool isStartingJump = false;
  private bool wasJumpCanceled = false;
  private float initialFallVelocity;
  private Vector3 movementDirection;
  private Quaternion rotationDirection;
  private RunningHandler runningHandler;

  // Start is called before the first frame update
  void Start()
  {
    runningHandler = new RunningHandler();
    controller = GetComponent<CharacterController>();
    initialFallVelocity = Physics.gravity.y * fallSpeed;
  }

  // Update is called once per frame
  void Update()
  {
    // Animation must happen before the Character is moved.
    characterAnimator.Animate();

    float horizontalInput = Input.GetAxis("Horizontal");
    float verticalInput = Input.GetAxis("Vertical");

    float absHorizontalInput = Mathf.Abs(horizontalInput);
    float absVericalInput = Mathf.Abs(verticalInput);
    
    bool isInputingDirection =  absHorizontalInput > 0f || absVericalInput > 0f;
    HandleChangeInRotation(horizontalInput, verticalInput, isInputingDirection);

    CalculateMovementDirection(absHorizontalInput, absVericalInput);

    HandleJump();

    HandleGravity();

    // Give momentum to the Character
    if (!isDrasticallyChangingDirection) {
      controller.Move(movementDirection * Time.deltaTime);
    }
  }

  private void HandleJump() {
    if (isStartingJump && !controller.isGrounded) {
      wasJumpCanceled = true;
    }

    if (controller.isGrounded)
    {
      if (Input.GetButtonDown("Jump"))
      {
        StartCoroutine(InitiateJump());
      }
    }
  }

  private IEnumerator InitiateJump()
  {
    isStartingJump = true;

    yield return new WaitForSeconds(jumpDelay);

    if (!wasJumpCanceled) {
      movementDirection.y = 0f;
      movementDirection.y = jumpForce;
      characterAnimator.TriggerIsJumping();
    }

    isStartingJump = false;
    wasJumpCanceled = false;
  }

  private void HandleChangeInRotation(float horizontalInput, float verticalInput, bool isInputingDirection) {
    if (isInputingDirection && !isDrasticallyChangingDirection) {
      isCharacterRotating = true;

      float joystickAngle = Mathf.Atan2(horizontalInput, verticalInput) *
        (180 / Mathf.PI);

      rotationDirection = Quaternion.Euler(
        0f,
        joystickAngle + (pivot.eulerAngles.y - 180f),
        0f
      );

      float changeInRotation = Mathf.Abs(
        rotationDirection.eulerAngles.y - transform.rotation.eulerAngles.y
      ); 

      isDrasticallyChangingDirection = changeInRotation > 170 && changeInRotation < 190;
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
    
    float rotationMultiplier = isDrasticallyChangingDirection ? spinSpeed : 1f;
    
    transform.rotation = Quaternion.RotateTowards(
      transform.rotation,
      rotationDirection,
      Time.deltaTime * rotationSpeed * rotationMultiplier
    );

    bool isRotationComplete = transform.rotation == rotationDirection;
    if (isRotationComplete || !isInputingDirection) {
      isCharacterRotating = false;
      isDrasticallyChangingDirection = false;
    }
  }

  private void HandleGravity() {
    if (controller.isGrounded && movementDirection.y < initialFallVelocity) {
      // be held n the ground by the force of gravity 
      movementDirection.y = initialFallVelocity;
      return;
    }
    // fall with increasing speed unil maximum speed is reached.
    if (movementDirection.y > maximumFallSpeed) {
      movementDirection.y += initialFallVelocity;
      return;
    } 
    movementDirection.y = maximumFallSpeed;
  }

  private void CalculateMovementDirection(float absHorizontalInput, float absVericalInput) {
    float running = Input.GetAxis("Running");
    float runningModifier = runningHandler.GetIsRunningModifier(running);

    float actualSpeed = Mathf.Max(absVericalInput, absHorizontalInput) * movementSpeed * runningModifier; 
    movementDirection = transform.TransformDirection(new Vector3(0f, movementDirection.y, actualSpeed));
  }
}
