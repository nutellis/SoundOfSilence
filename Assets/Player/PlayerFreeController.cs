using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFreeController : MonoBehaviour
{
    public float xSensitivity;
    public float ySensitivity;

    public Transform cameraTransform;


    public float speed;
    public float jumpForce;

    private float pitch = 0f;
    private float yaw = 0f;

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

        bool isWalking = movementDirection.magnitude > 0.1f;

        if (modelAnimator != null)
        {
            modelAnimator.SetBool("isWalking", isWalking);
            modelAnimator.SetFloat("Forward", movementDirection.z);
            modelAnimator.SetFloat("Right", movementDirection.x);
        }

        // Update footstep sounds
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateFootsteps(isWalking && !isJumping, Time.deltaTime);
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
        pitch += deltaY;
        pitch = Mathf.Clamp(pitch, -85f, 85f);

        Quaternion yawRotation = Quaternion.AngleAxis(yaw, Vector3.up);
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, Vector3.right);

        cameraTransform.localRotation = yawRotation * pitchRotation;
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
            // Play landing sound only if we were jumping
            if (isJumping && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayJump();
            }
            isJumping = false;
        }
    }
}
