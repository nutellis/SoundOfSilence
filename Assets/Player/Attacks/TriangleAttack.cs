using UnityEngine;

[CreateAssetMenu(fileName = "TriangleAttack", menuName = "Combat/TriangleAttack")]
public class TriangleAttack : PlayerAttack
{
    public override void SpecialBehavior()
    {
        base.SpecialBehavior();

        Debug.Log("Ding Ding Ding!");
        //do what a triangle does
    }
}
