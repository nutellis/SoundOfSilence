import os
import csv
import json
import requests

# ------------------------------
# CONFIG
# ------------------------------
# Uses OpenAI REST API. Provide your key in the environment variable `OPENAI_API_KEY`.
# Optionally set `OPENAI_MODEL` env var (defaults to 'gpt-3.5-turbo').
OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
OPENAI_MODEL = os.getenv("OPENAI_MODEL", "gpt-3.5-turbo")
OPENAI_ENDPOINT = "https://api.openai.com/v1/chat/completions"

# Number of items to request
REQUESTED_WORDS = 100

CSV_PATH = "Assets/Scripts/WordList/words.csv"
CSV_SAVE_PATH = "Assets/Scripts/WordList/wordsGPT.csv"

# ------------------------------
# PROMPT
# ------------------------------
PROMPT_TEMPLATE = f"""
You are generating vocabulary for an insult-based monster game.

Provide {REQUESTED_WORDS} entries.
Each entry must be a JSON object with:

- \"word\": a single word (funny, weird, or usable in humorous insults)
- \"tags\": list of category tags; possible tags include:
  \"appearance\", \"intelligence\", \"hygiene\", \"personality\", \"cowardice\", \"smell\", \"misc\"

Rules:
- MUST return valid JSON ONLY (no commentary)
- MUST be a list of objects
- Words should be family-friendly enough for comedic insults, not obscene.

Return ONLY the JSON list.
"""

# ------------------------------
# HELPERS
# ------------------------------

def try_parse_json(text):
    """Try to parse the entire text as JSON, otherwise try to extract the first JSON list substring.
    Returns parsed object or raises JSONDecodeError.
    """
    try:
        return json.loads(text)
    except json.JSONDecodeError:
        # Fallback: find first '[' and last ']' and attempt to parse substring
        start = text.find('[')
        end = text.rfind(']')
        if start != -1 and end != -1 and end > start:
            candidate = text[start:end+1]
            return json.loads(candidate)
        raise

# ------------------------------
# FETCH FROM OPENAI
# ------------------------------

def fetch_funny_words():
    if not OPENAI_API_KEY:
        raise RuntimeError("OPENAI_API_KEY environment variable is not set.")

    headers = {
        "Authorization": f"Bearer {OPENAI_API_KEY}",
        "Content-Type": "application/json"
    }

    payload = {
        "model": OPENAI_MODEL,
        "messages": [
            {"role": "system", "content": "You are a helpful assistant that outputs only valid JSON when asked."},
            {"role": "user", "content": PROMPT_TEMPLATE}
        ],
        # Use lower temperature for more consistent JSON output; you can raise it if you want more variety
        "temperature": 0.2,
        "max_tokens": 2000
    }

    resp = requests.post(OPENAI_ENDPOINT, headers=headers, data=json.dumps(payload))

    try:
        resp.raise_for_status()
    except Exception as e:
        # Include response body for debugging
        raise RuntimeError(f"OpenAI request failed: {e} - status: {resp.status_code} body: {resp.text}")

    data = resp.json()

    # Extract text from the common chat/completions response format
    text = None
    try:
        # Newer Chat Completions: choices[0].message.content
        text = data["choices"][0]["message"]["content"]
    except Exception:
        try:
            # Older/alternate format: choices[0].text
            text = data["choices"][0]["text"]
        except Exception:
            # Fallback: if response includes an 'error' or other structure, report it
            raise RuntimeError(f"Unexpected OpenAI response format: {data}")

    # Try parsing JSON strictly, then fallback to extraction
    try:
        parsed = try_parse_json(text)
    except json.JSONDecodeError as e:
        snippet = text[:2000] if isinstance(text, str) else str(text)
        raise RuntimeError("Failed to parse JSON from model output. Response snippet: " + snippet)

    if not isinstance(parsed, list):
        raise RuntimeError("Parsed JSON is not a list as expected. Got: " + str(type(parsed)))

    return parsed

# ------------------------------
# LOAD/MERGE/SAVE
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


def merge_and_save(new_words, existing):
    merged = existing.copy()

    for entry in new_words:
        # Validate entry structure
        if not isinstance(entry, dict) or "word" not in entry:
            continue

        word_id = entry["word"].strip().lower()

        structured = {
            "id": word_id,
            "displayText": entry["word"].strip(),
            "tags": entry.get("tags", []),
            "isNSFW": False
        }

        # Avoid duplicates
        if not any(e["id"] == word_id for e in merged):
            merged.append(structured)

    # Write back
    with open(CSV_SAVE_PATH, "w", newline='', encoding='utf-8') as csvfile:
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
    print("Fetching new words from OpenAI model...")
    try:
        funny_words = fetch_funny_words()
    except Exception as e:
        print("Error fetching words:", e)
        raise

    print(f"Fetched {len(funny_words)} words.")
    existing = load_existing_entries()

    print("Merging and saving...")
    merge_and_save(funny_words, existing)

    print("Done. Saved to:", CSV_SAVE_PATH)
