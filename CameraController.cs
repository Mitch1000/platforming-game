using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float rotateSpeed;
    public Transform pivot;
    public float maxViewAngle;
    public float minViewAngle;
    public bool invertY;
    public float cameraResetSpeed;

    private Quaternion originalRotation;
    private Vector3 offset;
    private bool isCameraReseting;

    // Start is called before the first frame update
    void Start()
    {
        offset = target.position - transform.position;
        pivot.position = target.position;

        originalRotation = pivot.rotation;
        isCameraReseting = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float horizontalCameraInput = Input.GetAxis("Mouse X") + Input.GetAxis("JoyCameraHorizontal");
        float verticalCameraInput = Input.GetAxis("Mouse Y") + Input.GetAxis("JoyCameraVertical");

        bool isCameraMovingManually = horizontalCameraInput != 0 || verticalCameraInput != 0;
        handleCameraReset(isCameraMovingManually);
        // Get X Position of Mouse and Move Target(Character)
        float horizontal = horizontalCameraInput * rotateSpeed;
        pivot.Rotate(0, horizontal, 0);

        // Get Y Position of Mouse and Move Pivot
        float vertical = verticalCameraInput * rotateSpeed;

        if (invertY)
        {
          pivot.Rotate(vertical, 0, 0);
        }
        else
        {
          pivot.Rotate(-vertical, 0, 0);
        }

        // Limit up/down camera rotation
        if (pivot.rotation.eulerAngles.x > maxViewAngle && pivot.rotation.eulerAngles.x < 180f)
        {
          pivot.rotation = Quaternion.Euler(maxViewAngle, pivot.rotation.eulerAngles.y, 0);
        }

        if (pivot.rotation.eulerAngles.x > 180f && pivot.rotation.eulerAngles.x < 360f + minViewAngle)
        {
          pivot.rotation = Quaternion.Euler(360f + minViewAngle, pivot.rotation.eulerAngles.y, 0);
        }

        // Move camera based on pivot position
        float desiredYAngle = pivot.eulerAngles.y;
        float desiredXAngle = pivot.eulerAngles.x;

        Quaternion rotation = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);
        transform.position = target.position - (rotation * offset);

        transform.LookAt(target);

        if(transform.position.y < target.position.y)
        {
            transform.position = new Vector3(
              transform.position.x,
              target.position.y - 0.5f,
              transform.position.z
            );
        }
    }

    private void handleCameraReset(bool isCameraMovingManually) {
      // Camera Center button doesn't do anything while the Camera Control Joystick is active.
      // Centering the camera is canceled by Camera Control Joystick movement.
      if (Input.GetButtonDown("CenterCamera") && !isCameraMovingManually) {
        isCameraReseting = true;
      }

      if (isCameraReseting) {
        pivot.rotation = Quaternion.RotateTowards(
          pivot.rotation,
          originalRotation * target.rotation,
          Time.deltaTime * cameraResetSpeed 
        );

        bool isPivotResetComplete = pivot.rotation == originalRotation * target.rotation;
        if (isPivotResetComplete || isCameraMovingManually) {
          isCameraReseting = false;
        }
      }
    }
}
