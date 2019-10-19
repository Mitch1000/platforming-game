using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
  public float distanceToStartLanding;
  public Animator animator;
  public CharacterController controller;

  private float inputX;
  private float inputY;
  private float running;
  private float highestJumpHeight;
  private RunningHandler runningHandler;
  private bool isStartingJump = false;
  private bool isJumping = false;
  private bool hasJumpReachedPeak = false;

  // Start is called before the first frame update
  void Start()
  {
    runningHandler = new RunningHandler();
    animator = this.gameObject.GetComponent<Animator>();
  }

  // Update is called once per frame
  public void Animate()
  {
    running = Input.GetAxis("Running");
    float runningModifier = runningHandler.GetIsRunningModifier(running);

    inputY = Input.GetAxis("Vertical") * runningModifier;
    inputX = Input.GetAxis("Horizontal") * runningModifier;

    bool isWalking = inputY != 0f || inputX != 0f;

    animator.SetFloat("InputX", inputX);
    animator.SetFloat("InputY", inputY);

    HandleJumping();
    animator.SetBool("IsGrounded", controller.isGrounded);
    animator.SetBool("IsWalking", isWalking);
  }

  private void HandleJumping() {
    if (Input.GetButtonDown("Jump") && controller.isGrounded) {
      isStartingJump = true;
      animator.SetTrigger("IsStartingJump");
      highestJumpHeight = Mathf.Round(transform.position.y * 100) / 100;
    }

    if (!isStartingJump) { return; }

    if (!isJumping) {
      if (!controller.isGrounded) {
        isStartingJump = false; 
        // Set to falling animation once the animation is created.
        animator.SetTrigger("IsFalling");
      }

      return;
    }

    float currentCharacterHeight = Mathf.Round(transform.position.y * 100) / 100;
    if (highestJumpHeight <= currentCharacterHeight) {
      highestJumpHeight = currentCharacterHeight;
    } else {
      hasJumpReachedPeak = true;
    }

    if (!hasJumpReachedPeak) { return; }

    RaycastHit hit = new RaycastHit();
    int raycastLayerMask = 1 << 8; 
    if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, raycastLayerMask)) {
      float distanceToFloor = hit.distance;
      if (distanceToFloor < distanceToStartLanding) {
        animator.SetTrigger("IsFalling");
        isJumping = false;
        hasJumpReachedPeak = false;
        isStartingJump = false;
      }
    }
  }

  public void TriggerIsJumping() {
    isJumping = true;
  }
}
