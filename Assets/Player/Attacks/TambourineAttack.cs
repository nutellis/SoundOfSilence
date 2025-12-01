using UnityEngine;

[CreateAssetMenu(fileName = "TambourineAttack", menuName = "Combat/TambourineAttack")]
public class TambourineAttack : PlayerAttack
{
    public override void SpecialBehavior()
    {
        base.SpecialBehavior();
        //do what a tambourine does
        Debug.Log("Jingle Jingle!");
    }
}
