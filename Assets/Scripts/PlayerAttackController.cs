using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackController : MonoBehaviour
{
   
    public GameObject startingInstrument;

    private Instrument[] instruments;
    private Instrument attackingInstrument;
    private Boolean fireAttack = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instruments = new Instrument[] {
            startingInstrument.GetComponent<Instrument>()
        };
        foreach (Instrument instrument in instruments)
        {
            instrument.ResetCooldown();
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Instrument instrument in instruments)
        {
            if (instrument.triggerAction.action.triggered)
            {
                SetupAttack(instrument);
            }
        }

        if (fireAttack && attackingInstrument != null)
        {
            FireAttack();
        }
    }

    private void SetupAttack(Instrument instrument)
    {
        attackingInstrument = instrument;
        fireAttack = true;
    }

    private void FireAttack()
    {
       // attackingInstrument.Fire(projectileSpawn.transform);
        attackingInstrument = null;
        fireAttack = false;
    }
}
