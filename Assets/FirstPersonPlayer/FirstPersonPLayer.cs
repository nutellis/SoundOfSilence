using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FirstPersonPlayer : MonoBehaviour
{

    public float xSensitivity;
    public float ySensitivity;

    public Transform cameraTransform;


    public float speed;
    public float jumpForce;
    public Animator modelAnimator;

    Vector3 movementDirection;
    Rigidbody rb;

    bool isJumping;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //transform.position += transform.TransformDirection(movementDirection) * speed * Time.deltaTime;
        rb.MovePosition(transform.position + transform.TransformDirection(movementDirection) * speed * Time.deltaTime);

        modelAnimator.SetBool("isWalking", movementDirection.magnitude > 0.1f);
        modelAnimator.SetFloat("Forward", movementDirection.z);
        modelAnimator.SetFloat("Right", movementDirection.x);
    }


    public void OnMouseX(InputAction.CallbackContext context)
    {
        float deltaX = context.ReadValue<float>() * xSensitivity;
        Debug.Log("Mouse X movement: " + deltaX);
        transform.Rotate(0f, deltaX, 0f);
    }

    public void OnMouseY(InputAction.CallbackContext context)
    {
        float deltaY = context.ReadValue<float>() * ySensitivity;
        Debug.Log("Mouse Y movement: " + deltaY);
        Vector3 newRotation = cameraTransform.rotation.eulerAngles + new Vector3(Mathf.Clamp(-deltaY, -80f, 80f), 0f, 0f);
        cameraTransform.rotation = Quaternion.Euler(newRotation);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();
        Debug.Log("Movement input: " + movementInput);
        movementDirection = new Vector3(movementInput.x, 0f, movementInput.y);

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isJumping == false)
        {
            Debug.Log("Jump!");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Simple ground check
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isJumping = false;
        }
    }
}
