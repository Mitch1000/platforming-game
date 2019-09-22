using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingAnimator : MonoBehaviour
{

    public Animator animator;
    public float inputX;
    public float inputY;
    public float running;
    private RunningHandler runningHandler;

    // Start is called before the first frame updatejjj
    void Start()
    {
      runningHandler = new RunningHandler();
      animator = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
      running = Input.GetAxis("Running");
      float runningModifier = runningHandler.GetIsRunningModifier(running);

      inputY = Input.GetAxis("Vertical") * runningModifier;
      inputX = Input.GetAxis("Horizontal") * runningModifier;

      animator.SetFloat("InputX", inputX);
      animator.SetFloat("InputY", inputY);
      bool isWalking = inputY != 0f || inputX != 0f;
      animator.SetBool("IsWalking", isWalking);
    }
}
