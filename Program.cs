using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

bool _isLogging = true;
var s = string.Empty;

string[]? arg = Environment.GetCommandLineArgs();
if (arg.Length > 0)
{
    _isLogging = arg[1].ToString() == "1";
}

string logFilePath = Path.Combine(AppContext.BaseDirectory, "log.txt");
for (int i = 0; i < arg.Length; i++)
{
    s += i.ToString() + ":" + arg[i] + "\r";
}
File.AppendAllText(logFilePath, $"Anzahl:{arg.Length}\r{s}\r");

if (_isLogging)
{
    File.AppendAllText(logFilePath, $"--- SITZUNG GESTARTET: {DateTime.Now} ---\n");
}

string resourcesPath = @"E:\testN8N\Resources";
Directory.CreateDirectory(resourcesPath);

// Eine Endlosschleife, um die Anwendung am Laufen zu halten.
while (true)
{
    var jsonInput = Console.ReadLine();

    if (string.IsNullOrEmpty(jsonInput))
    {
        Log("Leere Zeile empfangen, beende Sitzung.");
        break;
    }

    Log($"Empfangen: {jsonInput}");

    object? requestId = null;
    try
    {
        var request = JsonSerializer.Deserialize<ToolExecutionRequest>(jsonInput, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        requestId = request?.Id;

        // Benachrichtigungen (ohne ID) ignorieren und keine Antwort senden.
        if (requestId == null)
        {
            if (request?.Method?.ToLower() == "utilities/logging")
            {
                Log($"[CLIENT LOG]: {request?.Params?.LogEntry?.Message}");
            }
            else
            {
                Log("Benachrichtigung empfangen, wird ignoriert.");
            }
            continue;
        }

        string? methodToExecute = request?.Method?.ToLower();

        switch (methodToExecute)
        {
            case "initialize":
                var initializeResult = new InitializeResult
                {
                    ProtocolVersion = request?.Params?.protocolVersion ?? "2025-07-01",
                    ServerInfo = new ServerInfo { Name = "Verzeichnis-Server-MCP", Version = "1.0.0" },
                    Capabilities = new ServerCapabilities { Tools = new { }, Prompts = new { }, Resources = new { }, Utilities = new { } }
                };
                SendResponse(initializeResult, requestId);
                break;

            // --- Tools ---
            case "tools/list":
                var toolList = new object[]
                {
                    new { name = "HelloWorld", description = "Ein einfacher Test-Tool, das 'HALLO WELT' zurückgibt.",
                        inputSchema = new {
                            type = "object",
                            properties = new {},
                            required = new string[] {}
                        }
                    },
                    new { name = "directory_list", description = "Gibt den Inhalt eines bestimmten Verzeichnisses zurück. Unterstützt rekursive und nicht-rekursive Suchen.",
                        inputSchema = new {
                            type = "object",
                            properties = new {
                                Pfad = new {
                                    type = "string",
                                    description = "Der vollständige Pfad zum Verzeichnis."
                                }
                            }, required = new string[] {                                "Pfad"                            }
                        }
                    },
                    new {
                        name = "read_text_file",
                        description = "Liest den Inhalt einer Textdatei und gibt ihn als String zurück.",
                        inputSchema = new {
                            type = "object",
                            properties = new {
                                Pfad = new {
                                    type = "string",
                                    description = "Der vollständige Pfad zur Textdatei."
                                }
                            }, required = new string[] {                                "Pfad"                            }
                        }
                    },
                    new { name = "read_media_file", description = "Liest ein Bild oder ein Audio File. Gibt base64-Daten mit dem passenden MIME-Typ zurück.",
                        inputSchema = new {
                            type = "object",
                            properties = new {
                                Pfad = new {
                                    type = "string",
                                    description = "Der vollständige Pfad zur Mediendatei."
                                }
                            },
                            required = new string[] { "Pfad" }
                        }
                    },
                    new { name = "write_text_file", description = " Schreibt einen String in eine Textdatei (erstellt oder überschreibt sie).",
                        inputSchema = new {
                            type = "object",
                            properties = new {
                                Pfad = new {
                                    type = "string",
                                    description = "Der vollständige Pfad zur Textdatei."
                                } ,
                                Content = new {
                                    type = "string",
                                    description = "Der Inhalt der Textdatei."
                                }
                            },
                            required = new string[] { "Pfad","Content" }
                        }
                    },
                    new { name = "copy_file", description = "Kopiert eine Datei von einem Quell- zu einem Zielpfad.",
                        inputSchema = new {
                            type = "object",
                            properties = new {
                                QuellePfad = new {
                                    type = "string",
                                    description = "Der vollständige Pfad zur Textdatei."
                                } ,
                                ZielPfad = new {
                                    type = "string",
                                    description = "Der neue vollständige Pfad zur Textdatei."
                                }
                            },
                            required = new string[] { "QuellePfad", "ZielPfad" }
                        }
                    },
                    new { name = "move_file", description = "Verschiebt oder benennt eine Datei um.",
                        inputSchema = new {
                            type = "object",
                            properties = new {
                                QuellePfad = new {
                                    type = "string",
                                    description = "Der vollständige Pfad zur Textdatei."
                                } ,
                                ZielPfad = new {
                                    type = "string",
                                    description = "Der neue vollständige Pfad zur Textdatei."
                                }
                            },
                            required = new string[] { "QuellePfad", "ZielPfad" }
                        }
                    },
                    new { name = "delete_file", description = "Entfernt eine spezifische Datei sicher.",
                        inputSchema = new {
                            type = "object",
                            properties = new {
                                Pfad = new {
                                    type = "string",
                                    description = "Der vollständige Pfad zur Textdatei."
                                }
                            },
                            required = new string[] { "Pfad" }
                        }
                    },
                    new { name = "create_directory", description = "Erstellt ein neues Verzeichnis am angegebenen Pfad.",
                        inputSchema = new {
                            type = "object",
                            properties = new {
                                Pfad = new {
                                    type = "string",
                                    description = "Der vollständige Pfad."
                                }
                            },
                            required = new string[] { "Pfad" }
                        }
                    },
                    new { name = "delete_directory", description = "Entfernt ein leeres oder volles Verzeichnis.",
                        inputSchema = new {
                            type = "object",
                            properties = new {
                                Pfad = new {
                                    type = "string",
                                    description = "Der vollständige Pfad."
                                }
                            },
                            required = new string[] { "Pfad" }
                        }
                    }
                };
                SendResponse(new { tools = toolList }, requestId);
                break;

            case "tools/call":
                HandleToolCall(request, requestId);
                break;

            // --- Prompts, Resources, Utilities bleiben unverändert ---
            case "prompts/list":
                var promptList = new[]
                {
                    new {
                        name = "alle_ressourcen_zusammenfassen_final",
                        description = "Fasst den Inhalt aller verfügbaren Ressourcen zusammen, indem es den Server die Vor-Zusammenfassung machen lässt."
                    }
                };
                SendResponse(new { prompts = promptList }, requestId);
                break;

            case "prompts/get":
                HandlePromptGet(request, requestId);
                break;

            case "resources/list":
                try
                {
                    string[] filePaths = Directory.GetFiles(resourcesPath);
                    var allResources = filePaths.Select(filePath =>
                    {
                        var resourceName = Path.GetFileName(filePath);
                        return new
                        {
                            uri = $"mcp://verzeichnis-server-mcp/resources/{Uri.EscapeDataString(resourceName)}",
                            name = resourceName,
                            description = $"Datei aus dem Ressourcen-Verzeichnis."
                        };
                    }).ToList();

                    int limit = request?.Params?.Limit ?? 10;
                    int offset = request?.Params?.Offset ?? 0;
                    var paginatedResources = allResources.Skip(offset).Take(limit).ToList();
                    SendResponse(new { resources = paginatedResources, total = allResources.Count }, requestId);
                }
                catch (Exception ex)
                {
                    SendError($"Fehler beim Auflisten der Ressourcen: {ex.Message}", -32000, requestId);
                }
                break;

            case "resources/read":
                try
                {
                    string? resourceUri = request?.Params?.Uri;
                    if (string.IsNullOrEmpty(resourceUri))
                    {
                        SendError("Resource URI ist erforderlich.", -32602, requestId);
                        break;
                    }
                    string? rawResourceFileName = resourceUri.Split('/').LastOrDefault();
                    // KORREKTUR: Den Dateinamen aus der URI dekodieren.
                    string? resourceFileName = WebUtility.UrlDecode(rawResourceFileName);

                    if (string.IsNullOrEmpty(resourceFileName))
                    {
                        SendError($"Ungültige Resource URI: '{resourceUri}'", -32602, requestId);
                        break;
                    }

                    string fullFilePath = Path.Combine(resourcesPath, resourceFileName);

                    if (!Path.GetFullPath(fullFilePath).StartsWith(Path.GetFullPath(resourcesPath)))
                    {
                        SendError($"Ungültiger Dateizugriff versucht für URI: '{resourceUri}'", -32000, requestId);
                        break;
                    }

                    if (File.Exists(fullFilePath))
                    {
                        byte[] fileBytes = File.ReadAllBytes(fullFilePath);
                        string mimeType = GetMimeType(resourceFileName);

                        object contentPart;
                        if (mimeType.StartsWith("text/"))
                        {
                            contentPart = new
                            {
                                type = "text",
                                text = Encoding.UTF8.GetString(fileBytes)
                            };
                        }
                        else
                        {
                            contentPart = new
                            {
                                type = "data",
                                mimeType = mimeType,
                                data = Convert.ToBase64String(fileBytes)
                            };
                        }

                        SendResponse(new { contents = new[] { contentPart } }, requestId);
                    }
                    else
                    {
                        SendError($"Resource für URI '{resourceUri}' nicht gefunden (Datei existiert nicht).", -32601, requestId);
                    }
                }
                catch (Exception ex)
                {
                    SendError($"Fehler beim Lesen der Ressource: {ex.Message}", -32000, requestId);
                }
                break;

            default:
                SendError($"Methode '{request?.Method}' wurde nicht gefunden.", -32601, requestId);
                break;
        }
    }
    catch (Exception ex)
    {
        SendError($"Ein interner Fehler ist aufgetreten: {ex.Message}", -32000, requestId);
    }

    Console.Out.Flush();
}

void HandleToolCall(ToolExecutionRequest request, object? requestId)
{
    string? toolToExecute = (request?.Params?.Tool ?? request?.Params?.Name)?.ToLower();
    JsonElement? argsElement = request?.Params?.Arguments is JsonElement element ? element : null;

    SendLogNotification("info", $"Führe Tool '{toolToExecute}' aus...");

    switch (toolToExecute)
    {
        case "HelloWorld":
            SendResponse(new { output = "HALLO WELT AUS DEM STDIO TOOL" }, requestId);
            break;
        case "directory_list":
            try
            {
                string? path = argsElement?.TryGetProperty("Pfad", out var pathElement) == true ? pathElement.GetString() : null;
                if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                {
                    SendError($"Verzeichnis nicht gefunden oder Pfad ungültig: '{path}'", -32602, requestId);
                    break;
                }
                string[] files = Directory.GetFiles(path);
                string[] modifiedFiles = files.Select(item => item.Replace("\\", "/")).ToArray();
                SendResponse(new { output = modifiedFiles }, requestId);
            }
            catch (Exception ex)
            {
                SendError($"Fehler beim Zugriff auf das Verzeichnis: {ex.Message}", -32000, requestId);
            }
            break;
        case "read_text_file":
            try
            {
                string? path = argsElement?.TryGetProperty("Pfad", out var pathElement) == true ? pathElement.GetString() : null;
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                {
                    SendError($"read_text_file Datei nicht gefunden oder Pfad ungültig: '{path}'", -32602, requestId);
                    break;
                }
                string content = File.ReadAllText(path, Encoding.UTF8);
                SendResponse(new { output = content }, requestId);
            }
            catch (Exception ex)
            {
                SendError($"Fehler beim Lesen der Datei: {ex.Message}", -32000, requestId);
            }
            break;
        case "read_media_file":
            try
            {
                string? path = argsElement?.TryGetProperty("Pfad", out var pathElement) == true ? pathElement.GetString() : null;
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                {
                    SendError($"read_media_file Datei nicht gefunden oder Pfad ungültig: '{path}'", -32602, requestId);
                    break;
                }
                byte[] fileBytes = File.ReadAllBytes(path);
                string base64Data = Convert.ToBase64String(fileBytes);
                string mimeType = GetMimeType(path);
                SendResponse(new { output = new { mimeType, data = base64Data } }, requestId);
            }
            catch (Exception ex)
            {
                SendError($"Fehler beim Lesen der Mediendatei: {ex.Message}", -32000, requestId);
            }
            break;
        case "write_text_file":
            try
            {
                string? path = argsElement?.TryGetProperty("Pfad", out var pathElement) == true ? pathElement.GetString() : null;
                if (string.IsNullOrWhiteSpace(path))
                {
                    SendError($"Pfad ungültig: '{path}'", -32602, requestId);
                    break;
                }
                string? newContent = argsElement?.TryGetProperty("Content", out var contentElement) == true ? contentElement.GetString() : null;
                if (string.IsNullOrWhiteSpace(newContent))
                {
                    SendError($"Inhalt nicht gefunden.", -32602, requestId);
                    break;
                }

                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }

                string[] content;
                if (!File.Exists(path))
                {
                    content = new string[0];
                }
                else
                {
                    content = File.ReadAllLines(path, Encoding.UTF8);
                    content = content.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
                }
                content = content.Append(newContent).ToArray();

                using (var sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    sw.Write(string.Join("\r", content));
                    sw.Flush();
                }
                SendResponse(new { output = newContent }, requestId);
            }
            catch (Exception ex)
            {
                SendError($"Fehler beim Schreiben der Datei: {ex.Message}", -32000, requestId);
            }
            break;
        case "copy_file":
            try
            {
                string? sourcePath = argsElement?.TryGetProperty("QuellePfad", out var sourceElement) == true ? sourceElement.GetString() : null;
                string? destPath = argsElement?.TryGetProperty("ZielPfad", out var destElement) == true ? destElement.GetString() : null;

                if (string.IsNullOrWhiteSpace(sourcePath) || string.IsNullOrWhiteSpace(destPath))
                {
                    SendError("Quell- und Zielpfad sind erforderlich.", -32602, requestId);
                    break;
                }
                if (!File.Exists(sourcePath))
                {
                    SendError($"Quelldatei nicht gefunden: '{sourcePath}'", -32602, requestId);
                    break;
                }

                File.Copy(sourcePath, destPath, true); // true = überschreiben, falls Zieldatei existiert
                SendResponse(new { output = $"Datei erfolgreich von '{sourcePath}' nach '{destPath}' kopiert." }, requestId);
            }
            catch (Exception ex)
            {
                SendError($"Fehler beim Kopieren der Datei: {ex.Message}", -32000, requestId);
            }
            break;
        case "move_file":
            try
            {
                string? sourcePath = argsElement?.TryGetProperty("QuellePfad", out var sourceElement) == true ? sourceElement.GetString() : null;
                string? destPath = argsElement?.TryGetProperty("ZielPfad", out var destElement) == true ? destElement.GetString() : null;

                if (string.IsNullOrWhiteSpace(sourcePath) || string.IsNullOrWhiteSpace(destPath))
                {
                    SendError("Quell- und Zielpfad sind erforderlich.", -32602, requestId);
                    break;
                }
                if (!File.Exists(sourcePath))
                {
                    SendError($"Quelldatei nicht gefunden: '{sourcePath}'", -32602, requestId);
                    break;
                }

                File.Move(sourcePath, destPath, true); // true = überschreiben
                SendResponse(new { output = $"Datei erfolgreich von '{sourcePath}' nach '{destPath}' verschoben." }, requestId);
            }
            catch (Exception ex)
            {
                SendError($"Fehler beim Verschieben der Datei: {ex.Message}", -32000, requestId);
            }
            break;
        case "delete_file":
            try
            {
                string? path = argsElement?.TryGetProperty("Pfad", out var pathElement) == true ? pathElement.GetString() : null;
                if (string.IsNullOrWhiteSpace(path))
                {
                    SendError("Ein gültiger Dateipfad ist erforderlich.", -32602, requestId);
                    break;
                }
                if (!File.Exists(path))
                {
                    SendError($"Datei nicht gefunden: '{path}'", -32602, requestId);
                    break;
                }
                File.Delete(path);
                SendResponse(new { output = $"Datei '{path}' erfolgreich gelöscht." }, requestId);
            }
            catch (Exception ex)
            {
                SendError($"Fehler beim Löschen der Datei: {ex.Message}", -32000, requestId);
            }
            break;
        case "create_directory":
            try
            {
                string? path = argsElement?.TryGetProperty("Pfad", out var pathElement) == true ? pathElement.GetString() : null;
                if (string.IsNullOrWhiteSpace(path))
                {
                    SendError("Ein gültiger Verzeichnispfad ist erforderlich.", -32602, requestId);
                    break;
                }
                if (Directory.Exists(path))
                {
                    SendResponse(new { output = $"Verzeichnis '{path}' existiert bereits." }, requestId);
                    break;
                }
                Directory.CreateDirectory(path);
                SendResponse(new { output = $"Verzeichnis '{path}' erfolgreich erstellt." }, requestId);
            }
            catch (Exception ex)
            {
                SendError($"Fehler beim Erstellen des Verzeichnisses: {ex.Message}", -32000, requestId);
            }
            break;
        case "delete_directory":
            try
            {
                string? path = argsElement?.TryGetProperty("Pfad", out var pathElement) == true ? pathElement.GetString() : null;
                if (string.IsNullOrWhiteSpace(path))
                {
                    SendError("Ein gültiger Verzeichnispfad ist erforderlich.", -32602, requestId);
                    break;
                }
                if (!Directory.Exists(path))
                {
                    SendError($"Verzeichnis nicht gefunden: '{path}'", -32602, requestId);
                    break;
                }
                Directory.Delete(path, true); // true = rekursiv, löscht auch Inhalt
                SendResponse(new { output = $"Verzeichnis '{path}' erfolgreich gelöscht." }, requestId);
            }
            catch (Exception ex)
            {
                SendError($"Fehler beim Löschen des Verzeichnisses: {ex.Message}", -32000, requestId);
            }
            break;
        case "read_time":
            SendResponse(new { output = DateTime.UtcNow.ToLocalTime().ToString("o", CultureInfo.InvariantCulture) }, requestId);

            break;

        default:
            SendError($"Tool '{toolToExecute}' in Methode 'tools/call' nicht gefunden.", -32601, requestId);
            break;
    }
}

