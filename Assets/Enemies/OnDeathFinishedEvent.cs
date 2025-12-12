using UnityEngine;

public class OnDeathFinishedEvent : MonoBehaviour
{
    public Minion parent;

    private void Start()
    {
        if (parent == null)
        {
            parent = GetComponentInParent<Minion>();

        }

    }

    public void OnDeathFinished()
    {
        if (parent != null)
            parent.Die();
        else
            Debug.LogWarning("AnimationEventRelay: parent is null");
    }
}
