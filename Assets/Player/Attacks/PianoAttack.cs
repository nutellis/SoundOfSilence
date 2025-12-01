using UnityEngine;

[CreateAssetMenu(fileName = "PianoAttack", menuName = "Combat/PianoAttack")]
public class PianoAttack : PlayerAttack
{
    public override void SpecialBehavior()
    {
        base.SpecialBehavior();
        //do what a guitar does
        Debug.Log("Plink Plonk!");
    }
}
