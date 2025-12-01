using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Player Attack")]
public class PlayerAttack : ScriptableObject
{
    public int id;
    public string attackName;

    public float attackCooldown = 0.5f;
    
    public float lastAttackTime = -Mathf.Infinity;

    public int attackDamage = 10;
    public float attackRange = 0.5f;

}
