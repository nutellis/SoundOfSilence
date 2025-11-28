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

    // This is the function you attach to the Animation Event (must be public, no parameters)
    public void OnSpellCastingFinished()
    {
        if (parent != null)
            parent.PerformAttack();
        else
            Debug.LogWarning("AnimationEventRelay: parent is null");
    }
}
