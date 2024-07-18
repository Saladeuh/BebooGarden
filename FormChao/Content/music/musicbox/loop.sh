#!/bin/bash

# Boucle à travers tous les fichiers .wav dans le dossier courant
for file in *.mp3; do
  # Vérifie que le fichier existe (pour éviter des erreurs si aucun fichier .wav n'est trouvé)
  [ -e "$file" ] || continue
  
  # Extrait le nom de base du fichier (sans extension)
  base_name="${file%.mp3}"
  
  # Définit le nom du fichier de sortie .txt
  output_file="${base_name}.txt"
  
  # Exécute la commande pymusiclooper et redirige uniquement la sortie finale
  pymusiclooper -i export-points --path "$file" --export-to txt
  # Affiche un message indiquant que le fichier a été traité
  echo "Traitement de $file terminé, sortie dans $output_file"
done

echo "Tous les fichiers .wav ont été traités."