void HandlePromptGet(ToolExecutionRequest request, object? requestId)
{
    string? promptName = request?.Params?.Name?.ToLower();
    if (promptName == "alle_ressourcen_zusammenfassen_final")
    {
        var messages = new[]
        {
            new {
                role = "system",
                content = new {
                    type = "text",
                    text = "Du bist ein AI-Agent für Dokumentenanalyse. Deine Aufgabe ist es, eine strukturierte Synthese zu erstellen, indem du dir zuerst eine Liste aller Ressourcen holst und dann für jede Ressource eine Vorschau anforderst und diese am Ende zusammenfasst."
                }
            },
            new {
                role = "user",
                content = new {
                    type = "text",
                    text = "Erstelle eine Zusammenfassung aller verfügbaren Ressourcen. Dein Vorgehen: 1. Rufe `resources/list` auf. 2. Rufe für jede `uri` aus der Ergebnisliste das Werkzeug `summarize_resource` auf. 3. Sammle alle Text-Vorschauen. 4. Erstelle eine finale Zusammenfassung im Markdown-Format mit der Hauptüberschrift '# Gesamtsynthese' und Unterüberschriften '## [Ressourcen-Bezeichnung]' für jedes Dokument basierend auf den Vorschauen."
                }
            }
        };
        SendResponse(new { messages }, requestId);
    }
    else
    {
        SendError($"Prompt '{promptName}' nicht gefunden.", -32601, requestId);
    }
}


