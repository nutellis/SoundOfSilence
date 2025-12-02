from wordfreq import top_n_list
import csv

common_1000 = top_n_list("en", n=1000)

letterWords = ["a", "i"]

commonWords = []
for word in common_1000:
    #check if it is a real word
    if not word.isalpha() or (len(word) < 2 and word not in letterWords):
        continue
    commonWords.append({
        "id": word,
        "displayText": word,
        "tags": ["common"],
        "isNSFW": False
    })

# If words.csv already exists, read it and merge with commonWords
try:
    with open("Assets/Scripts/WordList/words.csv", newline='', encoding='utf-8') as csvfile:
        reader = csv.DictReader(csvfile)
        for row in reader:
            word_entry = {
                "id": row["id"],
                "displayText": row["displayText"],
                "tags": row["tags"].split(';') if row["tags"] else [],
                "isNSFW": row["isNSFW"].lower() == 'true'
            }
            if word_entry not in commonWords:
                commonWords.append(word_entry)
except FileNotFoundError:
    pass

# Write the merged list back to words.csv
with open("Assets/Scripts/WordList/words.csv", 'w', newline='', encoding='utf-8') as csvfile:
    fieldnames = ["id", "displayText", "tags", "isNSFW"]
    writer = csv.DictWriter(csvfile, fieldnames=fieldnames)

    writer.writeheader()
    for word in commonWords:
        writer.writerow({
            "id": word["id"],
            "displayText": word["displayText"],
            "tags": ';'.join(word["tags"]),
            "isNSFW": str(word["isNSFW"])
        })
print(f"Total words in words.csv: {len(commonWords)}")

