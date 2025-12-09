using UnityEngine;

public interface InsultTarget
{
    string[] WeaknessTags { get; }
    string[] ResistanceTags { get; }

    void TakeInsultDamage(int amount);
}