void SendResponse(object payload, object? id)
{
    var response = new ToolExecutionResponse { Id = id, Result = payload };
    string jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
    Log($"Senden: {jsonResponse}");
    Console.WriteLine(jsonResponse);
}

void SendError(string errorMessage, int errorCode, object? id)
{
    var errorPayload = new ErrorPayload { Code = errorCode, Message = errorMessage };
    var response = new ToolExecutionResponse { Id = id, Error = errorPayload };
    string jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
    Log($"Senden (FEHLER): {jsonResponse}");
    Console.WriteLine(jsonResponse);
}

void SendLogNotification(string level, string message)
{
    var logNotification = new
    {
        jsonrpc = "2.0",
        method = "utilities/logging",
        @params = new
        {
            entry = new
            {
                level,
                message
            }
        }
    };
    string jsonNotification = JsonSerializer.Serialize(logNotification, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
    Log($"Senden (LOG): {jsonNotification}");
    Console.WriteLine(jsonNotification);
}


void Log(string message)
{
    if (!_isLogging) return;

    try
    {
        File.AppendAllText(logFilePath, $"[{DateTime.Now:HH:mm:ss.fff}] {message}\n");
    }
    catch { /* Ignoriere Fehler beim Loggen */ }
}

static string GetMimeType(string filePath)
{
    string extension = Path.GetExtension(filePath).ToLowerInvariant();
    return extension switch
    {
        ".txt" => "text/plain",
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".mp3" => "audio/mpeg",
        ".wav" => "audio/wav",
        ".mp4" => "video/mp4",
        ".pdf" => "application/pdf",
        _ => "application/octet-stream",
    };
}


// --- Datenklassen ---

public class ToolExecutionRequest
{
    public string? Jsonrpc { get; set; }
    public string? Method { get; set; }
    [JsonPropertyName("params")]
    public RequestParams? Params { get; set; }
    public object? Id { get; set; }
}

public class RequestParams
{
    [JsonPropertyName("protocolVersion")]
    public string? protocolVersion { get; set; }
    [JsonPropertyName("tool")]
    public string? Tool { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("arguments")]
    public object? Arguments { get; set; }
    [JsonPropertyName("entry")]
    public LogEntry? LogEntry { get; set; }
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }
    [JsonPropertyName("fragment")]
    public string? Fragment { get; set; }
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }
}

