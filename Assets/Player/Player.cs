using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float xSensitivity;
    public float ySensitivity;


    Animator modelAnimator;
    private PlayerInput input;

    public GameObject playerWonUI;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        input = GetComponent<PlayerInput>();

        modelAnimator = GetComponentInChildren<Animator>();


        // hide all weapons
        var instruments = GetComponentsInChildren<Instrument>();
        if(instruments.Length > 0)
        {
            foreach (var weapon in instruments)
            {
                weapon.gameObject.SetActive(false);
            }
        }

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

    public void YouWon()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        input.DeactivateInput();
        playerWonUI.SetActive(true);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
