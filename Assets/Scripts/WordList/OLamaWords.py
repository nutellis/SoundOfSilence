import csv
import json
import requests

# ------------------------------
# CONFIG
# ------------------------------

# can be replaced with another local or free LLM endpoint
# Examples:
#   - Local Ollama: http://localhost:11434/v1/chat/completions
#   - LM Studio local server, OpenRouter free models, DeepInfra free models
AI_ENDPOINT = "http://localhost:11434/v1/chat/completions"
MODEL_NAME = "gemma3"  # or another installed/free model

# Number of items to request
REQUESTED_WORDS = 200

CSV_PATH = "Assets/Scripts/WordList/words.csv"
CSV_SAVE_PATH = "Assets/Scripts/WordList/wordsAI.csv"

# ------------------------------
# FUNCTION: ASK AI FOR WORDS
# ------------------------------

def fetch_funny_words():
    """Ask the LLM for a JSON list of words with tags."""
    
    prompt = f"""
You are generating vocabulary for an insult-based monster game.

Provide {REQUESTED_WORDS} entries. 
Each entry must be a JSON object with:

- "word": a single word or short phrase (funny, weird, or usable in humorous insults)
- "tags": list of category tags; possible tags include:
  "appearance", "intelligence", "hygiene", "personality", "cowardice", "smell", "misc"

Rules:
- MUST return valid JSON ONLY (no commentary)
- MUST be a list of objects
- Words should be family-friendly enough for comedic insults, not obscene.

Return ONLY the JSON list.
"""

    response = requests.post(
        AI_ENDPOINT,
        headers={"Content-Type": "application/json"},
        data=json.dumps({
            "model": MODEL_NAME,
            "messages": [{"role": "user", "content": prompt}],
            "temperature": 0.8
        })
    )

    data = response.json()
    print(data)
    # Extract answer depending on provider format
    text = data["choices"][0]["message"]["content"]
    return json.loads(text)


# ------------------------------
# LOAD EXISTING CSV IF PRESENT
# ------------------------------

def load_existing_entries():
    entries = []
    try:
        with open(CSV_PATH, newline='', encoding='utf-8') as csvfile:
            reader = csv.DictReader(csvfile)
            for row in reader:
                entries.append({
                    "id": row["id"],
                    "displayText": row["displayText"],
                    "tags": row["tags"].split(';') if row["tags"] else [],
                    "isNSFW": row["isNSFW"].lower() == "true"
                })
    except FileNotFoundError:
        pass
    return entries


# ------------------------------
# MERGE AND SAVE
# ------------------------------

def merge_and_save(new_words, existing):
    merged = existing.copy()

    for entry in new_words:
        word_id = entry["word"].strip().lower()

        structured = {
            "id": word_id,
            "displayText": entry["word"].strip(),
            "tags": entry["tags"],
            "isNSFW": False
        }

        # Avoid duplicates
        if not any(e["id"] == word_id for e in merged):
            merged.append(structured)

    # Write back
    with open(f"{CSV_PATH}", "w", newline='', encoding='utf-8') as csvfile:
        fieldnames = ["id", "displayText", "tags", "isNSFW"]
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
        writer.writeheader()

        for word in merged:
            writer.writerow({
                "id": word["id"],
                "displayText": word["displayText"],
                "tags": ';'.join(word["tags"]),
                "isNSFW": str(word["isNSFW"])
            })

    print(f"Total words in words.csv: {len(merged)}")


# ------------------------------
# MAIN
# ------------------------------

if __name__ == "__main__":
    print("Fetching new words from AI model...")
    funny_words = fetch_funny_words()

    print(f"Fetched {len(funny_words)} words.")
    existing = load_existing_entries()

    print("Merging and saving...")
    merge_and_save(funny_words, existing)
