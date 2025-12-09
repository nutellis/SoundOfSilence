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
        // 1. Pick CSV file
        string path = EditorUtility.OpenFilePanel("Select InsultWords CSV", Application.dataPath, "csv");
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("CSV import cancelled.");
            return;
        }

        // 2. Ensure target folder exists
        string targetFolder = "Assets/Data/InsultWords";
        EnsureFolderExists(targetFolder);

        // 3. Read lines
        string[] lines = File.ReadAllLines(path);
        if (lines.Length == 0)
        {
            Debug.LogWarning("CSV file is empty.");
            return;
        }

        int createdCount = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Optional: skip header if it looks like one
            if (i == 0 && (line.ToLower().Contains("display") || line.ToLower().Contains("tags")))
                continue;

            string[] cols = SplitCsvLine(line);

            // We expect EXACTLY 4 columns:
            // 0: id/key
            // 1: displayText
            // 2: tags (semicolon-separated)
            // 3: isNSFW
            if (cols.Length != 4)
            {
                Debug.LogWarning($"Line {i + 1}: expected 4 columns but got {cols.Length}. Skipping. Line: {line}");
                continue;
            }

            string key = cols[0].Trim().Trim('"'); // internal key / id / file name suggestion
            string displayText = cols[1].Trim().Trim('"');
            string tagsCell = cols[2].Trim().Trim('"');
            string isNSFWStr = cols[3].Trim();

            if (string.IsNullOrEmpty(displayText))
            {
                Debug.LogWarning($"Line {i + 1}: empty displayText, skipping.");
                continue;
            }

            // Parse tags separated by ';'
            string[] tags = string.IsNullOrEmpty(tagsCell)
                ? new string[0]
                : tagsCell.Split(';')
                          .Select(t => t.Trim())
                          .Where(t => !string.IsNullOrEmpty(t))
                          .ToArray();

            // Parse bool (True/False/true/false all work with bool.TryParse)
            bool isNSFW = false;
            bool.TryParse(isNSFWStr, out isNSFW);

            // Randomize damage and gold between 1 and 10 (inclusive)
            int baseDamage = Random.Range(1, 11); // max is exclusive
            int goldCost = Random.Range(1, 11);

            // Create asset
            CreateInsultWordAsset(
                targetFolder,
                suggestedName: string.IsNullOrEmpty(key) ? displayText : key,
                displayText: displayText,
                tags: tags,
                baseDamage: baseDamage,
                goldCost: goldCost,
                isNSFW: isNSFW
            );

            createdCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"CSV import complete. Created {createdCount} InsultWord assets in {targetFolder}");
    }

    private static void EnsureFolderExists(string targetFolder)
    {
        if (AssetDatabase.IsValidFolder(targetFolder))
            return;

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

    private static void CreateInsultWordAsset(
        string targetFolder,
        string suggestedName,
        string displayText,
        string[] tags,
        int baseDamage,
        int goldCost,
        bool isNSFW)
    {
        InsultWord wordAsset = ScriptableObject.CreateInstance<InsultWord>();

        string safeName = MakeSafeFileName(suggestedName);
        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{targetFolder}/{safeName}.asset");

        // Assuming your InsultWord fields are public like this:
        // public string displayText;
        // public string[] tags;
        // public bool isNSFW;
        // public int baseDamage;
        // public int goldCost;
        wordAsset.displayText = displayText;
        wordAsset.tags = tags;
        wordAsset.baseDamage = baseDamage;
        wordAsset.goldCost = goldCost;
        wordAsset.isNSFW = isNSFW;

        // Set ID via your function (if you want unique GUIDs)
        wordAsset.id = System.Guid.NewGuid().ToString();

        AssetDatabase.CreateAsset(wordAsset, assetPath);
    }

    private static string[] SplitCsvLine(string line)
    {
        // Simple split by comma (no support for commas inside quotes)
        return line.Split(',');
    }

    private static string MakeSafeFileName(string input)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var safe = new string(input.Where(c => !invalid.Contains(c)).ToArray());

        if (string.IsNullOrWhiteSpace(safe))
            safe = "InsultWord";

        return safe.Trim();
    }
}
