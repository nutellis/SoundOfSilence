using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLaneController : MonoBehaviour
{
    [Header("Lane Settings")]
    public Transform[] lanePositions; // 0 = Left, 1 = Center, 2 = Right
    public float laneMoveSpeed = 10f;

    [Header("Cooldown Settings")]
    public float laneChangeCooldown = 0.4f;
    private bool isChangingLane = false;

    private int currentLaneIndex = 1; // center by default

    private Rigidbody rb;
    private FirstPersonPlayer playerScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerScript = GetComponent<FirstPersonPlayer>();

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

    public void OnMovement(InputAction.CallbackContext context)
    {
        // Early exit if not ready
        if (lanePositions == null || lanePositions.Length == 0 || playerScript == null) return;
        if (currentLaneIndex < 0 || currentLaneIndex >= lanePositions.Length) return;


        Vector2 input = context.ReadValue<Vector2>();
        float horizontal = input.x;

        // Forward/Backward movement still goes to the original script (only if movement allowed)
        if (playerScript.allowMovement)
        {
            playerScript.OnMovement(context);
        }

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
