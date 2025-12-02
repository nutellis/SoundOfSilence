using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLaneController : MonoBehaviour
{
    [Header("Look Sensitivity")]
    public float xSensitivity;
    public float ySensitivity;


    public Transform cameraTransform;


    [Header("Lane Settings")]
    public Transform[] lanePositions; // 0 = Left, 1 = Center, 2 = Right
    public float laneMoveSpeed = 10f;

    [Header("Cooldown Settings")]
    public float laneChangeCooldown = 0.4f;
    private bool isChangingLane = false;

    private int currentLaneIndex = 1; // center by default

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (lanePositions == null || lanePositions.Length == 0)
        {
            Debug.LogError("PlayerLaneController: 'lanePositions' is not set or is empty. Disabling script.");
            enabled = false;
            return;
        }

        if (lanePositions.Length != 3)
        {
            Debug.LogWarning("PlayerLaneController expects 3 lane positions (Left, Center, Right). Found: " + lanePositions.Length);
        }

        currentLaneIndex = Mathf.Clamp(currentLaneIndex, 0, lanePositions.Length - 1);

        if (lanePositions == null || lanePositions.Length == 0)
            return;

        if (currentLaneIndex < 0 || currentLaneIndex >= lanePositions.Length)
            return;

        var laneTransform = lanePositions[currentLaneIndex];
        if (laneTransform == null)
            return;

        Vector3 targetPos = new Vector3(
            laneTransform.position.x,
            transform.position.y,
            transform.position.z
        );

        Vector3 newPos = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * laneMoveSpeed);
        rb.MovePosition(newPos);
    }

    void Update()
    {
        if (lanePositions == null || lanePositions.Length == 0)
            return;

        if (currentLaneIndex < 0 || currentLaneIndex >= lanePositions.Length)
            return;

        var laneTransform = lanePositions[currentLaneIndex];
        if (laneTransform == null)
            return;

        Vector3 targetPos = new Vector3(
            laneTransform.position.x,
            transform.position.y,
            transform.position.z
        );

        Vector3 newPos = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * laneMoveSpeed);
        rb.MovePosition(newPos);
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
        Vector3 newRotation = cameraTransform.rotation.eulerAngles + new Vector3(deltaY, 0f, 0f);
        cameraTransform.rotation = Quaternion.Euler(newRotation);
    }

    public void OnLaneMovement(InputAction.CallbackContext context)
    {
        // Early exit if not ready
        if (lanePositions == null || lanePositions.Length == 0) return;
        if (currentLaneIndex < 0 || currentLaneIndex >= lanePositions.Length) return;


        Vector2 input = context.ReadValue<Vector2>();
        float horizontal = input.x;

        if (!context.performed || isChangingLane)
            return;

        if (horizontal > 0.5f)
        {
            TryChangeLane(+1);
        }
        else if (horizontal < -0.5f)
        {
            TryChangeLane(-1);
        }
    }
    public void SetLane(int laneIndex)
    {
        currentLaneIndex = Mathf.Clamp(laneIndex, 0, lanePositions.Length - 1);
    }

    private void TryChangeLane(int direction)
    {
        if (lanePositions == null || lanePositions.Length == 0)
            return;

        int newLaneIndex = Mathf.Clamp(currentLaneIndex + direction, 0, lanePositions.Length - 1);

        if (newLaneIndex != currentLaneIndex)
        {
            currentLaneIndex = newLaneIndex;
            if(laneChangeCooldown > 0 )
                StartCoroutine(LaneCooldown());
        }
    }

    private System.Collections.IEnumerator LaneCooldown()
    {
        isChangingLane = true;
        yield return new WaitForSeconds(laneChangeCooldown);
        isChangingLane = false;
    }
}
