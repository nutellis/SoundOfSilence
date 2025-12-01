using UnityEngine;

[CreateAssetMenu(fileName = "GuitarAttack", menuName = "Combat/GuitarAttack")]
public class GuitarAttack : PlayerAttack
{
    public override void SpecialBehavior()
    {
        base.SpecialBehavior();
        //do what a guitar does
        Debug.Log("Strum Strum!");
    }
}
