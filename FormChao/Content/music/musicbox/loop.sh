for file in *.mp3; do    
  pymusiclooper -i export-points --path "$file"
done