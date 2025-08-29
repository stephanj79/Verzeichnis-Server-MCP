# Verzeichnis-Server-MCP (Model Context Protocol) - Filesystem MCP
Ein robustes C#-Backend, das KI-Agenten die Fähigkeit verleiht, sicher und kontrolliert mit dem lokalen Dateisystem zu interagieren. Dieses Repository implementiert ein definiertes Protokoll, über das ein Agent Aktionen wie das Auflisten von Verzeichnissen, das Lesen oder Schreiben von Dateien anfordern kann. in Verbindung mit N8N ideal zu nutzen.

## ✨ Features / Verfügbare Skills
- Verzeichnisse auflisten (directory_list): Gibt den Inhalt eines bestimmten Verzeichnisses zurück. Unterstützt rekursive und nicht-rekursive Suchen.✅
- Datei kopieren (copy_file): Kopiert eine Datei von einem Quell- zu einem Zielpfad.
- Datei verschieben/umbenennen (move_file) Verschiebt oder benennt eine Datei um.
- Datei löschen (delete_file): Entfernt eine spezifische Datei sicher.
- Textdatei lesen (read_text_file): Liest den Inhalt einer Textdatei und gibt ihn als String zurück. 
- Mediadatei lesen (read_media_file): Liest ein Bild oder ein Audio File. Gibt base64-Daten mit dem passenden MIME-Typ zurück. 
- Textdatei schreiben (write_text_file): Schreibt einen String in eine Textdatei (erstellt oder überschreibt sie).
- Verzeichnis erstellen (create_directory): Erstellt ein neues Verzeichnis am angegebenen Pfad.
- Verzeichnis löschen (delete_directory): Entfernt ein leeres oder volles Verzeichnis.
___
# Filesystem Server MCP (Model Context Protocol)
A robust C# backend that gives AI agents the ability to interact with the local file system in a secure and controlled manner. This repository implements a defined protocol through which an agent can request actions such as listing directories or reading and writing files. Ideal for use in conjunction with N8N.

## ✨ Features / Available Skills
- List Directories (directory_list): Returns the content of a specified directory. Supports recursive and non-recursive searches. ✅
- Copy File (copy_file): Copies a file from a source to a destination path.
- Move/Rename File (move_file): Moves or renames a file.
- Delete File (delete_file): Securely removes a specific file.
- Read Text File (read_text_file): Reads the content of a text file and returns it as a string.
- Read Media File (read_media_file): Reads an image or audio file. Returns base64 data with the appropriate MIME type.
- Write Text File (write_text_file): Writes a string to a text file (creates or overwrites it).
- Create Directory (create_directory): Creates a new directory at the specified path.
- Delete Directory (delete_directory): Removes an empty or full directory.
___
## How to use in n8n
### Credential

<img width="1247" height="772" alt="image" src="https://github.com/user-attachments/assets/4c9d1f9d-1fcd-4791-85bc-296b8f2a67ff" />

___

### MCP List Tools

<img width="845" height="845" alt="image" src="https://github.com/user-attachments/assets/693e5ce9-fe39-40ec-930a-8e04f0f064fe" />

___

### Execute directory_list (parameter)

<img width="1109" height="633" alt="image" src="https://github.com/user-attachments/assets/53523fc9-8ba9-480a-b136-ab8e5c35d3a4" />
