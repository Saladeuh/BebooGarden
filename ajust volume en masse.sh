#!/bin/bash
# Full gpt ce truc
# Chemin du répertoire à parcourir (par défaut, le répertoire courant)
DIR="${1:-.}"

# Fonction pour traiter les fichiers
process_file() {
    local file="$1"
    local ext="${file##*.}"
    local base="${file%.*}"
    local newfile="${base}_volume_adjusted.${ext}"

    echo "Processing: $file -> $newfile"

    ffmpeg -i "$file" -af "volume=0.8" "$newfile"
}

# Export de la fonction pour qu'elle soit utilisable par find
export -f process_file

# Parcours de tous les fichiers dans les sous-dossiers
find "$DIR" -type f -exec bash -c 'process_file "$0"' {} \;

echo "Traitement terminé."

#!/bin/bash

# Chemin du répertoire à parcourir (par défaut, le répertoire courant)
DIR="${1:-.}"

# Supprimer les fichiers qui ne contiennent pas 'volume_adjusted' dans leur nom
find "$DIR" -type f ! -name "*volume_adjusted*" -exec rm -v {} \;

# Renommer les fichiers en retirant 'volume_adjusted' de leur nom
find "$DIR" -type f -name "*volume_adjusted*" | while read -r file; do
    # Nouveau nom de fichier sans 'volume_adjusted'
    newfile="${file//_volume_adjusted/}"
    
    # Renommer le fichier
    echo "Renaming: $file -> $newfile"
    mv -v "$file" "$newfile"
done

echo "Opération terminée."
