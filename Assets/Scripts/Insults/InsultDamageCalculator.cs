using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InsultDamageCalculator : MonoBehaviour
{
    // Default is 1
    [Header("Balance Settings")]
    [SerializeField] private float weaknessBonusPerTag = 1;
    [SerializeField] private float resistancePenaltyPerTag = 1;
    [SerializeField] private float minMultiplier = 1;
    [SerializeField] private float maxMultiplier = 1;

    public int CalculateDamage(IEnumerable<InsultWord> words, string[] weaknessTags, string[] resistanceTags)
    {
        if (words == null) return 0;

        // Base damage = sum of all words baseDamage
        int baseDamage = words.Sum(w => w.baseDamage);
        if (baseDamage <= 0) return 0;

        // Collect all tags from the insult words
        List<string> insultTags = new List<string>();
        foreach (var w in words)
        {
            if (w.tags == null) continue;
            insultTags.AddRange(w.tags);
        }

        // Remove duplicates
        insultTags = insultTags.Distinct().ToList();

        // Count weakness and resistance matches
        int weaknessMatches = 0;
        int resistanceMatches = 0;

        if (weaknessTags != null)
        {
            foreach (var tag in insultTags)
            {
                if (weaknessTags.Contains(tag))
                {
                    weaknessMatches++;
                }
            }
        }

        if (resistanceTags != null)
        {
            foreach (var tag in insultTags)
            {
                if (resistanceTags.Contains(tag))
                {
                    resistanceMatches++;
                }
            }
        }

        // Build multiplier
        float multiplier = 1f
                           + weaknessMatches * weaknessBonusPerTag
                           - resistanceMatches * resistancePenaltyPerTag;

        // Clamp the multiplier so it doesn't get insane or negative
        multiplier = Mathf.Clamp(multiplier, minMultiplier, maxMultiplier);

        //Final damage
        float rawDamage = baseDamage * multiplier;
        int finalDamage = Mathf.RoundToInt(rawDamage);

        Debug.Log($"Insult damage: base={baseDamage}, weakMatches={weaknessMatches}, resistMatches={resistanceMatches}, " +
                  $"mult={multiplier:F2}, final={finalDamage}");

        return Mathf.Max(finalDamage, 0);
    }

    // Helper: apply damage directly to a target using your InsultBuilder
    public void ApplyInsult(InsultBuilder builder, InsultTarget target)
    {
        if (builder == null || target == null) return;

        var words = builder.CurrentWords;
        if (words == null || words.Count == 0)
        {
            Debug.Log("No words in insult – no damage.");
            return;
        }

        int dmg = CalculateDamage(words, target.WeaknessTags, target.ResistanceTags);
        target.TakeInsultDamage(dmg);

    }
}
