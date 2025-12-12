using UnityEngine;

public class OnSpellCastingFinishedEvent : MonoBehaviour
{
    public Attack parent;

    private void Start()
    {
        if (parent == null)
        {
            parent = GetComponentInParent<Attack>();

        }

    }

    public void CastSpell()
    {
        if (parent != null)
            parent.CastSpell();
        else
            Debug.LogWarning("AnimationEventRelay: parent is null");
    }

    public void OnSpellCastingFinished()
    {
        if (parent != null)
            parent.RangedAttackFinished();
        else
            Debug.LogWarning("AnimationEventRelay: parent is null");
    }
}
