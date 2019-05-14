using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingAnimator : MonoBehaviour
{

    public Animator animator;
    public float inputX;
    public float inputY;
    // Start is called before the first frame updatejjj
    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        inputY = Input.GetAxis("Vertical");
        inputX = Input.GetAxis("Horizontal");
        animator.SetFloat("InputX", inputX);
        animator.SetFloat("InputY", inputY);
    }
}