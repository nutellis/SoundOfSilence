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

    public void OnSpellCastingFinished()
    {
        if (parent != null)
            parent.PerformAttack();
        else
            Debug.LogWarning("AnimationEventRelay: parent is null");
    }
}
