# Verzeichnis-Server-MCP
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

## Wie benutzen in N8N
<img width="1247" height="772" alt="image" src="https://github.com/user-attachments/assets/4c9d1f9d-1fcd-4791-85bc-296b8f2a67ff" />
___
<img width="845" height="845" alt="image" src="https://github.com/user-attachments/assets/693e5ce9-fe39-40ec-930a-8e04f0f064fe" />
___
<img width="1109" height="633" alt="image" src="https://github.com/user-attachments/assets/53523fc9-8ba9-480a-b136-ab8e5c35d3a4" />
___

