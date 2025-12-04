using UnityEngine;

public class OnMeleeFinishedEvent : MonoBehaviour
{
    public Attack parent;

    private void Start()
    {
        if (parent == null)
        {
            parent = GetComponentInParent<Attack>();

        }

    }

    public void OnMeleeFinished()
    {
        if (parent != null)
            parent.PerformMeleeAttack();
        else
            Debug.LogWarning("AnimationEventRelay: parent is null");
    }
}
