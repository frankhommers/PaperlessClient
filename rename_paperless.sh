#!/usr/bin/env bash

find . -type f -name "*VMelnalksnis.PaperlessDotNet*" | while IFS= read -r file; do
    # Replace VMelnalksnis.PaperlessDotNet with Paperless in the filepath
    newfile="${file/VMelnalksnis.PaperlessDotNet/Paperless}"

    # Ensure the target directory exists
    mkdir -p "$(dirname "$newfile")"

    # Rename the file
    mv -v "$file" "$newfile"
done
