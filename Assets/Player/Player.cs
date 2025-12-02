using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float xSensitivity;
    public float ySensitivity;


    Animator modelAnimator;
    private PlayerInput input;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        input = GetComponent<PlayerInput>();

        modelAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        
    }



    public void switchToBattle()
    {
        GetComponent<ActorState>().isWalking = false;
        modelAnimator.SetBool("isWalking", false);

        input.SwitchCurrentActionMap("Battle");

        if (input.currentActionMap.name != "Battle")
        {
            //TODO: this needs a better handling.
            Debug.LogError("Failed to switch to Battle action map");

            // assert to crash it :)
            Assert.AreEqual("Battle", input.currentActionMap.name);
        }

        GetComponent<PlayerFreeController>().enabled = false;
       
        var battleController = GetComponent<PlayerComboController>();
        var laneController = GetComponent<PlayerLaneController>();

        if (laneController != null && battleController != null)
        {
            laneController.enabled = true;
            laneController.SetLane(1); // center by default

            battleController.enabled = true;
        }
    }
}
