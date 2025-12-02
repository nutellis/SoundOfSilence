using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFreeController : MonoBehaviour
{
    public float xSensitivity;
    public float ySensitivity;

    public Transform cameraTransform;


    public float speed;
    public float jumpForce;

    Vector3 movementDirection;
    Rigidbody rb;

    Animator modelAnimator;

    bool isJumping;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        modelAnimator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        rb.MovePosition(transform.position + transform.TransformDirection(movementDirection) * speed * Time.deltaTime);

        if (modelAnimator != null)
        {
            modelAnimator.SetBool("isWalking", movementDirection.magnitude > 0.1f);
            modelAnimator.SetFloat("Forward", movementDirection.z);
            modelAnimator.SetFloat("Right", movementDirection.x);
        }
    }


    public void OnMouseX(InputAction.CallbackContext context)
    {
        float deltaX = context.ReadValue<float>() * xSensitivity;
        transform.Rotate(0f, deltaX, 0f);
    }

    public void OnMouseY(InputAction.CallbackContext context)
    {
        float deltaY = context.ReadValue<float>() * ySensitivity;
        Vector3 newRotation = cameraTransform.rotation.eulerAngles + new Vector3(deltaY, 0f, 0f);
        cameraTransform.rotation = Quaternion.Euler(newRotation);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();
        movementDirection = new Vector3(movementInput.x, 0f, movementInput.y);

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isJumping == false)
        {
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