public class LogEntry
{
    [JsonPropertyName("level")]
    public string? Level { get; set; }
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

public class ErrorPayload
{
    [JsonPropertyName("code")]
    public int Code { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = "";
}

public class ToolExecutionResponse
{
    [JsonPropertyName("jsonrpc")]
    public string Jsonrpc { get; set; } = "2.0";
    [JsonPropertyName("result")]
    public object? Result { get; set; }
    [JsonPropertyName("error")]
    public ErrorPayload? Error { get; set; }
    [JsonPropertyName("id")]
    public object? Id { get; set; }
}

public class InitializeResult
{
    [JsonPropertyName("protocolVersion")]
    public string ProtocolVersion { get; set; } = "";
    [JsonPropertyName("serverInfo")]
    public ServerInfo ServerInfo { get; set; } = new();
    [JsonPropertyName("capabilities")]
    public ServerCapabilities Capabilities { get; set; } = new();
}

public class ServerInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("version")]
    public string Version { get; set; } = "";
}

public class ServerCapabilities
{
    [JsonPropertyName("tools")]
    public object? Tools { get; set; }
    [JsonPropertyName("prompts")]
    public object? Prompts { get; set; }
    [JsonPropertyName("resources")]
    public object? Resources { get; set; }
    [JsonPropertyName("utilities")]
    public object? Utilities { get; set; }
}
