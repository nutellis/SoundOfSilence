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

######---------------- PATH-------------------------------
##### Change them to your local path
CSV_PATH = "/home/smokey/Projects/soundOfSilence/words.csv"
CSV_SAVE_PATH = "/home/smokey/Projects/soundOfSilence.csv"

# ------------------------------
# FUNCTION: ASK AI FOR WORDS
# ------------------------------

def fetch_funny_words():
    """Ask the LLM for a JSON list of words with tags."""
    
    prompt = f"""
You are generating insult vocabulary for a comedic, slightly edgy boss-fight game set in a church where the player roasts a corrupt priest using musical attacks and insult words.

Generate {REQUESTED_WORDS} entries.

Each entry MUST be a JSON object with:
- "word": a single word or short insult phrase (salty, spicy, sarcastic, or lightly NSFW, but WITHOUT explicit sexual content or slurs; think PG-13 insults like "sin-sniffer", "filthy cherub", "guilt-goblin", "holy fraud", "altar creeper", "shameless sermonizer")
- "tags": list of category tags from:
  "appearance", "intelligence", "hygiene", "personality", "cowardice", "smell", "religion", "misc"

Tone requirements:
- Humor style: irreverent, spicy, sarcastic, church-themed mockery
- Allowed NSFW level: *mild PG-13 profanity only* (e.g., “dumbass”, “jackass”, “creep”, “bastard”), but NO explicit sexual content, NO hate speech, NO slurs.
- Focus on insults that are ridiculous, exaggerated, weird, or theatrical.
- Should feel like over-the-top roasting, not real-world offensive hate.
- Atleast have 100 words

Rules:
- MUST return valid JSON ONLY.
- NO markdown, NO code fences, NO ```json.
- MUST be a JSON list of objects like:
  [{{"word": "...", "tags": ["appearance"]}}, ...]
- Words should be NSFW enough for comedic insults, not obscene.

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
    print(data)  # you already have this

    text = data["choices"][0]["message"]["content"]
    text = text.strip()

    # --- remove ```json ... ``` wrapper if present ---
    if text.startswith("```"):
        lines = text.splitlines()

        # remove first line if it starts with ``` or ```json
        if lines and lines[0].startswith("```"):
            lines = lines[1:]

        # remove last line if it's ``` 
        if lines and lines[-1].startswith("```"):
            lines = lines[:-1]

        text = "\n".join(lines).strip()

    # now text should be a clean JSON list: [ {...}, {...} ]
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
