using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class InsultWordCsvImporter
{
    // Menu: Tools → Insults → Import InsultWords from CSV
    [MenuItem("Tools/Insults/Import InsultWords from CSV")]
    public static void ImportFromCsv()
    {
        // 1. Let user pick the CSV file
        string path = EditorUtility.OpenFilePanel("Select InsultWords CSV", Application.dataPath, "csv");
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("CSV import cancelled.");
            return;
        }

        // 2. Where to save the ScriptableObject assets
        string targetFolder = "Assets/Data/InsultWords";
        if (!AssetDatabase.IsValidFolder(targetFolder))
        {
            // Create folder if it doesn't exist
            string[] parts = targetFolder.Split('/');
            string current = "Assets";
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }
                current = next;
            }
        }

        // 3. Read all lines
        string[] lines = File.ReadAllLines(path);

        if (lines.Length <= 1)
        {
            Debug.LogWarning("CSV file seems empty or only has a header.");
            return;
        }

        // 4. Assume first line is header
        int startLine = 1;

        int createdCount = 0;

        for (int i = startLine; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Basic CSV split by comma.
            // NOTE: This assumes no commas inside fields.
            string[] cols = SplitCsvLine(line);
            if (cols.Length < 5)
            {
                Debug.LogWarning($"Line {i + 1}: expected 5 columns but got {cols.Length}. Skipping.");
                continue;
            }

            string displayText = cols[0].Trim().Trim('"');
            string tagsCell = cols[1].Trim().Trim('"');
            string baseDamageStr = cols[2].Trim();
            string goldCostStr = cols[3].Trim();
            string isNSFWStr = cols[4].Trim();

            if (string.IsNullOrEmpty(displayText))
            {
                Debug.LogWarning($"Line {i + 1}: empty displayText, skipping.");
                continue;
            }

            // Parse tags (split by ';')
            string[] tags = string.IsNullOrEmpty(tagsCell)
                ? new string[0]
                : tagsCell.Split(';').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToArray();

            // Parse ints
            if (!int.TryParse(baseDamageStr, out int baseDamage))
            {
                baseDamage = 1;
            }

            if (!int.TryParse(goldCostStr, out int goldCost))
            {
                goldCost = 1;
            }

            // Parse bool
            bool isNSFW = false;
            if (!bool.TryParse(isNSFWStr, out isNSFW))
            {
                // allow "1"/"0", "yes"/"no"
                string lower = isNSFWStr.ToLowerInvariant();
                isNSFW = (lower == "1" || lower == "yes" || lower == "y" || lower == "true");
            }

            // 5. Create the ScriptableObject asset
            InsultWord wordAsset = ScriptableObject.CreateInstance<InsultWord>();

            // Try to generate a clean asset name from displayText
            string safeName = MakeSafeFileName(displayText);
            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{targetFolder}/{safeName}.asset");

            // Set fields according to your current InsultWord definition
            wordAsset.displayText = displayText;
            wordAsset.tags = tags;
            wordAsset.baseDamage = baseDamage;
            wordAsset.goldCost = goldCost;
            wordAsset.isNSFW = isNSFW;

            // Set ID via your setter (or you can leave it empty and let getID() generate it lazily)
            wordAsset.id = System.Guid.NewGuid().ToString();

            AssetDatabase.CreateAsset(wordAsset, assetPath);
            createdCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"CSV import complete. Created {createdCount} InsultWord assets in {targetFolder}");
    }

    // Very simple CSV splitter (doesn't handle quotes with commas inside)
    private static string[] SplitCsvLine(string line)
    {
        // Basic split by comma
        // If you need advanced CSV parsing later, you can swap this out.
        return line.Split(',');
    }

    private static string MakeSafeFileName(string input)
    {
        // Remove invalid filename chars and trim
        var invalid = Path.GetInvalidFileNameChars();
        var safe = new string(input.Where(c => !invalid.Contains(c)).ToArray());

        if (string.IsNullOrWhiteSpace(safe))
            safe = "InsultWord";

        return safe.Trim();
    }
}
